namespace SDefence.Packet
{
    using SDefence.Actor;

    public class TurretBattlePacket : IBattlePacket
    {
        private TurretActor _actor;
        public TurretActor Actor => _actor;

        public void SetData(TurretActor actor)
        {
            _actor = actor;
        }
    }
}