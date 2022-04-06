namespace SDefence.Manager
{
    using HQ;
    using Packet;

    public class GameSystem
    {
        private HQManager _hqMgr;

        public static GameSystem Create() => new GameSystem();

        private GameSystem()
        {
            _hqMgr = HQManager.Create();
            _hqMgr.AddOnEntityPacketListener(OnEntityPacketEvent);
        }

        public void Initialize()
        {
            _hqMgr.Initialize();
        }

        public void CleanUp()
        {
            _hqMgr.RemoveOnEntityPacketListener(OnEntityPacketEvent);
        }
        public void RefreshAll()
        {
            _hqMgr.Refresh();
        }

        public void OnCommandPacketEvent(ICommandPacket packet)
        {
            switch (packet)
            {
                default:
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