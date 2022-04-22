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
                case RefreshCommandPacket refPk:

                    switch (refPk.TypeCmdKey)
                    {
                        case TYPE_COMMAND_KEY.HQ:
                            OnHQRefreshEvent(false, false, false);
                            break;
                        case TYPE_COMMAND_KEY.Turret:
                            OnTurretOrbitRefreshEntity();
                            if (refPk.ParentIndex == 0)
                                OnTurretRefreshEvent(0, 0, false, false, false);
                            else
                                OnTurretArrayRefreshEvent(refPk.ParentIndex, 1, false, false, false, false);
                            break;
                    } 
                    break;
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

            GUILayout.Label("HQ ��Ŷ");

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("HQ ���� �Ϲ�"))
            {
                OnHQRefreshEvent(false, false, false);
            }

            if (GUILayout.Button("HQ ���� ����"))
            {
                OnHQRefreshEvent(true, false, false);
            }

            if (GUILayout.Button("HQ ��ũ / ��ȭ ����"))
            {
                OnHQRefreshEvent(false, false, true);
            }
            if (GUILayout.Button("HQ ���� / ��ȭ ����"))
            {
                OnHQRefreshEvent(false, true, true);
            }

            GUILayout.EndHorizontal();

            GUILayout.Label("��ž ��Ŷ");

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("�� ��ž ����"))
            {
                OnHQRefreshEvent(false, false, false);
            }
            if (GUILayout.Button("�� ��ž ���� ����"))
            {
                OnHQRefreshEvent(true, false, false);
            }
            if (GUILayout.Button("�� ��ž ��ũ / ��ȭ ����"))
            {
                OnHQRefreshEvent(false, false, true);
            }
            if (GUILayout.Button("�� ��ž ��ũ / ��ȭ ���"))
            {
                OnHQRefreshEvent(false, true, true);
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("�˵� ����"))
            {
                OnTurretOrbitRefreshEntity();
            }

            if (GUILayout.Button("�˵� ��ž ��ü ����"))
            {
                OnTurretArrayRefreshEvent(1, 1, false, false, false, false);
            }

            if (GUILayout.Button("�˵� ��ž ��ü ���� Ȯ��"))
            {
                OnTurretArrayRefreshEvent(1, 1, false, false, false, true);
            }


            GUILayout.BeginHorizontal();

            if (GUILayout.Button("�˵� ��ž ����"))
            {
                OnTurretRefreshEvent(1, 0, false, false, false);
            }


            if (GUILayout.Button("�˵� ��ž ���� ����"))
            {
                OnTurretRefreshEvent(1, 0, true, false, false);
            }

            if (GUILayout.Button("�˵� ��ž ��ũ / ��ȭ ����"))
            {
                OnTurretRefreshEvent(1, 0, false, false, true);
            }
            if (GUILayout.Button("�˵� ��ž ��ũ / ��ȭ ����"))
            {
                OnTurretRefreshEvent(1, 0, false, true, true);
            }

            GUILayout.EndHorizontal();



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


            if (GUILayout.Button("�������� ����"))
            {
                var pk = new RewardOfflineEntityPacket();
                _uiGame.OnEntityPacketEvent(pk);
            }


            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }


        private void OnHQRefreshEvent(bool isActiveUpgrade, bool isActiveUpTech, bool isFullUpgrade)
        {
            var raw = HQ.HQData.Create();
            var entity = HQ.Entity.HQEntity.Create();
            entity.SetData(raw);
            if (isFullUpgrade) entity.SetMaxUpgrade_Test();

            var pk = new HQEntityPacket();
            pk.Entity = entity;
            pk.IsActiveUpgrade = isActiveUpgrade;
            pk.IsActiveUpTech = isActiveUpTech;
            _uiGame.OnEntityPacketEvent(pk);
        }

        private void OnTurretOrbitRefreshEntity()
        {
            var pk = new TurretOrbitEntityPacket();
            pk.OrbitCount = 1;
            _uiGame.OnEntityPacketEvent(pk);
        }


        private void OnTurretRefreshEvent(int orbitIndex, int index, bool isUpgrade, bool isUpTech, bool isFullUpgrade)
        {
            var raw = Turret.TurretData.Create();
            var entity = Turret.Entity.TurretEntity.Create();
            entity.Initialize(raw, orbitIndex);
            if (isFullUpgrade) entity.SetMaxUpgrade_Test();

            var pk = new TurretEntityPacket();
            pk.Entity = entity;
            pk.OrbitIndex = entity.OrbitIndex;
            pk.Index = index;
            pk.IsActiveUpgrade = isUpgrade;
            pk.IsActiveUpTech = isUpTech;
            _uiGame.OnEntityPacketEvent(pk);
        }

        private void OnTurretArrayRefreshEvent(int orbitIndex, int count, bool isUpgrade, bool isUpTech, bool isFullUpgrade, bool isExpand)
        {
            var pks = new TurretArrayEntityPacket();
            pks.packets = new TurretEntityPacket[count];
            pks.IsExpand = isExpand;

            for (int i = 0; i < count; i++)
            {
                var raw = Turret.TurretData.Create();
                var entity = Turret.Entity.TurretEntity.Create();
                if (isFullUpgrade) entity.SetMaxUpgrade_Test();
                entity.Initialize(raw, orbitIndex);

                var pk = new TurretEntityPacket();
                pk.Entity = entity;
                pk.OrbitIndex = entity.OrbitIndex;
                pk.Index = i;
                pk.IsActiveUpgrade = isUpgrade;
                pk.IsActiveUpTech = isUpTech;

                pks.packets[i] = pk;
            }

            _uiGame.OnEntityPacketEvent(pks);
        }
    }
}
#endif