#if UNITY_EDITOR
namespace SDefence.UI.Test
{
    using Packet;
    using Data;
    using Storage;
    using UnityEngine;
    using SDefence.Asset.Entity;
    using SDefence.Asset.Raw;

    public class UIGameTester : MonoBehaviour
    {
        private UIGame _uiGame;
        private LevelWaveData _levelWaveData;

        void Start()
        {
            _uiGame = FindObjectOfType<UIGame>(true);
            if (_uiGame == null)
            {
                var obj = DataStorage.Instance.GetDataOrNull<GameObject>("UI@Game");
                _uiGame = obj.GetComponent<UIGame>();
            }

            _uiGame.Initialize();
            _uiGame.AddOnCommandPacketListener(OnCommandPacketEvent);


            _levelWaveData = new LevelWaveData();
        }

        private void OnDestroy()
        {
            _uiGame.RemoveOnCommandPacketListener(OnCommandPacketEvent);
            _uiGame.CleanUp();
        }

        private void OnCommandPacketEvent(ICommandPacket packet)
        {
            Debug.Log($"Packet {packet.GetType().Name}");

            switch (packet)
            {
                case TabCommandPacket tabPacket:
                    if(tabPacket.Index == 0)
                    {
                        var raw = Turret.TurretData.Create();
                        var entity = Turret.Entity.TurretEntity.Create();
                        entity.Initialize(raw, 0);
                        var pk = new TurretEntityPacket();
                        pk.Entity = entity;
                        pk.OrbitIndex = entity.OrbitIndex;
                        pk.Index = 0;
                        _uiGame.OnEntityPacketEvent(pk);
                    }
                    else
                    {
                        var raw = Turret.TurretData.Create();
                        var entity = Turret.Entity.TurretEntity.Create();
                        entity.Initialize(raw, 1);

                        var pk = new TurretEntityPacket();
                        pk.Entity = entity;
                        pk.OrbitIndex = entity.OrbitIndex;
                        pk.Index = 0;

                        var pks = new TurretArrayEntityPacket();
                        pks.packets = new TurretEntityPacket[1];
                        pks.packets[0] = pk;
                        pks.IsExpand = false;

                        _uiGame.OnEntityPacketEvent(pks);
                    }
                    break;
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(0, 0, 1920, 1080));

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();

            GUILayout.Label("전투 패킷");

            if (GUILayout.Button("전투 시작"))
            {
                var pk = new PlayBattlePacket();
                pk.data = _levelWaveData;
                pk.BossIcon = null;
                _uiGame.OnBattlePacketEvent(pk);
            }

            if (GUILayout.Button("전투 클리어"))
            {
                var pk = new ClearBattlePacket();
                _uiGame.OnBattlePacketEvent(pk);
            }

            if (GUILayout.Button("전투 패배"))
            {
                var pk = new DefeatBattlePacket();
                _uiGame.OnBattlePacketEvent(pk);
            }

            if (GUILayout.Button("보스 등장"))
            {
                var pk = new AppearEnemyBattlePacket();
                pk.TypeEnemyStyle = Enemy.TYPE_ENEMY_STYLE.NormalBoss;
                _uiGame.OnBattlePacketEvent(pk);
            }

            if (GUILayout.Button("특수 적 보스 등장"))
            {
                var pk = new AppearEnemyBattlePacket();
                pk.TypeEnemyStyle = Enemy.TYPE_ENEMY_STYLE.SpecialBoss;
                _uiGame.OnBattlePacketEvent(pk);
            }

            if (GUILayout.Button("테마 보스 등장"))
            {
                var pk = new AppearEnemyBattlePacket();
                pk.TypeEnemyStyle = Enemy.TYPE_ENEMY_STYLE.ThemeBoss;
                _uiGame.OnBattlePacketEvent(pk);
            }

            if (GUILayout.Button("웨이브 진행"))
            {
                var pk = new NextWaveBattlePacket();
                pk.data = _levelWaveData;
                _uiGame.OnBattlePacketEvent(pk);
                _levelWaveData.IncreaseNumber();
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            GUILayout.Label("로비 패킷");

            GUILayout.Label("HQ 패킷");

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("HQ 갱신 일반"))
            {
                var raw = HQ.HQData.Create();
                var entity = HQ.Entity.HQEntity.Create();
                entity.SetData(raw);

                var pk = new HQEntityPacket();
                pk.Entity = entity;
                pk.IsActiveUpgrade = false;
                pk.IsActiveUpTech = false;
                _uiGame.OnEntityPacketEvent(pk);
            }

            if (GUILayout.Button("HQ 갱신 - 업글"))
            {
                var raw = HQ.HQData.Create();
                var entity = HQ.Entity.HQEntity.Create();
                entity.SetData(raw);

                var pk = new HQEntityPacket();
                pk.Entity = entity;
                pk.IsActiveUpgrade = true;
                pk.IsActiveUpTech = false;
                _uiGame.OnEntityPacketEvent(pk);
            }

            if (GUILayout.Button("HQ 갱신 - 테크"))
            {
                var raw = HQ.HQData.Create();
                var entity = HQ.Entity.HQEntity.Create();
                entity.SetData(raw);
                entity.SetMaxUpgrade_Test();

                var pk = new HQEntityPacket();
                pk.Entity = entity;
                pk.IsActiveUpgrade = false;
                pk.IsActiveUpTech = true;
                _uiGame.OnEntityPacketEvent(pk);
            }

            GUILayout.EndHorizontal();

            GUILayout.Label("포탑 패킷");

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("주 포탑 갱신"))
            {
                var raw = Turret.TurretData.Create();
                var entity = Turret.Entity.TurretEntity.Create();
                entity.Initialize(raw, 0);
                var pk = new TurretEntityPacket();
                pk.Entity = entity;
                pk.OrbitIndex = entity.OrbitIndex;
                pk.Index = 0;
                pk.IsActiveUpgrade = false;
                pk.IsActiveUpTech = false;
                _uiGame.OnEntityPacketEvent(pk);
            }

            if (GUILayout.Button("주 포탑 업글"))
            {
                var raw = Turret.TurretData.Create();
                var entity = Turret.Entity.TurretEntity.Create();
                entity.Initialize(raw, 0);
                var pk = new TurretEntityPacket();
                pk.Entity = entity;
                pk.OrbitIndex = entity.OrbitIndex;
                pk.Index = 0;
                pk.IsActiveUpgrade = true;
                pk.IsActiveUpTech = false;
                _uiGame.OnEntityPacketEvent(pk);
            }

            if (GUILayout.Button("주 포탑 테크"))
            {
                var raw = Turret.TurretData.Create();
                var entity = Turret.Entity.TurretEntity.Create();
                entity.Initialize(raw, 0);
                entity.SetMaxUpgrade_Test();
                var pk = new TurretEntityPacket();
                pk.Entity = entity;
                pk.OrbitIndex = entity.OrbitIndex;
                pk.Index = 0;
                pk.IsActiveUpgrade = false;
                pk.IsActiveUpTech = true;
                _uiGame.OnEntityPacketEvent(pk);
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("궤도 갱신"))
            {
                var pk = new TurretOrbitEntityPacket();
                pk.OrbitCount = 1;
                _uiGame.OnEntityPacketEvent(pk);
            }

            if (GUILayout.Button("궤도 포탑 전체 갱신"))
            {
                var raw = Turret.TurretData.Create();
                var entity = Turret.Entity.TurretEntity.Create();
                entity.Initialize(raw, 1);
                var pk = new TurretEntityPacket();
                pk.Entity = entity;
                pk.OrbitIndex = entity.OrbitIndex;
                pk.Index = 0;

                var pks = new TurretArrayEntityPacket();
                pks.packets = new TurretEntityPacket[1];
                pks.packets[0] = pk;
                pks.IsExpand = false;

                _uiGame.OnEntityPacketEvent(pks);
            }

            if (GUILayout.Button("궤도 포탑 전체 갱신 확장"))
            {

                var raw = Turret.TurretData.Create();
                var entity = Turret.Entity.TurretEntity.Create();
                entity.Initialize(raw, 1);
                var pk = new TurretEntityPacket();
                pk.Entity = entity;
                pk.OrbitIndex = entity.OrbitIndex;
                pk.Index = 0;

                var pks = new TurretArrayEntityPacket();
                pks.packets = new TurretEntityPacket[1];
                pks.packets[0] = pk;
                pks.IsExpand = true;
                _uiGame.OnEntityPacketEvent(pks);
            }


            GUILayout.BeginHorizontal();

            if (GUILayout.Button("궤도 포탑 갱신"))
            {
                var raw = Turret.TurretData.Create();
                var entity = Turret.Entity.TurretEntity.Create();
                entity.Initialize(raw, 1);
                var pk = new TurretEntityPacket();
                pk.Entity = entity;
                pk.OrbitIndex = entity.OrbitIndex;
                pk.Index = 0;
                _uiGame.OnEntityPacketEvent(pk);
            }


            if (GUILayout.Button("궤도 포탑 업글"))
            {
                var raw = Turret.TurretData.Create();
                var entity = Turret.Entity.TurretEntity.Create();
                entity.Initialize(raw, 1);
                var pk = new TurretEntityPacket();
                pk.Entity = entity;
                pk.OrbitIndex = entity.OrbitIndex;
                pk.Index = 0;
                pk.IsActiveUpTech = false;
                pk.IsActiveUpgrade = true;
                _uiGame.OnEntityPacketEvent(pk);
            }

            if (GUILayout.Button("궤도 포탑 테크"))
            {
                var raw = Turret.TurretData.Create();
                var entity = Turret.Entity.TurretEntity.Create();
                entity.Initialize(raw, 1);
                entity.SetMaxUpgrade_Test();
                var pk = new TurretEntityPacket();
                pk.Entity = entity;
                pk.OrbitIndex = entity.OrbitIndex;
                pk.Index = 0;
                pk.IsActiveUpTech = true;
                pk.IsActiveUpgrade = false;
                _uiGame.OnEntityPacketEvent(pk);
            }

            GUILayout.EndHorizontal();



            if (GUILayout.Button("재화 갱신 Test"))
            {
                var raw = AssetRawData.Create();
                var usable = raw.GetUsableData();

                var assetEntity = AssetUsableEntity.Create();
                assetEntity.Set(usable);

                var pk = new AssetEntityPacket();
                pk.Entity = assetEntity;
                _uiGame.OnEntityPacketEvent(pk);
            }


            if (GUILayout.Button("오프라인 보상"))
            {
                var pk = new RewardOfflineEntityPacket();
                _uiGame.OnEntityPacketEvent(pk);
            }


            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }


    }
}
#endif