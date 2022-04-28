namespace SDefence.Packet
{
    using Actor;
    using Data;
    using Enemy;
    using Manager;
    using Asset.Entity;
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

    public class ClearBattlePacket : IBattlePacket 
    {
        public AssetUsableEntity AssetEntity;
    }
    public class DefeatBattlePacket : IBattlePacket 
    {
        public AssetUsableEntity AssetEntity;
    }

    public class DestroyBattlePacket : IBattlePacket
    {
        public IActor Actor;
        public BattleManager.TYPE_BATTLE_ACTION TypeBattleAction;
        public bool IsReward;
    }

    public class HitBattlePacket : IBattlePacket
    {
        public Vector2 NowPosition;
        //실드피격 일반피격 파괴
        public bool IsShieldHit;
        public bool IsDestroy;
    }

    public class LevelWaveBattlePacket : IBattlePacket
    {
        public LevelWaveData data;
    }

    public class PlayBattlePacket : IBattlePacket
    {
        public Sprite BossIcon;
        public LevelWaveData data;
    }

    public class AssetBattlePacket : IBattlePacket
    {
        public AssetUsableEntity AssetEntity;
    }
}