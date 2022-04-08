namespace SDefence.Packet
{
    using SDefence.Actor;

    public class ActorBattlePacket : IBattlePacket
    {
        public IActor Actor;
    }
}