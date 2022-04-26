

namespace SDefence.Packet
{
    using Asset;
    using Asset.Entity;

    //CommandKey
    //HQ
    //Turret

    public enum TYPE_COMMAND_KEY { 
        HQ, 
        Turret
    }


    public class NextLevelCommandPacket : ICommandPacket 
    {
        public AssetUsableEntity AssetEntity;
    }
    public class RetryCommandPacket : ICommandPacket 
    {
        public AssetUsableEntity AssetEntity;
    }
    public class ToLobbyCommandPacket : ICommandPacket 
    {
        public AssetUsableEntity AssetEntity;
    }
    public class AdbToLobbyCommandPacket : ICommandPacket 
    {
        public AssetUsableEntity AssetEntity;
    }
    public class PlayBattleCommandPacket : ICommandPacket { }
    public class RewardOfflineCommandPacket : ICommandPacket 
    {
        public bool IsAdb = false;
    }



    public class UpgradeCommandPacket : ICommandPacket 
    {
        public TYPE_COMMAND_KEY TypeCmdKey;
        public int ParentIndex;
        public int Index;
    }

    public class OpenTechCommandPacket : ICommandPacket 
    {
        public TYPE_COMMAND_KEY TypeCmdKey;
        public int ParentIndex;
        public int Index;
    }

    public class UpTechCommandPacket : ICommandPacket 
    {
        public TYPE_COMMAND_KEY TypeCmdKey;
        public string Key;
        public int ParentIndex;
        public int Index;
        public IAssetUsableData AssetUsableData;
    }

    public class OpenDisassembleCommandPacket : ICommandPacket 
    {
        public TYPE_COMMAND_KEY TypeCmdKey;
        public int ParentIndex;
        public int Index;
    }
    public class DisassembleCommandPacket : ICommandPacket 
    {
        public TYPE_COMMAND_KEY TypeCmdKey;
        public int ParentIndex;
        public int Index;
        public IAssetUsableData AssetUsableData;
    }
    public class RefreshCommandPacket : ICommandPacket 
    {
        public TYPE_COMMAND_KEY TypeCmdKey;
        public int ParentIndex;
        public int Index;
    }
    public class OpenExpandCommandPacket : ICommandPacket { }
    public class ExpandCommandPacket : ICommandPacket { }

    public class SettingsCommandPacket : ICommandPacket { }

    public class TabCommandPacket : ICommandPacket
    {
        public int OrbitIndex;
    }

    public class HelpCommandPacket : ICommandPacket 
    {
        public TYPE_COMMAND_KEY TypeCmdKey;
    }

    public class CategoryCommandPacket : ICommandPacket 
    {
        public string Category;
    }

}