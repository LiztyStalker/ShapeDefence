namespace SDefence.Packet
{
    using SDefence.Actor;

    public class HQBattlePacket : IBattlePacket
    {
        private HQActor _actor;
        public HQActor Actor => _actor;

        public void SetData(HQActor actor)
        {
            _actor = actor;
        }
    }
}