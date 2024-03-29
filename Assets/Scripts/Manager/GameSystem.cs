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
    using UnityEngine;
    using System.Collections.Generic;

    public class GameSystem : ISavable
    {
        private HQManager _hqMgr;
        private TurretManager _turretMgr;


        private AssetUsableEntity _assetEntity;
        private StatisticsPackage _statistics;

        private Stack<ICommandPacket> _lastCmdPacketStack;

        public static GameSystem Create() => new GameSystem();

        private GameSystem()
        {
            _lastCmdPacketStack = new Stack<ICommandPacket>();

            _hqMgr = HQManager.Create();
            _turretMgr = TurretManager.Create();

            _hqMgr.AddOnEntityPacketListener(OnEntityPacketEvent);
            _turretMgr.AddOnEntityPacketListener(OnEntityPacketEvent);

            _assetEntity = AssetUsableEntity.Create();
            _assetEntity.SetOnAssetEntityListener(OnAssetEntityPacketEvent);
            _statistics = StatisticsPackage.Create();
        }

        public void Initialize()
        {
            _hqMgr.Initialize();
            _turretMgr.Initialize();           
        }

        public void CleanUp()
        {
            _lastCmdPacketStack.Clear();

            _hqMgr.RemoveOnEntityPacketListener(OnEntityPacketEvent);
            _turretMgr.RemoveOnEntityPacketListener(OnEntityPacketEvent);

            _assetEntity.CleanUp();
            _statistics.CleanUp();

        }
        public void RefreshAll()
        {
            _hqMgr.Refresh();
            _turretMgr.RefreshAll();
            _assetEntity.Refresh();
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
                                    if(pk.IsReward) _assetEntity.Add(eActor.RewardAssetUsableData);
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
            //리팩토링 필요
            switch (packet)
            {
#if UNITY_EDITOR
                case TestAssetCommandPacket pk:
                    _assetEntity.Add(pk.AssetData);
                    break;
#endif


                case UpgradeCommandPacket pk:
                    // HQ / Turret                                       


                    switch (pk.TypeCmdKey)
                    {
                        case TYPE_COMMAND_KEY.HQ:
                            {
                                var assetData = _hqMgr.Upgrade(); //AssetUsableData
                                _assetEntity.Subject(assetData);

                                // AddStatisticsData
                                _statistics.AddStatisticsData<UpgradeHQStatisticsData>();
                            }

                            break;
                        case TYPE_COMMAND_KEY.Turret:
                            {
                                var assetData = _turretMgr.Upgrade(pk.ParentIndex, pk.Index);
                                _assetEntity.Subject(assetData);

                                // AddStatisticsData
                                _statistics.AddStatisticsData<UpgradeTurretStatisticsData>();
                            }
                            break;
                    }
                    break;


                case OpenTechCommandPacket pk:
                    // HQ / Turret
                    //OpenTechEntityPacket
                    //lastCommandPacket
                    switch (pk.TypeCmdKey)
                    {
                        case TYPE_COMMAND_KEY.HQ:
                            _hqMgr.OnOpenTechCommandPacketEvent();
                            break;                            
                        case TYPE_COMMAND_KEY.Turret:
                            _turretMgr.OnOpenTechCommandPacketEvent(pk.ParentIndex, pk.Index);
                            break;
                    }

                    CommandPush(pk);

                    break;
                case UpTechCommandPacket pk:
                    // HQ / Turret
                    // AddStatisticsData
                    switch (pk.TypeCmdKey)
                    {
                        case TYPE_COMMAND_KEY.HQ:
                            //Asset
                            if (_hqMgr.UpTech(pk.Key))
                            {
                                _assetEntity.Subject(pk.AssetUsableData);
                                _statistics.AddStatisticsData<UpTechHQStatisticsData>();
                            }
#if UNITY_EDITOR
                            else
                            {
                                UnityEngine.Debug.LogWarning($"{pk.Key} UpTech 실패");
                            }
#endif
                            break;

                            //Turret Orbit Expand
                            //_turretMgr.ExpandOrbit();
                        case TYPE_COMMAND_KEY.Turret:
                            if (_turretMgr.UpTech(pk.ParentIndex, pk.Index, pk.Key, pk.AssetUsableData))
                            {
                                _assetEntity.Subject(pk.AssetUsableData);
                                _statistics.AddStatisticsData<UpTechTurretStatisticsData>();
                            }
#if UNITY_EDITOR
                            else
                            {
                                UnityEngine.Debug.LogWarning($"{pk.Key} UpTech 실패");
                            }
#endif
                            _statistics.AddStatisticsData<UpTechTurretStatisticsData>();
                            break;
                    }
                    break;
                case OpenExpandCommandPacket pk:
                    // Turret OpenExpand
                    _turretMgr.OnOpenExpandTurretEntityPacketEvent(pk.OrbitIndex);
                    break;
                case ExpandCommandPacket pk:
                    {
                        // Turret Expand
                        var assetData = _turretMgr.ExpandTurret(pk.OrbitIndex);
                        _assetEntity.Subject(assetData);
                    }
                    break;
                case OpenDisassembleCommandPacket pk:
                    // Turret
                    //OpenDiassembleEntityPacket
                    _turretMgr.OnOpenDisassembleTurretEntityPacket(pk.ParentIndex, pk.Index);
                    break;
                case DisassembleCommandPacket pk:
                    // Turret
                    _assetEntity.Add(pk.AssetEntity);
                    _turretMgr.OnDisassembleTurretEntityPacket(pk.ParentIndex, pk.Index);
                    //AddStatisticsData - Turret
                    //Asset
                    _statistics.AddStatisticsData<DisassembleTurretStatisticsData>();
                    break;
                //case TabCommandPacket pk:
                //    _turretMgr.Refresh(pk.OrbitIndex);
                //    break;
                case RefreshCommandPacket pk:
                    // HQ / Turret
                    switch (pk.TypeCmdKey)
                    {
                        case TYPE_COMMAND_KEY.HQ:
                            _hqMgr.Refresh();
                            break;
                        case TYPE_COMMAND_KEY.Turret:
                            _turretMgr.Refresh(pk.ParentIndex);
                            break;
                    }
                    //기억
                    CommandPush(pk);

                    break;
                case ClosedUICommandPacket pk:
                    CommandPop();
                    break;
                //Battle
                case RetryCommandPacket pk:
                    _assetEntity.Add(pk.AssetEntity);
                    break;
                case ToLobbyCommandPacket pk:
                    //pk.AssetEntity
                    _assetEntity.Add(pk.AssetEntity);
                    //Asset
                    break;
                case AdsResultCommandPacket pk:
                    if (pk.Rewarded)
                    {
                        _assetEntity.Add(pk.AssetEntity);
                        _assetEntity.Add(pk.AssetEntity);
                    }
                    break;
                case NextLevelCommandPacket pk:
                    _assetEntity.Add(pk.AssetEntity);
                    //Asset
                    break;
            }
        }

        private void CommandPush(ICommandPacket cmdPacket)
        {
            if (_lastCmdPacketStack.Count > 0)
            {
                //Refresh Command
                if(_lastCmdPacketStack.Peek().GetType() == cmdPacket.GetType())
                {
                    CommandPop();
                }
            }
            _lastCmdPacketStack.Push(cmdPacket);
        }
        private void CommandPop()
        {
            if (_lastCmdPacketStack.Count > 0)
                _lastCmdPacketStack.Pop();
        }

        private void OnAssetEntityPacketEvent(AssetUsableEntity assetEntity)
        {
            var packet = new AssetEntityPacket();
            packet.Entity = assetEntity;
            OnEntityPacketEvent(packet);
        }

        #region ##### Listener #####

        private System.Action<IEntityPacket> _entityEvent;
        public void AddOnEntityPacketListener(System.Action<IEntityPacket> act) => _entityEvent += act;
        public void RemoveOnEntityPacketListener(System.Action<IEntityPacket> act) => _entityEvent -= act;
        private void OnEntityPacketEvent(IEntityPacket packet)
        {
            switch (packet)
            {
                case AssetEntityPacket pk:
                    if(_lastCmdPacketStack.Count > 0)
                    {
                        var cmdPacket = _lastCmdPacketStack.Peek();
                        switch (cmdPacket)
                        {
                            //AssetEntityPacket 진행시 RefreshCommandPacket의 기억한 값과 같이 출력
                            case RefreshCommandPacket refPacket:
                                switch (refPacket.TypeCmdKey)
                                {
                                    case TYPE_COMMAND_KEY.HQ:
                                        _hqMgr.Refresh();
                                        break;
                                    case TYPE_COMMAND_KEY.Turret:
                                        _turretMgr.Refresh(refPacket.ParentIndex);
                                        break;
                                }
                                break;
                            case OpenTechCommandPacket openTechPacket:
                                switch (openTechPacket.TypeCmdKey)
                                {
                                    case TYPE_COMMAND_KEY.HQ:
                                        _hqMgr.OnOpenTechCommandPacketEvent();
                                        break;
                                    case TYPE_COMMAND_KEY.Turret:
                                        _turretMgr.OnOpenTechCommandPacketEvent(openTechPacket.ParentIndex, openTechPacket.Index);
                                        break;
                                }
                                break;
                        }
                    }
                    //없으면 Asset만 진행
                    break;
                case HQEntityPacket pk:
                    if (!pk.IsActiveUpTech)
                    {
                        var assetData = pk.Entity.GetUpgradeData();
                        pk.IsActiveUpgrade = (_assetEntity.Compare(assetData) <= 0);
                    }

                    _turretMgr.SetOrbitCount(pk.Entity.TechLevel - 1, pk.Entity.TurretCount);

                    break;
                case TurretArrayEntityPacket pk:

                    //Upgrade
                    for(int i = 0; i < pk.packets.Length; i++)
                    {
                        //아래와 중첩
                        if (!pk.packets[i].IsActiveUpTech)
                        {
                            var assetData = pk.packets[i].Entity.GetUpgradeAssetUsableData();
                            pk.packets[i].IsActiveUpgrade = (_assetEntity.Compare(assetData) <= 0);
                        }
                    }
                    break;
                case TurretExpandEntityPacket pk:
                    ///Expand
                    if (!pk.IsMaxExpand)
                    {
                        pk.IsActivateExpand = (_assetEntity.Compare(pk.ExpandAssetData) <= 0);
                    }
                    break;
                case TurretEntityPacket pk:
                    //위와 중첩
                    if (!pk.IsActiveUpTech)
                    {
                        var assetData = pk.Entity.GetUpgradeAssetUsableData();
                        pk.IsActiveUpgrade = (_assetEntity.Compare(assetData) <= 0);
                    }
                    break;
                case OpenTechEntityPacket pk:
                    for(int i = 0; i < pk.Elements.Length; i++)
                    {
                        var element = pk.Elements[i];
                        element.IsActiveTech = (_assetEntity.Compare(element.Element.GetUsableData()) <= 0);
                        pk.Elements[i] = element;
                    }
                    break;
                    
            }
            _entityEvent?.Invoke(packet);
        }
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
            data.AddData(_assetEntity.SavableKey(), _assetEntity.GetSavableData());
            data.AddData(_statistics.SavableKey(), _statistics.GetSavableData());
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

                var assetSavable = data.GetValue<SavableData>(_assetEntity.SavableKey());
                _assetEntity.SetSavableData(assetSavable);

                var statisticsSavable = data.GetValue<SavableData>(_statistics.SavableKey());
                _statistics.SetSavableData(statisticsSavable);

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