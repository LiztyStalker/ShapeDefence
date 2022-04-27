namespace SDefence.Packet
{
    using HQ.Entity;
    using Asset.Entity;
    using Turret.Entity;
    using Entity;
    using Tech;
    public struct TechPacketElement
    {
        public TechRawElement Element;
        public bool IsActiveTech;
    }
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
        public int OrbitIndex;
        public bool IsExpand;
    }

    public class AssetEntityPacket : IEntityPacket
    {
        public AssetUsableEntity Entity;
    }


    public class OpenTechEntityPacket : IEntityPacket
    {       
        public TechPacketElement[] Elements;
    }

    public class UpTechEntityPacket : IEntityPacket
    {
        public IEntity PastEntity;
        public IEntity NowEntity;
    }

    public class OpenDisassembleEntityPacket : IEntityPacket
    {
        public IEntity Entity;
    }

    public class DisassembleEntityPacket : IEntityPacket
    {
        public IEntity PastEntity;
        public IEntity NowEntity;
    }

    public class RewardOfflineEntityPacket : IEntityPacket
    {
        //AssetEntity
    }

}