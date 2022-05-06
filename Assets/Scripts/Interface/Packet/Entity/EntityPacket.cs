namespace SDefence.Packet
{
    using HQ.Entity;
    using Asset.Entity;
    using Turret.Entity;
    using Entity;
    using Tech;
    using Asset;
    using SDefence.Turret;

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
        public int TurretCapacity;
        public int TurretCount;
    }

    public class TurretExpandEntityPacket : IEntityPacket
    {
        public bool IsMaxExpand;
        public bool IsActivateExpand;
        public IAssetUsableData ExpandAssetData;
    }

    public class AssetEntityPacket : IEntityPacket
    {
        public AssetUsableEntity Entity;
    }


    public class OpenTechEntityPacket : IEntityPacket
    {       
        public TechPacketElement[] Elements;
        public int OrbitIndex;
        public int Index;
    }

    public class UpTechEntityPacket : IEntityPacket
    {
        public IEntity PastEntity;
        public IEntity NowEntity;
    }

    public class OpenDisassembleEntityPacket : IEntityPacket
    {
        public AssetUsableEntity AssetEntity;
        public int OrbitIndex;
        public int Index;
    }

    public class DisassembleEntityPacket : IEntityPacket
    {
        public IEntity NowEntity;
        public IEntity PastEntity;
    }

    public class OpenExpandTurretEntityPacket : IEntityPacket
    {
        public int OrbitIndex;
        public IAssetUsableData AssetData;
    }

    public class RewardOfflineEntityPacket : IEntityPacket
    {
        //AssetEntity
    }

}