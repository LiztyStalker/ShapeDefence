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

            GUILayout.Label("���� ��Ŷ");

            if (GUILayout.Button("���� ����"))
            {
                var pk = new PlayBattlePacket();
                pk.data = _levelWaveData;
                pk.BossIcon = null;
                _uiGame.OnBattlePacketEvent(pk);
            }

            if (GUILayout.Button("���� Ŭ����"))
            {
                var pk = new ClearBattlePacket();
                _uiGame.OnBattlePacketEvent(pk);
            }

            if (GUILayout.Button("���� �й�"))
            {
                var pk = new DefeatBattlePacket();
                _uiGame.OnBattlePacketEvent(pk);
            }

            if (GUILayout.Button("���� ����"))
            {
                var pk = new AppearEnemyBattlePacket();
                pk.TypeEnemyStyle = Enemy.TYPE_ENEMY_STYLE.NormalBoss;
                _uiGame.OnBattlePacketEvent(pk);
            }

            if (GUILayout.Button("Ư�� �� ���� ����"))
            {
                var pk = new AppearEnemyBattlePacket();
                pk.TypeEnemyStyle = Enemy.TYPE_ENEMY_STYLE.SpecialBoss;
                _uiGame.OnBattlePacketEvent(pk);
            }

            if (GUILayout.Button("�׸� ���� ����"))
            {
                var pk = new AppearEnemyBattlePacket();
                pk.TypeEnemyStyle = Enemy.TYPE_ENEMY_STYLE.ThemeBoss;
                _uiGame.OnBattlePacketEvent(pk);
            }

            if (GUILayout.Button("���̺� ����"))
            {
                var pk = new NextWaveBattlePacket();
                pk.data = _levelWaveData;
                _uiGame.OnBattlePacketEvent(pk);
                _levelWaveData.IncreaseNumber();
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            GUILayout.Label("�κ� ��Ŷ");

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("HQ ���� �Ϲ�"))
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

            if (GUILayout.Button("HQ ���� - ����"))
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

            if (GUILayout.Button("HQ ���� - ��ũ"))
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

            if (GUILayout.Button("��ž ����"))
            {
                var pk = new TurretEntityPacket();
                _uiGame.OnEntityPacketEvent(pk);
            }

            if (GUILayout.Button("��ȭ ���� Test"))
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