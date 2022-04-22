namespace SDefence.Packet
{
    using HQ.Entity;
    using Asset.Entity;
    using Turret.Entity;

    public class HQEntityPacket : IEntityPacket
    {
        public HQEntity Entity;
        public bool IsActiveUpgrade;
        public bool IsActiveUpTech;
    }

    public class TurretEntityPacket : IEntityPacket
    {
        public TurretEntity Entity;
        public int OrbitIndex;
        public int Index;
        public bool IsActiveUpgrade;
        public bool IsActiveUpTech;
    }

    public class TurretOrbitEntityPacket : IEntityPacket
    {
        public int OrbitCount;
    }
    
    public class TurretArrayEntityPacket : IEntityPacket
    {
        public TurretEntityPacket[] packets;
        public bool IsExpand;
    }

    public class AssetEntityPacket : IEntityPacket
    {
        public AssetUsableEntity Entity;
    }

}