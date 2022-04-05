namespace SDefence.Manager
{
    using Actor;
    using Packet;
    using Storage;
    using UnityEngine;

    public class BattleManager
    {
        private GameObject _gameObject;
        

        private HQActor _hqActor;
        private float _waveTime;
        

        public static BattleManager Create() => new BattleManager();

        private BattleManager()
        {
            _gameObject = new GameObject();
            _gameObject.name = "Mgr@Battle";
            _gameObject.transform.position = Vector3.zero;
            _gameObject.transform.localScale = Vector3.one;
        }

        public void RunProcess(float deltaTime)
        {
            _waveTime += deltaTime;
        }

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

        public void CleanUp()
        {
            _hqActor.RemoveOnBattlePacketListener(OnBattlePacketEvent);
        }

        #region ##### Listener #####

        private System.Action<IBattlePacket> _battleEvent;
        public void AddOnBattlePacketListener(System.Action<IBattlePacket> act) => _battleEvent += act;
        public void RemoveOnBattlePacketListener(System.Action<IBattlePacket> act) => _battleEvent -= act;
        private void OnBattlePacketEvent(IBattlePacket packet)
        {
            _battleEvent?.Invoke(packet);
        }

        #endregion

    }
}