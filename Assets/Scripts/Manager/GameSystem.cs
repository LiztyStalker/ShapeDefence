namespace SDefence.Manager
{
    using HQ;
    using Turret;
    using Packet;
    using Asset.Entity;
    using Utility.Statistics;
    using Utility.IO;
    using Actor;
    using SDefence.Enemy;

    public class GameSystem
    {
        private HQManager _hqMgr;
        private TurretManager _turretMgr;


        private AssetUsableEntity _asset;
        private StatisticsPackage _statistics;

        private SavableEntity _savableEntity;

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

        public void Save()
        {

        }

        public void Load()
        {
            //일 로그인 적용
            _statistics.AddStatisticsData<DailyLoginCountStatisticsData>();
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
                    if (pk.Actor is EnemyActor)
                    {
                        var eActor = (EnemyActor)pk.Actor;
                        AddStatisticsData(eActor.TypeEnemyStyle);
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
                                _statistics.AddStatisticsData<UpTechHQStatisticsData>();
                            }
                            break;
                        case TYPE_COMMAND_KEY.Turret:
                            {
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
                    _statistics.AddStatisticsData<DisassembleTurretStatisticsData>();
                    break;
                case RefreshCommandPacket pk:
                    // HQ / Turret
                    break;
            }
        }

        #region ##### Listener #####

        private System.Action<IEntityPacket> _packetEvent;
        public void AddOnRefreshEntityPacketListener(System.Action<IEntityPacket> act) => _packetEvent += act;
        public void RemoveOnRefreshEntityPacketListener(System.Action<IEntityPacket> act) => _packetEvent -= act;
        private void OnEntityPacketEvent(IEntityPacket packet) => _packetEvent?.Invoke(packet);

        #endregion
    }
}