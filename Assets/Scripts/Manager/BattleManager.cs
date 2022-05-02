namespace SDefence.Manager
{
    using Actor;
    using Packet;
    using Data;
    using Storage;
    using UnityEngine;
    using Utility.Bullet;
    using Utility.Effect;
    using UtilityManager;
    using System.Collections.Generic;
    using PoolSystem;
    using SDefence.Turret.Entity;
    using Utility.Bullet.Data;
    using Attack;
    using BattleGen.Data;
    using Attack.Usable;
    using BattleGen.Entity;
    using Enemy;
    using Utility.IO;
    using Asset.Entity;

    #region ##### Orbit #####
    public class OrbitCase
    {
        private const float RADIUS_OFFSET = 0.5f;

        private List<TurretActor> _list;

        private float _radius;
        private float _nowTime;
        private float _angle;
        private bool _isRev = false;

        public static OrbitCase Create(float radius, bool isRev = false) => new OrbitCase(radius, isRev);

        private OrbitCase(float radius, bool isRev)
        {
            _list = new List<TurretActor>();
            _radius = radius * RADIUS_OFFSET;
            _isRev = isRev;
        }

        public void RunProcess(float deltaTime, Vector2 center)
        {
            if (_isRev)
            {
                _nowTime -= deltaTime;
                if (_nowTime < -360f) _nowTime += 360f;
            }
            else
            {
                _nowTime += deltaTime;
                if (_nowTime > 360f) _nowTime -= 360f;
            }


            for (int i = 0; i < _list.Count; i++)
            {
                SetPosition(_list[i], center, i, deltaTime);
            }
        }

        private void SetPosition(TurretActor actor, Vector2 center, int offset, float deltaTime)
        {
            var dirX = Mathf.Cos(_nowTime + (_angle * offset) * Mathf.Deg2Rad) * _radius + center.x;
            var dirY = Mathf.Sin(_nowTime + (_angle * offset) * Mathf.Deg2Rad) * _radius + center.y;

            actor.transform.position = Vector2.Lerp(actor.transform.position, new Vector3(dirX, dirY, 0f), deltaTime);
        }

        public void Add(TurretActor actor)
        {
            if (!_list.Contains(actor))
            {
                _list.Add(actor);
                SetAngle();
            }
        }

        private void SetAngle()
        {
            _angle = 360f / (float)_list.Count;
        }
    }

    public class OrbitAction
    {
        private Dictionary<int, OrbitCase> _dic;

        public static OrbitAction Create() => new OrbitAction();

        private OrbitAction()
        {
            _dic = new Dictionary<int, OrbitCase>();
        }

        public void RunProcess(float deltaTime, Vector2 center)
        {
            foreach(var value in _dic.Values)
            {
                value.RunProcess(deltaTime, center);
            }           
        }        

        public void AddTurret(int orbitIndex, TurretActor actor)
        {
            if (!_dic.ContainsKey(orbitIndex))
            {
                _dic.Add(orbitIndex, OrbitCase.Create((float)orbitIndex, (orbitIndex % 2 == 0)));
            }

            _dic[orbitIndex].Add(actor);
        }
    }

    #endregion

    public class BattleManager : ISavable
    {

#if UNITY_EDITOR
        public static bool IS_LOBBY_GEN = true;
#endif

        private const float NEXT_WAVE_TIME = 5f;

        public enum TYPE_BATTLE_ACTION { Lobby, Battle}

        private PoolSystem<EnemyActor> _enemyPool;

        //등장 위치 - 카메라 화면 가장자리 정의 필요
        private Vector2 _appearSize;

        
        private GameObject _gameObject;

        private LevelWaveData _levelWaveData;

        private HQActor _hqActor;
        private OrbitAction _orbitAction;
        private Dictionary<int, List<TurretActor>> _turretDic;
        private List<TurretActor> _turretList;
        private List<EnemyActor> _enemyActorList;

        private List<AttackActionUsableData> _attackActionList;

        private AssetUsableEntity _battleAssetEntity;

        //터렛 위치 지정자
        private float _waveTime;

        private BulletManager _bulletMgr;
        private EffectManager _effectMgr;
        private AudioManager _audioMgr;


        private BattleGenEntity _battleGenEntity;


        private TYPE_BATTLE_ACTION _typeBattleAction = TYPE_BATTLE_ACTION.Lobby;

        private bool _isDefeat = false;

        public static BattleManager Create() => new BattleManager();

        private BattleManager()
        {
            _gameObject = new GameObject();
            _gameObject.name = "Mgr@Battle";
            _gameObject.transform.position = Vector3.zero;
            _gameObject.transform.localScale = Vector3.one;

            _turretDic = new Dictionary<int, List<TurretActor>>();
            _turretList = new List<TurretActor>();

            _orbitAction = OrbitAction.Create();

            _bulletMgr = BulletManager.Current;
            _effectMgr = EffectManager.Current;
            _audioMgr = AudioManager.Current;

            _enemyPool = PoolSystem<EnemyActor>.Create();
            _enemyPool.Initialize(CreateEnemyActor);

            _enemyActorList = new List<EnemyActor>();

            _levelWaveData = new LevelWaveData();

            _attackActionList = new List<AttackActionUsableData>();

            _appearSize = new Vector2(6f, 6f);

            _battleAssetEntity = AssetUsableEntity.Create();

            _battleGenEntity = BattleGenEntity.Create();
            _battleGenEntity.SetOnAppearEnemyListener(OnAppearEnemyEvent);
        }

        public void CleanUp()
        {
            _hqActor.RemoveOnBattlePacketListener(OnBattlePacketEvent);
            _waveTime = 0f;
            _bulletMgr.CleanUp();
            _effectMgr.CleanUp();
            _audioMgr.CleanUp();

            _enemyPool.CleanUp();
            _enemyActorList.Clear();

            _battleAssetEntity.CleanUp();

            _battleGenEntity = null;
        }

        private EnemyActor CreateEnemyActor()
        {
            var actor = EnemyActor.Create();
            actor.transform.SetParent(_gameObject.transform);
            actor.transform.localScale = Vector3.one;
            actor.name = "Actor@Enemy";
            actor.AddOnRetrieveListener(OnRetrieveEnemyActorEvent);
            actor.SetOnDestroyListener(OnEnemyDestroyBattlePacketEvent);
            actor.AddOnBattlePacketListener(OnBattlePacketEvent);
            actor.AddOnAttackListener(OnAttackEvent);
            actor.Inactivate();
            return actor;
        }

        private void OnRetrieveEnemyActorEvent(EnemyActor actor)
        {
            _enemyActorList.Remove(actor);
            _enemyPool.RetrieveElement(actor);
            actor.Inactivate();
        }

        private void OnEnemyDestroyBattlePacketEvent(EnemyActor actor, bool isReward)
        {
            var packet = new DestroyBattlePacket();
            packet.Actor = actor;
            packet.IsReward = isReward;
            OnBattlePacketEvent(packet);
        }

        public void SetBattle()
        {
            Debug.Log("Start Battle");

            _isDefeat = false;
            _waveTime = 0f;
            _typeBattleAction = TYPE_BATTLE_ACTION.Battle;

            //HQActor 비무적
            _hqActor.SetInvincible(false);
            _hqActor.SetDurableBattleEntity();

            //Turret 초기화
            //MainTurret == invincible
            for (int i = 0; i < _turretList.Count; i++)
            {
                _turretList[i].SetInvincible(false);
                _turretList[i].Reset();
            }


            //Enemy 파괴
            for (int i = _enemyActorList.Count - 1; i >= 0; i--)
            {
                _enemyActorList[i].ForceRetrieve();
            }

            //Bullet 파괴
            _bulletMgr.ForceRetrieve();

            //LevelWave 적용하기
            _battleGenEntity.SetBattle(_levelWaveData);

            //AssetEntity 초기화
            _battleAssetEntity.Clear();

            //PlayBattle Event 보내기
            OnPlayBattlePacketEvent(_battleGenEntity.GetBossIconKey(), _levelWaveData);
            OnAssetBattlePacketEvent(_battleAssetEntity);
            OnLevelWaveBattlePacketEvent();
        }


        private void SetBattleGen()
        {
            var enemyLevelDataKey = string.Format("Level{0:d4}", _levelWaveData.GetLevel());
            var enemyLevelData = (BattleGenLevelData)DataStorage.Instance.GetDataOrNull<ScriptableObject>(enemyLevelDataKey, "BattleGenLevelData");

            if (enemyLevelData != null)
            {
                _battleGenEntity.SetData(enemyLevelData);
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogWarning($"{enemyLevelDataKey} is not Found");
            }
#endif
        }

        public void SetLobby()
        {
            Debug.Log("Start Lobby");
            _isDefeat = false;

            _typeBattleAction = TYPE_BATTLE_ACTION.Lobby;
            _waveTime = 0f;

            SetBattleGen();

            //HQActor 무적
            _hqActor.SetInvincible(true);

            //Turret 무적 / 초기화
            for (int i = 0; i < _turretList.Count; i++)
            {
                _turretList[i].SetInvincible(true);
                _turretList[i].Reset();
            }

            //Enemy 파괴
            for (int i = _enemyActorList.Count - 1; i >= 0; i--)
            {
                _enemyActorList[i].ForceRetrieve();
            }

            //Bullet 파괴
            _bulletMgr.ForceRetrieve();


            //Lobby Gen
            _battleGenEntity.SetLobby();

            //LobbyBattle Event 보내기
            OnLevelWaveBattlePacketEvent();
        }

        public void RunProcess(float deltaTime)
        {
            switch (_typeBattleAction) 
            {
                case TYPE_BATTLE_ACTION.Battle:
                    if (!_isDefeat)
                    {
                        _waveTime += deltaTime;

                        _battleGenEntity.RunProcessBattle(deltaTime);

                        if (!_levelWaveData.IsLastWave())
                        {
                            if (_waveTime > NEXT_WAVE_TIME)
                            {
                                NextWave();
                            }
                        }
                    }
                    break;
                case TYPE_BATTLE_ACTION.Lobby:

#if UNITY_EDITOR
                    if(IS_LOBBY_GEN) _battleGenEntity.RunProcessLobby(deltaTime);
#else
                    _battleGenEntity.RunProcessLobby(deltaTime);
#endif

                    break;
            }


            if (!_isDefeat)
            {
                for (int i = 0; i < _turretList.Count; i++)
                {
                    _turretList[i].RunProcess(deltaTime);
                }

                _orbitAction.RunProcess(deltaTime, _hqActor.transform.position);

                for (int i = 0; i < _enemyActorList.Count; i++)
                {
                    _enemyActorList[i].RunProcess(deltaTime, _hqActor.transform.position);
                }

                _bulletMgr.RunProcess(deltaTime);

                for (int i = 0; i < _attackActionList.Count; i++)
                {
                    _attackActionList[i].RunProcess(deltaTime);
                }
            }
        }

        private void NextWave()
        {
            _levelWaveData.IncreaseNumber();
            _hqActor.NextWave();

            _waveTime = 0f;

            //적 등장 바뀌기
            _battleGenEntity.SetBattle(_levelWaveData);


            OnLevelWaveBattlePacketEvent();
            //Debug.Log($"NextWave {_levelWaveData.GetLevel()} / {_levelWaveData.GetWave()}");
        }



        #region ##### Listener #####

        private void OnAppearEnemyEvent(string enemyDataKey)
        {
            AppearEnemy(enemyDataKey);
        }

        public void OnEntityPacketEvent(IEntityPacket packet)
        {
            switch (packet)
            {
                case HQEntityPacket hqPacket:
                    {
                        if (_hqActor == null)
                        {
                            _hqActor = HQActor.Create();
                            _hqActor.Activate();
                            _hqActor.AddOnBattlePacketListener(OnBattlePacketEvent);
                            _hqActor.transform.SetParent(_gameObject.transform);
                            //Debug.Log("HQReady");
                        }


                        var entity = hqPacket.Entity;
                        _hqActor.SetEntity(entity);
                        _hqActor.SetDurableBattleEntity();

                        var obj = DataStorage.Instance.GetDataOrNull<GameObject>(entity.GraphicObjectKey);
                        if (obj != null) _hqActor.SetGraphicObject(obj);

                        break;
                    }
                case TurretArrayEntityPacket pk:
                    for(int i = 0; i < pk.packets.Length; i++)
                    {
                        OnEntityPacketEvent(pk.packets[i]);
                    }
                    break;
                case TurretEntityPacket pk:
                    {
                        var orbitIndex = pk.OrbitIndex;
                        var index = pk.Index;
                        if (!_turretDic.ContainsKey(orbitIndex))
                        {
                            _turretDic.Add(orbitIndex, new List<TurretActor>());
                        }

                        if (index >= _turretDic[orbitIndex].Count)
                        {
                            var actor = TurretActor.Create(orbitIndex);
                            actor.AddOnBattlePacketListener(OnBattlePacketEvent);
                            actor.AddOnAttackListener(OnAttackEvent);
                            actor.transform.SetParent(_gameObject.transform);
                            actor.Activate();
                            _turretDic[orbitIndex].Add(actor);
                            _turretList.Add(actor);
                        }

                        var trActor = _turretDic[orbitIndex][index];
                        var entity = pk.Entity;
                        trActor.SetEntity(entity);
                        trActor.SetDurableBattleEntity();

                        var obj = DataStorage.Instance.GetDataOrNull<GameObject>(entity.GraphicObjectKey, "Turret");
                        if (obj != null) trActor.SetGraphicObject(obj);

                        _orbitAction.AddTurret(entity.OrbitIndex, trActor);

                        break;
                    }
            }
        }







        private System.Action<IBattlePacket> _battleEvent;
        public void AddOnBattlePacketListener(System.Action<IBattlePacket> act) => _battleEvent += act;
        public void RemoveOnBattlePacketListener(System.Action<IBattlePacket> act) => _battleEvent -= act;
        private void OnBattlePacketEvent(IBattlePacket packet)
        {
            switch (packet)
            {
                case HitBattlePacket hitPacket:

                    string key = (hitPacket.IsShieldHit) ? "ShieldHit" : "ActorHit";
                    var hitEffect = DataStorage.Instance.GetDataOrNull<GameObject>(key);
                    _effectMgr.Activate(hitEffect, hitPacket.NowPosition, 1f);


                    break;
                case DestroyBattlePacket destroyBattlePacket:
                    
                    var destroyKey = "DestroyActor";

                    var destroyEffectKey = DataStorage.Instance.GetDataOrNull<GameObject>(destroyKey);
                    _effectMgr.Activate(destroyEffectKey, destroyBattlePacket.Actor.NowPosition, 1f);

                    destroyBattlePacket.TypeBattleAction = _typeBattleAction;

                    //전투일때
                    if (_typeBattleAction == TYPE_BATTLE_ACTION.Battle)
                    {

                        switch (destroyBattlePacket.Actor)
                        {
                            case HQActor actor:
                                {
                                    //HQ이면 게임 패배 이벤트
                                    var pk = new DefeatBattlePacket();
                                    pk.AssetEntity = _battleAssetEntity;
                                    _battleEvent?.Invoke(pk);

                                    _isDefeat = true;
                                }
                                break;
                            case EnemyActor actor:
                                {
                                    if (destroyBattlePacket.IsReward)
                                    {
                                        //전투 중 적 처치시 획득 재화
                                        _battleAssetEntity.Add(actor.RewardAssetUsableData);
                                        OnAssetBattlePacketEvent(_battleAssetEntity);
                                    }

                                    //Enemy가 보스이면 게임 승리 이벤트
                                    //또는 적이 없으면 게임 승리 이벤트
                                    if (_battleGenEntity.IsEnd() && _enemyActorList.Count == 0 && _levelWaveData.IsLastWave())
                                    {
                                        var pk = new ClearBattlePacket();
                                        pk.AssetEntity = _battleAssetEntity;
                                        _battleEvent?.Invoke(pk);
                                    }
                                }
                                break;
                        }                        
                    }
                    break;
            }
            _battleEvent?.Invoke(packet);
        }

        private void OnAssetBattlePacketEvent(AssetUsableEntity assetEntity)
        {
            var packet = new AssetBattlePacket();
            packet.AssetEntity = assetEntity;
            _battleEvent?.Invoke(packet);
        }

        private void OnPlayBattlePacketEvent(string bossIconKey, LevelWaveData lwData)
        {
            var packet = new PlayBattlePacket();
            packet.data = lwData;
            packet.BossIcon = DataStorage.Instance.GetDataOrNull<Sprite>(bossIconKey, "Icon");
            _battleEvent?.Invoke(packet);
        }

        private void OnLevelWaveBattlePacketEvent()
        {
            var packet = new LevelWaveBattlePacket();
            packet.data = _levelWaveData;
            _battleEvent?.Invoke(packet);
        }

        private void OnAppearEnemyBattlePacketEvent(TYPE_ENEMY_STYLE typeEnemyStyle)
        {
            var packet = new AppearEnemyBattlePacket();
            packet.TypeEnemyStyle = typeEnemyStyle;
            _battleEvent?.Invoke(packet);
        }



        public void OnCommandPacketEvent(ICommandPacket packet)
        {
            switch (packet)
            {
#if UNITY_EDITOR
                case EnemyCommandPacket pk:
                    AppearEnemy();
                    break;
#endif
                case PlayBattleCommandPacket pk:
                    //전투시작 PlayBattleCommandPacket - Lobby To Battle
                    SetBattle();
                    break;
                case RetryCommandPacket pk:
                    //재도전 RetryCommandPacket
                    _levelWaveData.Retry();
                    SetBattleGen();
                    SetBattle();
                    break;
                case AdsResultCommandPacket pk:
                    _levelWaveData.IncreaseNumber();
                    SetLobby();
                    break;
                case ToLobbyCommandPacket pk:
                    //로비 ToLobbyCommandPacket - Battle To Lobby
                    if(pk.IsClear) _levelWaveData.IncreaseNumber();
                    else _levelWaveData.Retry();
                    SetLobby();
                    break;
                case NextLevelCommandPacket pk:
                    //다음단계 NextLevelCommandPacket
                    _levelWaveData.IncreaseNumber();
                    SetBattleGen();
                    SetBattle();
                    break;
            }
        }

        private void OnBulletAttackEvent(BulletActor actor, IAttackable attackable, IDamagable damagable, AttackActionUsableData actionData, System.Action endCallback)
        {
            actionData.SetOnAttackActionListener((range, isOverlap) =>
            {
                //단일
                if (range < 0.01f)
                {
                    if (damagable != null)
                    {
                        damagable.SetDamage(attackable.AttackUsableData);
                    }
                }
                //멀티
                else
                {
                    if (damagable != null) 
                        damagable.SetDamage(attackable.AttackUsableData);


                    switch (attackable)
                    {
                        case TurretActor turret:
                            for (int i = 0; i < _enemyActorList.Count; i++)
                            {
                                if ((EnemyActor)damagable != _enemyActorList[i])
                                {
                                    if (Vector2.Distance(_enemyActorList[i].AttackPos, actor.NowPosition) < range)
                                    {
                                        _enemyActorList[i].SetDamage(attackable.AttackUsableData);
                                    }
                                }
                            }
                            break;
                        case EnemyActor enemy:
                            for (int i = 0; i < _turretList.Count; i++)
                            {
                                var turret = _turretList[i];
                                if ((TurretActor)damagable != turret)
                                {
                                    if (Vector2.Distance(turret.NowPosition, actor.NowPosition) < range)
                                    {
                                        turret.SetDamage(attackable.AttackUsableData);
                                    }
                                }
                            }

                            if ((HQActor)damagable != _hqActor)
                            {
                                if (Vector2.Distance(_hqActor.NowPosition, actor.NowPosition) < range)
                                {
                                    _hqActor.SetDamage(attackable.AttackUsableData);
                                }
                            }
                            break;
                    }
                }

                ////이펙트
                var effect = DataStorage.Instance.GetDataOrNull<GameObject>(actor.DestroyEffectDataKey);
                _effectMgr.Activate(effect, actor.NowPosition, 1f);

                var sfx = DataStorage.Instance.GetDataOrNull<AudioClip>(actor.DestroyEffectSfxKey);
                _audioMgr.Activate(sfx, AudioManager.TYPE_AUDIO.SFX);

            });


            actionData.SetOnEndedActionListener(actionData => 
            {
                var effect = DataStorage.Instance.GetDataOrNull<GameObject>(actor.DestroyEffectDataKey);
                _effectMgr.Activate(effect, actor.NowPosition, 1f);

                var sfx = DataStorage.Instance.GetDataOrNull<AudioClip>(actor.DestroyEffectSfxKey);
                _audioMgr.Activate(sfx, AudioManager.TYPE_AUDIO.SFX);

                _attackActionList.Remove(actionData);
                endCallback?.Invoke();

            });


            _attackActionList.Add(actionData);

        }
       

        private void OnAttackEvent(string bulletKey, IAttackable attackable)
        {
            //탄환 발사
            var bulletData = (BulletData)DataStorage.Instance.GetDataOrNull<ScriptableObject>(bulletKey, "BulletData");

            if (bulletData != null)
            {
                BulletActor actor = null;

                switch (attackable)
                {
                    case EnemyActor eActor:
                        actor = _bulletMgr.Activate(attackable, bulletData, 0.1f, attackable.AttackPos, _hqActor.transform.position, OnBulletAttackEvent, null);
                        break;
                    case TurretActor tActor:
                        if (_enemyActorList.Count > 0)
                        {
                            //타겟팅 정하면 되도록 변경하지 않기
                            //적이 사망하면 타겟팅 초기화
                            var enemyActor = _enemyActorList[UnityEngine.Random.Range(0, _enemyActorList.Count)];
                            actor = _bulletMgr.Activate(attackable, bulletData, 0.1f, attackable.AttackPos, enemyActor.NowPosition, OnBulletAttackEvent, null);
                        }
                        break;
                }

                if (actor != null)
                {
                    var effect = DataStorage.Instance.GetDataOrNull<GameObject>(bulletData.ActiveEffectDataKey);
                    _effectMgr.Activate(effect, attackable.AttackPos, 1f);

                    var sfx = DataStorage.Instance.GetDataOrNull<AudioClip>(bulletData.ActiveEffectSfxKey);
                    _audioMgr.Activate(sfx, AudioManager.TYPE_AUDIO.SFX);
                }

            }
        }

        private void AppearEnemy(string key)
        {
            var enemyData = (EnemyData)DataStorage.Instance.GetDataOrNull<ScriptableObject>(key, "EnemyData");
            if (enemyData != null) 
            {
                var data = enemyData;
                var entity = EnemyEntity.Create();
                entity.Initialize(data);
                entity.SetLevelWave(_levelWaveData);

                var actor = _enemyPool.GiveElement();
                actor.SetEntity(entity);
                actor.SetDurableBattleEntity();

                var obj = DataStorage.Instance.GetDataOrNull<GameObject>(entity.GraphicObjectKey);
                if (obj != null) actor.SetGraphicObject(obj);

                _enemyActorList.Add(actor);
                actor.Activate();
                actor.SetPosition(AppearPosition());
                //Debug.Log("Appear " + actor.GetInstanceID() + " " + _enemyActorList.Count);

                //보스? 특수? 
                OnAppearEnemyBattlePacketEvent(enemyData.TypeEnemyStyle);
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogWarning($"{key} is not found");
            }
#endif
        }

#if UNITY_EDITOR
        private void AppearEnemy()
        {
            var data = EnemyData.Create();
            var entity = EnemyEntity.Create();
            entity.Initialize(data);
            entity.SetLevelWave(_levelWaveData);

            var actor = _enemyPool.GiveElement();
            actor.SetEntity(entity);
            actor.SetDurableBattleEntity();

            var obj = DataStorage.Instance.GetDataOrNull<GameObject>(entity.GraphicObjectKey);
            if (obj != null) actor.SetGraphicObject(obj);

            _enemyActorList.Add(actor);
            actor.Activate();
            actor.SetPosition(AppearPosition());
        }
#endif

        private Vector2 AppearPosition()
        {
            Vector2 pos;
            if(25 < UnityEngine.Random.Range(0, 100))
            {
                pos.y = Random.Range(-_appearSize.y, _appearSize.y);
                if (UnityEngine.Random.Range(0, 100) > 50)
                {
                    pos.x = _appearSize.x;
                }
                else
                {
                    pos.x = -_appearSize.x;
                }
            }
            else
            {
                pos.x = Random.Range(-_appearSize.x, _appearSize.x);
                if (UnityEngine.Random.Range(0, 100) > 50)
                {
                    pos.y = _appearSize.y;
                }
                else
                {
                    pos.y = -_appearSize.y;
                }
            }
            return pos;
        }

        #endregion

        #region ##### Savable #####
        public string SavableKey() => typeof(BattleManager).Name;

        public SavableData GetSavableData()
        {
            var data = SavableData.Create();
            data.AddData(typeof(LevelWaveData).Name, _levelWaveData.GetLevel());
            return data;
        }

        public void SetSavableData(SavableData data)
        {
            if (data != null)
            {
                _levelWaveData.SetLevel(data.GetValue<int>(typeof(LevelWaveData).Name));
            }
        }

        #endregion

    }
}