namespace SDefence.Manager
{
    using HQ;
    using Turret;
    using Packet;

    public class GameSystem
    {
        private HQManager _hqMgr;
        private TurretManager _turretMgr;

        public static GameSystem Create() => new GameSystem();

        private GameSystem()
        {
            _hqMgr = HQManager.Create();
            _turretMgr = TurretManager.Create();

            _hqMgr.AddOnEntityPacketListener(OnEntityPacketEvent);
            _turretMgr.AddOnEntityPacketListener(OnEntityPacketEvent);
        }

        public void Initialize()
        {
            _hqMgr.Initialize();
            _turretMgr.Initialize();
        }

        public void CleanUp()
        {
            _hqMgr.RemoveOnEntityPacketListener(OnEntityPacketEvent);
            _turretMgr.RemoveOnEntityPacketListener(OnEntityPacketEvent);
        }
        public void RefreshAll()
        {
            _hqMgr.Refresh();
            _turretMgr.Refresh();
        }

        public void OnCommandPacketEvent(ICommandPacket packet)
        {
            switch (packet)
            {
                case HQCommandPacket hqPacket:
                    //UpTech -> trManager.ExpandOrbit
                    if (hqPacket.IsUpTech)
                    {
                        //Tmp
                        //_turretMgr.ExpandOrbit(2);

                        //UpTech -> turretCount
                    }
                    _hqMgr.OnCommandPacketEvent(hqPacket);
                    break;
                case TurretCommandPacket trPacket:
                    _turretMgr.OnCommandPacketEvent(trPacket);
                    break;
            }

        }

        #region ##### Listener #####

        private System.Action<IEntityPacket> _packetEvent;
        public void AddOnRefreshEntityPacketListener(System.Action<IEntityPacket> act) => _packetEvent += act;
        public void RemoveOnRefreshEntityPacketListener(System.Action<IEntityPacket> act) => _packetEvent -= act;
        private void OnEntityPacketEvent(IEntityPacket packet) => _packetEvent?.Invoke(packet);

        #endregion
    }
}