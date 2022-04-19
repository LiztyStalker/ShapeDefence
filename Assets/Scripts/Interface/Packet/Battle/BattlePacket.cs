namespace SDefence.Packet
{
    using Actor;
    using Data;
    using Enemy;
    using UnityEngine;

    public class AppearEnemyBattlePacket : IBattlePacket
    {
        //����
        //Ư��
        //������
        public TYPE_ENEMY_STYLE TypeEnemyStyle;
    }

    public class ActorBattlePacket : IBattlePacket
    {
        public IActor Actor;
    }

    public class ClearBattlePacket : IBattlePacket { }
    public class DefeatBattlePacket : IBattlePacket { }

    public class DestroyBattlePacket : IBattlePacket
    {
        public IActor Actor;
    }

    public class HitBattlePacket : IBattlePacket
    {
        public Vector2 NowPosition;
        //�ǵ��ǰ� �Ϲ��ǰ� �ı�
        public bool IsShieldHit;
        public bool IsDestroy;
    }

    public class NextWaveBattlePacket : IBattlePacket
    {
        public LevelWaveData data;
    }

#if UNITY_EDITOR

    public class PlayBattlePacket : IBattlePacket
    {
    }
#endif
}