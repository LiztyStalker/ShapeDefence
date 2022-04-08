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
    using Utility.ScriptableObjectData;
    using Utility.Effect.Data;
    using SDefence.BattleGen.Data;

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

    public class BattleManager
    {
        private PoolSystem<EnemyActor> _enemyPool;

        private Vector2 _appearSize;

        private GameObject _gameObject;
        private LevelWaveData _levelWaveData;

        private HQActor _hqActor;
        private OrbitAction _orbitAction;
        private Dictionary<int, TurretActor> _turretDic;
        private List<EnemyActor> _enemyActorList;

        //터렛 위치 지정자
        private float _waveTime;

        private BulletManager _bulletMgr;
        private EffectManager _effectMgr;
        private AudioManager _audioMgr;

        private BattleGenLevelData _battleGenLevelData;

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

            _appearSize = new Vector2(6f, 6f);
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
        }

        private EnemyActor CreateEnemyActor()
        {
            var actor = EnemyActor.Create();
            actor.name = "Actor@Enemy";
            actor.AddOnRetrieveListener(RetrieveEnemyActor);
            actor.AddOnAttackListener(OnAttackEvent);
            actor.Inactivate();
            _enemyActorList.Add(actor);
            return actor;
        }
        private void RetrieveEnemyActor(EnemyActor actor)
        {
            actor.Inactivate();
            _enemyActorList.Remove(actor);
            _enemyPool.RetrieveElement(actor);
            actor.RemoveOnRetrieveListener(RetrieveEnemyActor);
            actor.RemoveOnAttackListener(OnAttackEvent);
        }

        public void RunProcess(float deltaTime)
        {
            _waveTime += deltaTime;
            foreach(var value in _turretDic.Values) 
            {
                value.RunProcess(deltaTime);
            }

            _orbitAction.RunProcess(deltaTime, _hqActor.transform.position);

            for(int i = 0; i < _enemyActorList.Count; i++)
            {
                _enemyActorList[i].RunProcess(deltaTime, _hqActor.transform.position);
            }

            _bulletMgr.RunProcess(deltaTime);
            //if(_waveTime > 10f)
            //{
            //    NextWave();
            //}
        }

        //private void NextWave()
        //{
        //    _levelWaveData.IncreaseNumber();
        //    _hqActor.NextWave();
        //    //적 등장 바뀌기
        //}

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



        #region ##### Listener #####

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
                    var destroyEffect = DataStorage.Instance.GetDataOrNull<GameObject>(destroyKey);
                    _effectMgr.Activate(destroyEffect, destroyBattlePacket.Actor.NowPosition, 1f);
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
            }
        }

        private void OnAttackEvent(string bulletKey, IAttackable attackable)
        {
            //var bulletData = (BulletData)DataStorage.Instance.GetDataOrNull<ScriptableObject>(bulletKey, "BulletData");

            //if (bulletData != null)
            //{
            //    switch (attackable)
            //    {
            //        case EnemyActor eActor:
            //            _bulletMgr.Activate(attackable, bulletData, 1f, attackable.AttackPos, _hqActor.transform.position, actor =>
            //            {
            //                var effect = DataStorage.Instance.GetDataOrNull<GameObject>(bulletData.DestroyEffectDataKey);
            //                _effectMgr.Activate(effect, actor.NowPosition);
            //            });
            //            break;
            //        case TurretActor tActor:
            //            if (_enemyActorList.Count > 0)
            //            {
            //                var enemyActor = _enemyActorList[UnityEngine.Random.Range(0, _enemyActorList.Count)];
            //                _bulletMgr.Activate(attackable, bulletData, 1f, attackable.AttackPos, enemyActor.transform.position, actor =>
            //                {
            //                    var effect = DataStorage.Instance.GetDataOrNull<GameObject>(bulletData.DestroyEffectDataKey);
            //                    _effectMgr.Activate(effect, actor.NowPosition);                                
            //                });
            //            }
            //            break;
            //    }
            //}



            //var bulletData = DataStorage.Instance.GetDataOrNull<BulletData>(bulletKey);
            //if(bulletData != null)
            //    _bulletMgr.Activate(bulletData, 1f, attackable.AttackPos, )
        }

        private void AppearEnemy()
        {
            var data = SDefence.Enemy.EnemyData.Create();
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