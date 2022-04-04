namespace SDefence.Packet
{
    using UnityEngine;
    using HQ.Entity;
    using Entity;

    public class HQEntityPacket : IEntityPacket
    {
        private HQEntity _entity;
        private bool _isActiveUpgrade;
        private bool _isActiveUpTech;

        public HQEntity Entity => _entity;
        public bool IsActiveUpgrade => _isActiveUpgrade;
        public bool IsActiveUpTech => _isActiveUpTech;

        public void SetData(HQEntity entity)
        {
            _entity = entity;
        }

        public void SetData(bool isActiveUpgrade, bool isActiveUpTech)
        {
            _isActiveUpgrade = isActiveUpgrade;
            _isActiveUpTech = isActiveUpTech;
        }

    }
}