namespace SDefence.Packet
{
    using UnityEngine;

    public class HitBattlePacket : IBattlePacket
    {
        public Vector2 NowPosition;
        //실드피격 일반피격 파괴
        public bool IsShieldHit;
        public bool IsDestroy;
    }
}