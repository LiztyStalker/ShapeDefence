#if UNITY_EDITOR
namespace SDefence.UI.Test
{
    using Packet;
    using Data;
    using Storage;
    using UnityEngine;
    using SDefence.Asset.Entity;
    using SDefence.Asset.Raw;
    using UtilityManager;

    public class UIGameTester : MonoBehaviour
    {
        private UIGame _uiGame;
        private LevelWaveData _levelWaveData;

        [SerializeField]
        private AudioClip _bgm;

        [SerializeField]
        private AudioClip _sfx;

        private AudioActor _bgmActor;

        private string _frameText;

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

            QualitySettings.vSyncCount = 0;
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
                case OpenDisassembleCommandPacket pk:
                    {
                        var raw = Turret.TurretData.Create();
                        var entity = Turret.Entity.TurretEntity.Create();
                        entity.Initialize(raw, pk.ParentIndex);

                        var techPK = new OpenDisassembleEntityPacket();
                        techPK.Entity = entity;
                        OnEntityPacketEvent(techPK);
                    }
                    break;
                case DisassembleCommandPacket pk:
                    {
                        var raw = Turret.TurretData.Create();
                        var entity = Turret.Entity.TurretEntity.Create();
                        entity.Initialize(raw, pk.ParentIndex);

                        var techPK = new DisassembleEntityPacket();
                        techPK.Entity = entity;
                        OnEntityPacketEvent(techPK);
                    }

                    break;
                case OpenTechCommandPacket pk:
                    switch (pk.TypeCmdKey)
                    {
                        case TYPE_COMMAND_KEY.HQ:
                            {
                                var raw = HQ.HQData.Create();
                                var entity = HQ.Entity.HQEntity.Create();
                                entity.SetData(raw);

                                var techPK = new OpenTechEntityPacket();
                                techPK.Entity = entity;
                                OnEntityPacketEvent(techPK);
                            }
                            break;
                        case TYPE_COMMAND_KEY.Turret:
                            {
                                var raw = Turret.TurretData.Create();
                                var entity = Turret.Entity.TurretEntity.Create();
                                entity.Initialize(raw, pk.ParentIndex);

                                var techPK = new OpenTechEntityPacket();
                                techPK.Entity = entity;
                                OnEntityPacketEvent(techPK);
                            }
                            break;
                    }
                    break;
                case UpTechCommandPacket pk:
                    switch (pk.TypeCmdKey)
                    {
                        case TYPE_COMMAND_KEY.HQ:
                            {
                                //AssetUsable 소비
                                //Key로 Data 찾기
                                var raw = HQ.HQData.Create();
                                var entity = HQ.Entity.HQEntity.Create();
                                entity.SetData(raw);

                                var techPK = new UpTechEntityPacket();
                                techPK.Entity = entity;
                                OnEntityPacketEvent(techPK);
                            }
                            break;
                        case TYPE_COMMAND_KEY.Turret:
                            {
                                var raw = Turret.TurretData.Create();
                                var entity = Turret.Entity.TurretEntity.Create();
                                entity.Initialize(raw, pk.ParentIndex);

                                var techPK = new UpTechEntityPacket();
                                techPK.Entity = entity;
                                OnEntityPacketEvent(techPK);
                            }
                            break;
                    }
                    break;
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
                    if(tabPacket.OrbitIndex == 0)
                    {
                        OnTurretRefreshEvent(0, 0, false, false, false);
                    }
                    else
                    {
                        OnTurretArrayRefreshEvent(tabPacket.OrbitIndex, 1, false, false, false, false);
                    }
                    break;
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(0, 0, 1920, 1080));

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();

            GUILayout.Label(_frameText);

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
                var pk = new LevelWaveBattlePacket();
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
                OnHQRefreshEvent(false, false, false);
            }

            if (GUILayout.Button("HQ 업글 가능"))
            {
                OnHQRefreshEvent(true, false, false);
            }

            if (GUILayout.Button("HQ 테크 / 재화 부족"))
            {
                OnHQRefreshEvent(false, false, true);
            }
            if (GUILayout.Button("HQ 갱신 / 재화 있음"))
            {
                OnHQRefreshEvent(false, true, true);
            }

            GUILayout.EndHorizontal();

            GUILayout.Label("포탑 패킷");

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("주 포탑 갱신"))
            {
                OnTurretRefreshEvent(0, 0, false, false, false);
            }
            if (GUILayout.Button("주 포탑 업글 가능"))
            {
                OnTurretRefreshEvent(0, 0, true, false, false);
            }
            if (GUILayout.Button("주 포탑 테크 / 재화 부족"))
            {
                OnTurretRefreshEvent(0, 0, false, false, true);
            }
            if (GUILayout.Button("주 포탑 테크 / 재화 충분"))
            {
                OnTurretRefreshEvent(0, 0, false, true, true);
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("궤도 갱신"))
            {
                OnTurretOrbitRefreshEntity();
            }

            if (GUILayout.Button("궤도 포탑 전체 갱신"))
            {
                OnTurretArrayRefreshEvent(1, 1, false, false, false, false);
            }

            if (GUILayout.Button("궤도 포탑 전체 갱신 확장"))
            {
                OnTurretArrayRefreshEvent(1, 1, false, false, false, true);
            }


            GUILayout.BeginHorizontal();

            if (GUILayout.Button("궤도 포탑 갱신"))
            {
                OnTurretRefreshEvent(1, 0, false, false, false);
            }


            if (GUILayout.Button("궤도 포탑 업글 가능"))
            {
                OnTurretRefreshEvent(1, 0, true, false, false);
            }

            if (GUILayout.Button("궤도 포탑 테크 / 재화 부족"))
            {
                OnTurretRefreshEvent(1, 0, false, false, true);
            }
            if (GUILayout.Button("궤도 포탑 테크 / 재화 있음"))
            {
                OnTurretRefreshEvent(1, 0, false, true, true);
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

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("BGM 재생"))
            {
                _bgmActor = AudioManager.Current.Activate(_bgm, AudioManager.TYPE_AUDIO.BGM, true);
            }

            if (GUILayout.Button("BGM 종료"))
            {
                if (_bgmActor != null)
                {
                    AudioManager.Current.Inactivate(_bgmActor);
                    _bgmActor = null;
                }
            }

            if (GUILayout.Button("SFX 재생"))
            {
                AudioManager.Current.Activate(_sfx, AudioManager.TYPE_AUDIO.SFX, false);
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void Update()
        {
            var frame = 1f / Time.deltaTime;
            _frameText = $"fps({frame}ms)";
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
            OnEntityPacketEvent(pk);
        }

        private void OnTurretOrbitRefreshEntity()
        {
            var pk = new TurretOrbitEntityPacket();
            pk.OrbitCount = 1;
            OnEntityPacketEvent(pk);
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
            OnEntityPacketEvent(pk);
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

            OnEntityPacketEvent(pks);
        }

        private void OnEntityPacketEvent(IEntityPacket packet) => _uiGame.OnEntityPacketEvent(packet);
    }
}
#endif