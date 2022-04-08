namespace SDefence.Packet
{
    using SDefence.Actor;

    public class DestroyBattlePacket : IBattlePacket
    {
        public IActor Actor;
    }
}