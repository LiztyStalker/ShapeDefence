namespace SDefence.Packet
{
    public class TurretCommandPacket : ICommandPacket
    {
        private int _index;
        private bool _isUpgrade;
        private bool _isUpTech;
        private bool _isExpand;

        public int Index => _index;
        public bool IsUpgrade => _isUpgrade;
        public bool IsUpTech => _isUpTech;
        public bool IsExpand => _isExpand;

        public void SetData(int index)
        {
            _index = index;
        }

        public void SetData(bool isUpgrade, bool isUpTech, bool isExpand)
        {
            _isUpgrade = isUpgrade;
            _isUpTech = isUpTech;
            _isExpand = isExpand;
        }

    }
}