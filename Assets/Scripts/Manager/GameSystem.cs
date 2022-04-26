namespace SDefence.Manager
{
    using HQ;
    using Turret;
    using Packet;
    using Asset.Entity;
    using Utility.Statistics;
    using Utility.IO;
    using Actor;
    using Enemy;

    public class GameSystem : ISavable
    {
        private HQManager _hqMgr;
        private TurretManager _turretMgr;


        private AssetUsableEntity _asset;
        private StatisticsPackage _statistics;

        public static GameSystem Create() => new GameSystem();

        private GameSystem()
        {
            _hqMgr = HQManager.Create();
            _turretMgr = TurretManager.Create();

            _hqMgr.AddOnEntityPacketListener(OnEntityPacketEvent);
            _turretMgr.AddOnEntityPacketListener(OnEntityPacketEvent);

            _asset = AssetUsableEntity.Create();
            _statistics = StatisticsPackage.Create();
        }

        public void Initialize()
        {
            _hqMgr.Initialize();
            _turretMgr.Initialize();
        }

        public void CleanUp()
        {
            _hqMgr.RemoveOnEntityPacketListener(OnEntityPacketEvent);
            _turretMgr.RemoveOnEntityPacketListener(OnEntityPacketEvent);

            _asset.CleanUp();
            _statistics.CleanUp();

        }
        public void RefreshAll()
        {
            _hqMgr.Refresh();
            _turretMgr.Refresh(0);
        }

    
        public void OnBattlePacketEvent(IBattlePacket packet)
        {
            switch (packet)
            {
                case LevelWaveBattlePacket pk:
                    var value = _statistics.GetStatisticsValue<MaximumArriveLevelStatisticsData>();
                    if(pk.data.GetLevel() > value.Value)
                        _statistics.SetStatisticsData<MaximumArriveLevelStatisticsData>(pk.data.GetLevel());
                    break;
                case DestroyBattlePacket pk:


                    switch (pk.Actor)
                    {
                        case EnemyActor eActor:

                            switch (pk.TypeBattleAction)
                            {
                                case BattleManager.TYPE_BATTLE_ACTION.Lobby:
                                    _asset.Add(eActor.RewardAssetUsableData);
                                    break;
                                case BattleManager.TYPE_BATTLE_ACTION.Battle:
                                    AddStatisticsData(eActor.TypeEnemyStyle);
                                    break;
                            }
                            break;
                    }
                    break;
                case ClearBattlePacket pk:
                    _statistics.AddStatisticsData<ClearCountStatisticsData>();
                    break;
                case DefeatBattlePacket pk:
                    _statistics.AddStatisticsData<DefeatCountStatisticsData>();
                    break;
            }
        }

        private void AddStatisticsData(TYPE_ENEMY_STYLE typeEnemyStyle)
        {
            _statistics.AddStatisticsData<DestroyEnemyStatisticsData>();

            switch (typeEnemyStyle)
            {
                case TYPE_ENEMY_STYLE.MiddleBoss:
                    _statistics.AddStatisticsData<DestroyMiddleBossEnemyStatisticsData>();
                    goto case TYPE_ENEMY_STYLE.NormalBoss;
                case TYPE_ENEMY_STYLE.SpecialBoss:
                    _statistics.AddStatisticsData<DestroySpecialEnemyStatisticsData>();
                    goto case TYPE_ENEMY_STYLE.NormalBoss;
                case TYPE_ENEMY_STYLE.ThemeBoss:
                    _statistics.AddStatisticsData<DestroyThemeBossEnemyStatisticsData>();
                    goto case TYPE_ENEMY_STYLE.NormalBoss;
                case TYPE_ENEMY_STYLE.NormalBoss:
                    _statistics.AddStatisticsData<DestroyBossEnemyStatisticsData>();
                    break;
            }
        }

        public void OnCommandPacketEvent(ICommandPacket packet)
        {
            switch (packet)
            {
                case UpgradeCommandPacket pk:
                    // HQ / Turret                                       


                    switch (pk.TypeCmdKey)
                    {
                        case TYPE_COMMAND_KEY.HQ:
                            {
                                var assetData = _hqMgr.Upgrade(); //AssetUsableData
                                _asset.Subject(assetData);

                                // AddStatisticsData
                                _statistics.AddStatisticsData<UpgradeHQStatisticsData>();
                            }

                            break;
                        case TYPE_COMMAND_KEY.Turret:
                            {
                                var assetData = _turretMgr.Upgrade(pk.ParentIndex, pk.Index);
                                _asset.Subject(assetData);

                                // AddStatisticsData
                                _statistics.AddStatisticsData<UpgradeTurretStatisticsData>();
                            }
                            break;
                    }
                    break;


                case OpenTechCommandPacket pk:
                    // HQ / Turret
                    //OpenTechEntityPacket
                    break;
                case UpTechCommandPacket pk:
                    // HQ / Turret



                    // AddStatisticsData
                    switch (pk.TypeCmdKey)
                    {
                        case TYPE_COMMAND_KEY.HQ:
                            {
                                //Asset
                                _statistics.AddStatisticsData<UpTechHQStatisticsData>();
                            }
                            break;
                        case TYPE_COMMAND_KEY.Turret:
                            {
                                //Asset
                                _statistics.AddStatisticsData<UpTechTurretStatisticsData>();
                            }
                            break;
                    }
                    break;
                case OpenExpandCommandPacket pk:
                    // Turret OpenExpand
                    //OpenExpandEntityPacket
                    break;
                case ExpandCommandPacket pk:
                    // Turret Expand
                    break;
                case OpenDisassembleCommandPacket pk:
                    // Turret
                    //OpenDiassembleEntityPacket
                    break;
                case DisassembleCommandPacket pk:
                    // Turret

                    //AddStatisticsData - Turret
                    //Asset
                    _statistics.AddStatisticsData<DisassembleTurretStatisticsData>();
                    break;
                case RefreshCommandPacket pk:
                    // HQ / Turret
                    break;


                //Battle
                case RetryCommandPacket pk:
                    _asset.Add(pk.AssetEntity);
                    break;
                case ToLobbyCommandPacket pk:
                    //pk.AssetEntity
                    _asset.Add(pk.AssetEntity);
                    //Asset
                    break;
                case AdbToLobbyCommandPacket pk:
                    _asset.Add(pk.AssetEntity);
                    _asset.Add(pk.AssetEntity);
                    //Asset * 2
                    break;
                case NextLevelCommandPacket pk:
                    _asset.Add(pk.AssetEntity);
                    //Asset
                    break;
            }
        }

        #region ##### Listener #####

        private System.Action<IEntityPacket> _packetEvent;
        public void AddOnEntityPacketListener(System.Action<IEntityPacket> act) => _packetEvent += act;
        public void RemoveOnRefreshEntityPacketListener(System.Action<IEntityPacket> act) => _packetEvent -= act;
        private void OnEntityPacketEvent(IEntityPacket packet) => _packetEvent?.Invoke(packet);
        #endregion





        #region ##### Savable #####

        private readonly string SAVABLE_SAVETIME_KEY = "SaveTime";

        public string SavableKey() => typeof(GameSystem).Name;

        public SavableData GetSavableData()
        {
            var data = SavableData.Create();
            data.AddData(SAVABLE_SAVETIME_KEY, System.DateTime.UtcNow);
            data.AddData(_hqMgr.SavableKey(), _hqMgr.GetSavableData());
            data.AddData(_turretMgr.SavableKey(), _turretMgr.GetSavableData());
            //Date
            return data;
        }

        public void SetSavableData(SavableData data)
        {
            if (data != null)
            {
                var hqSavable = data.GetValue<SavableData>(_hqMgr.SavableKey());
                _hqMgr.SetSavableData(hqSavable);


                var turretSavable = data.GetValue<SavableData>(_turretMgr.SavableKey());
                _turretMgr.SetSavableData(turretSavable);


                var time = data.GetValue<System.DateTime>(SAVABLE_SAVETIME_KEY);
                var year = System.DateTime.UtcNow.Year - time.Year;
                var month = System.DateTime.UtcNow.Month - time.Month;
                var day = System.DateTime.UtcNow.Day - time.Day;

                //일 로그인 적용
                if (day != 0 || month != 0 || year != 0)
                    _statistics.AddStatisticsData<DailyLoginCountStatisticsData>();
            }
        }


        #endregion
    }
}