namespace SDefence.Packet
{


    //CommandKey
    //HQ
    //Turret

    public enum TYPE_COMMAND_KEY { 
        HQ, 
        Turret
    }


    public class NextLevelCommandPacket : ICommandPacket {}
    public class RetryCommandPacket : ICommandPacket {}
    public class ToLobbyCommandPacket : ICommandPacket {}
    public class AdbToLobbyCommandPacket : ICommandPacket { }
    public class PlayBattleCommandPacket : ICommandPacket {}
    public class RewardOfflineCommandPacket : ICommandPacket 
    {
        public bool IsAdb = false;
    }



    public class UpgradeCommandPacket : ICommandPacket 
    {
        public TYPE_COMMAND_KEY typeCmdKey;
        public int ParentIndex;
        public int Index;
    }

    public class OpenTechCommandPacket : ICommandPacket 
    {
        public TYPE_COMMAND_KEY typeCmdKey;
        public int ParentIndex;
        public int Index;
    }

    public class UpTechCommandPacket : ICommandPacket 
    {
    }

    public class OpenDisassembleCommandPacket : ICommandPacket 
    {
        public TYPE_COMMAND_KEY TypeCmdKey;
        public int ParentIndex;
        public int Index;
    }
    public class DisassembleCommandPacket : ICommandPacket { }
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
    }

    public class CategoryCommandPacket : ICommandPacket 
    {
        public string Category;
    }

}