namespace SDefence.Packet
{
    using Turret.Entity;

    public class TurretEntityPacket : IEntityPacket
    {
        private TurretEntity _entity;
        private int _index;
        private bool _isActiveUpgrade;
        private bool _isActiveUpTech;

        public TurretEntity Entity => _entity;
        public int Index => _index;
        public bool IsActiveUpgrade => _isActiveUpgrade;
        public bool IsActiveUpTech => _isActiveUpTech;

        public void SetData(int index, TurretEntity entity)
        {
            _entity = entity;
            _index = index;
        }

        public void SetData(bool isActiveUpgrade, bool isActiveUpTech)
        {
            _isActiveUpgrade = isActiveUpgrade;
            _isActiveUpTech = isActiveUpTech;
        }

    }
}