namespace SDefence.Packet
{
    using UnityEngine;

    public class HitBattlePacket : IBattlePacket
    {
        public Vector2 NowPosition;
        //�ǵ��ǰ� �Ϲ��ǰ� �ı�
        public bool IsShieldHit;
        public bool IsDestroy;
    }
}