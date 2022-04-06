namespace SDefence.Packet
{
    public class HQCommandPacket : ICommandPacket
    {
        private bool _isUpgrade;
        private bool _isUpTech;

        public bool IsUpgrade => _isUpgrade;
        public bool IsUpTech => _isUpTech;

        public void SetData(bool isUpgrade, bool isUpTech)
        {
            _isUpgrade = isUpgrade;
            _isUpTech = isUpTech;
        }

    }
}