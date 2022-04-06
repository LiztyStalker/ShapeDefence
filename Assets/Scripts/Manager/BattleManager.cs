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

    public class BattleManager
    {
        private GameObject _gameObject;
        private LevelWaveData _levelWaveData;

        private HQActor _hqActor;
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
                    if(_hqActor == null)
                    {
                        _hqActor = HQActor.Create();
                        _hqActor.Activate();
                        _hqActor.AddOnBattlePacketListener(OnBattlePacketEvent);
                    }

                    var entity = hqPacket.Entity;
                    _hqActor.SetEntity(entity);

                    var obj = DataStorage.Instance.GetDataOrNull<GameObject>(entity.GraphicObjectKey);
                    if(obj != null) _hqActor.SetGraphicObject(obj);
                    break;
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