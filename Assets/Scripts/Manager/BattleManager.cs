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
    using SDefence.Attack;
    using SDefence.BattleGen.Data;
    using SDefence.Attack.Usable;
    using SDefence.BattleGen.Entity;
    using SDefence.Enemy;

    #region ##### Orbit #####
    public class OrbitCase
    {
        private List<TurretActor> _list;

        private float _radius;
        private float _nowTime;
        private float _angle;
        private bool _isRev = false;

        public static OrbitCase Create(float radius, bool isRev = false) => new OrbitCase(radius, isRev);

        private OrbitCase(float radius, bool isRev)
        {
            _list = new List<TurretActor>();
            _radius = radius;
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
                SetPosition(_list[i], center, i);
            }
        }

        private void SetPosition(TurretActor actor, Vector2 center, int offset)
        {
            var dirX = Mathf.Cos(_nowTime + (_angle * offset) * Mathf.Deg2Rad) * _radius * 0.5f + center.x;
            var dirY = Mathf.Sin(_nowTime + (_angle * offset) * Mathf.Deg2Rad) * _radius * 0.5f + center.y;

            actor.transform.position = Vector2.Lerp(actor.transform.position, new Vector3(dirX, dirY, 0f), 0.01f);
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

        public void AddTurret(int index, TurretActor actor)
        {
            if (!_dic.ContainsKey(index))
            {
                _dic.Add(index, OrbitCase.Create((float)index, (index % 2 == 0)));
            }

            _dic[index].Add(actor);
        }
    }

    #endregion

    public class BattleManager
    {

#if UNITY_EDITOR
        public static bool IS_LOBBY_GEN = false;
#endif

        private const float NEXT_WAVE_TIME = 10f;

        public enum TYPE_BATTLE_ACTION { Lobby, Battle}

        private PoolSystem<EnemyActor> _enemyPool;

        private Vector2 _appearSize;

        private GameObject _gameObject;
        private LevelWaveData _levelWaveData;

        private HQActor _hqActor;
        private OrbitAction _orbitAction;
        private Dictionary<int, TurretActor> _turretDic;
        private List<EnemyActor> _enemyActorList;

        private List<AttackActionUsableData> _attackActionList;

        //터렛 위치 지정자
        private float _waveTime;

        private BulletManager _bulletMgr;
        private EffectManager _effectMgr;
        private AudioManager _audioMgr;


        private BattleGenEntity _battleGenEntity;
        //private BattleGenLevelData _battleGenLevelData;
        //private BattleGenWaveData? _battleGenWaveData;
        //private int _waveElementIndex = 0;

        private TYPE_BATTLE_ACTION _typeBattleAction = TYPE_BATTLE_ACTION.Lobby;

        public static BattleManager Create() => new BattleManager();

        private BattleManager()
        {
            _gameObject = new GameObject();
            _gameObject.name = "Mgr@Battle";
            _gameObject.transform.position = Vector3.zero;
            _gameObject.transform.localScale = Vector3.one;

            _turretDic = new Dictionary<int, TurretActor>();

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

            _battleGenEntity = null;

            //enemy remove event
            //_enemyActorList
            //actor.RemoveOnRetrieveListener(RetrieveEnemyActor);
            //actor.RemoveOnAttackListener(OnAttackEvent);

        }

        private EnemyActor CreateEnemyActor()
        {
            var actor = EnemyActor.Create();
            actor.name = "Actor@Enemy";
            actor.AddOnRetrieveListener(RetrieveEnemyActor);
            actor.AddOnAttackListener(OnAttackEvent);
            actor.Inactivate();
            return actor;
        }
        private void RetrieveEnemyActor(EnemyActor actor)
        {
            actor.Inactivate();
            _enemyActorList.Remove(actor);
            _enemyPool.RetrieveElement(actor);
        }

        public void SetBattle()
        {
            _typeBattleAction = TYPE_BATTLE_ACTION.Battle;

            //HQActor 비무적
            _hqActor.SetInvincible(false);
            _hqActor.SetDurableBattleEntity();

            //Turret 초기화
            foreach (var value in _turretDic.Values)
            {
                value.SetInvincible(false);
                value.Reset();
            }


            //Enemy 파괴
            for (int i = _enemyActorList.Count - 1; i >= 0; i--)
            {
                _enemyActorList[i].ForceRetrieve();
            }

            //Bullet 파괴
            _bulletMgr.ForceRetrieve();

            //PlayBattle Event 보내기
        }


        public void SetLobby()
        {
            _typeBattleAction = TYPE_BATTLE_ACTION.Lobby;

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


            //HQActor 무적
            _hqActor.SetInvincible(true);

            //Turret 무적
            //Turret 초기화
            foreach (var value in _turretDic.Values)
            {
                value.SetInvincible(true);
                value.Reset();
            }


            //Enemy 파괴
            for (int i = _enemyActorList.Count - 1; i >= 0; i--)
            {
                _enemyActorList[i].ForceRetrieve();
            }

            //Bullet 파괴
            _bulletMgr.ForceRetrieve();


            //LobbyBattle Event 보내기
        }

        public void RunProcess(float deltaTime)
        {
            switch (_typeBattleAction) 
            {
                case TYPE_BATTLE_ACTION.Battle:
                    _waveTime += deltaTime;
                    
                    _battleGenEntity.RunProcessBattle(deltaTime);

                    if (_waveTime > NEXT_WAVE_TIME)
                    {
                        NextWave();
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

            foreach (var value in _turretDic.Values)
            {
                value.RunProcess(deltaTime);
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

        private void NextWave()
        {
            _levelWaveData.IncreaseNumber();
            _hqActor.NextWave();

            //적 등장 바뀌기
            _battleGenEntity.SetLevelWave(_levelWaveData);

            //NextWaveBattlePacket
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
                            Debug.Log("HQReady");
                        }


                        var entity = hqPacket.Entity;
                        _hqActor.SetEntity(entity);
                        _hqActor.SetDurableBattleEntity();

                        var obj = DataStorage.Instance.GetDataOrNull<GameObject>(entity.GraphicObjectKey);
                        if (obj != null) _hqActor.SetGraphicObject(obj);

                        break;
                    }
                case TurretEntityPacket trPacket:
                    {
                        if (!_turretDic.ContainsKey(trPacket.Index))
                        {
                            var actor = TurretActor.Create();
                            actor.Activate();
                            actor.AddOnBattlePacketListener(OnBattlePacketEvent);
                            actor.AddOnAttackListener(OnAttackEvent);
                            actor.transform.SetParent(_gameObject.transform);
                            _turretDic.Add(trPacket.Index, actor);

                        }

                        var trActor = _turretDic[trPacket.Index];
                        var entity = trPacket.Entity;
                        trActor.SetEntity(entity);
                        trActor.SetDurableBattleEntity();

                        var obj = DataStorage.Instance.GetDataOrNull<GameObject>(entity.GraphicObjectKey);
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

                    if (destroyBattlePacket.Actor is HQActor)
                    {
                        //HQ이면 게임 패배 이벤트
                        //DefeatBattlePacket                        
                    }

                    //Enemy가 보스이면 게임 승리 이벤트
                    break;
            }
            _battleEvent?.Invoke(packet);
        }


        public void OnCommandPacketEvent(ICommandPacket packet)
        {
            switch (packet)
            {
                case EnemyCommandPacket enemyPacket:
                    AppearEnemy();
                    break;
                case BattleCommandPacket battlePacket:
                    SetBattle();
                    break;
            }
        }

        private void OnBulletAttackEvent(BulletActor actor, IAttackable attackable, IDamagable damagable, AttackActionUsableData actionData, System.Action endCallback)
        {
            actionData.SetOnAttackActionListener((range, isOverlap) =>
            {
                if (range < 0.1f)
                {
                    Debug.Log(damagable);
                    if (damagable != null) damagable.SetDamage(attackable.AttackUsableData);
                }
                else
                {
                    if (damagable != null) damagable.SetDamage(attackable.AttackUsableData);

                    if (attackable is TurretActor)
                    {
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
                    }
                    else if (attackable is EnemyActor)
                    {
                        foreach (var value in _turretDic.Values)
                        {
                            if ((TurretActor)damagable != value)
                            {
                                if (Vector2.Distance(value.NowPosition, actor.NowPosition) < range)
                                {
                                    value.SetDamage(attackable.AttackUsableData);
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
                            //Debug.Log(actor);
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

                actor.Activate();
                actor.SetPosition(AppearPosition());
                _enemyActorList.Add(actor);

                //보스?
                //BossBattlePacket
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogWarning($"{key} is not found");
            }
#endif
        }

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

            actor.Activate();
            actor.SetPosition(AppearPosition());
            _enemyActorList.Add(actor);
        }

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

    }
}