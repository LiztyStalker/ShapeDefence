namespace SDefence.Packet
{
    using Actor;
    using Data;
    using Enemy;
    using UnityEngine;

    public class AppearEnemyBattlePacket : IBattlePacket
    {
        //보스
        //특수
        //진보스
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
        //실드피격 일반피격 파괴
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