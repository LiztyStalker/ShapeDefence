namespace SDefence.Packet
{
    public class NextLevelCommandPacket : ICommandPacket {}
    public class RetryCommandPacket : ICommandPacket {}
    public class ToLobbyCommandPacket : ICommandPacket {}
    public class AdbToLobbyCommandPacket : ICommandPacket { }
    public class PlayBattleCommandPacket : ICommandPacket {}
    public class RewardOfflineCommandPacket : ICommandPacket 
    {
        public bool IsAdb = false;
    }



    public class UpgradeCommandPacket : ICommandPacket { }
    public class OpenTechCommandPacket : ICommandPacket { }
    public class UpTechCommandPacket : ICommandPacket { }
    public class OpenDisassembleCommandPacket : ICommandPacket { }
    public class DisassembleCommandPacket : ICommandPacket { }
    public class RefreshCommandPacket : ICommandPacket { }
    public class ExpandCommandPacket : ICommandPacket { }

    public class SettingsCommandPacket : ICommandPacket { }

    public class HelpCommandPacket : ICommandPacket { }
    public class CategoryCommandPacket : ICommandPacket 
    {
        public string Category;
    }

}