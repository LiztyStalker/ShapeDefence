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

        private void OnCommandPacketEvent(ICommandPacket pk)
        {
            Debug.Log($"Packet {pk.GetType().Name}");
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

            if (GUILayout.Button("HQ 갱신"))
            {
                var pk = new HQEntityPacket();
                _uiGame.OnEntityPacketEvent(pk);
            }

            if (GUILayout.Button("포탑 갱신"))
            {
                var pk = new TurretEntityPacket();
                _uiGame.OnEntityPacketEvent(pk);
            }

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


            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
    }
}
#endif