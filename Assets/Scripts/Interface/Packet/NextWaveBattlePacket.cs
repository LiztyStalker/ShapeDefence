namespace SDefence.Packet
{
    using SDefence.Data;
    public class NextWaveBattlePacket : IBattlePacket
    {
        public LevelWaveData data;
    }
}