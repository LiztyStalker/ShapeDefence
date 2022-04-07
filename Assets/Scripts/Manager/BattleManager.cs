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
        private GameObject _gameObject;
        private LevelWaveData _levelWaveData;

        private HQActor _hqActor;
        private OrbitAction _orbitAction;
        private Dictionary<int, TurretActor> _turretDic;

        //터렛 위치 지정자
        private float _waveTime;

        private BulletManager _bulletMgr;
        private EffectManager _effectMgr;
        private AudioManager _audioMgr;

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

        }

        public void CleanUp()
        {
            _hqActor.RemoveOnBattlePacketListener(OnBattlePacketEvent);
            _waveTime = 0f;
            _bulletMgr.CleanUp();
            _effectMgr.CleanUp();
            _audioMgr.CleanUp();
        }

        public void RunProcess(float deltaTime)
        {
            _waveTime += deltaTime;
            foreach(var value in _turretDic.Values) 
            {
                value.RunProcess(deltaTime);
            }

            _orbitAction.RunProcess(deltaTime, _hqActor.transform.position);

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
                        }


                        var entity = hqPacket.Entity;
                        _hqActor.SetEntity(entity);

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
                            actor.transform.SetParent(_gameObject.transform);
                            _turretDic.Add(trPacket.Index, actor);
                                                        
                        }

                        var trActor = _turretDic[trPacket.Index];
                        var entity = trPacket.Entity;
                        trActor.SetEntity(entity);

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
            //switch (packet)
            //{
            //    case HQBattlePacket hqPacket:
            //        if(hqPacket.Actor.GetDurableValue<Shield>)
            //        break;
            //}
            //Sound 
            //Effect
            _battleEvent?.Invoke(packet);
        }

        #endregion

    }
}