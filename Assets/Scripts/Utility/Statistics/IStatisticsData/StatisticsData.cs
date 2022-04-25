namespace Utility.Statistics
{
    public class AccumulativelyGoldGetAssetStatisticsData : IStatisticsData { }
    public class AccumulativelyMeteoriteGetAssetStatisticsData : IStatisticsData { }
    public class AccumulativelyResearchGetAssetStatisticsData : IStatisticsData { }
    public class AccumulativelyResourceGetAssetStatisticsData : IStatisticsData { }


    public class AccumulativelyGoldUsedAssetStatisticsData : IStatisticsData { }
    public class AccumulativelyMeteoriteUsedAssetStatisticsData : IStatisticsData { }
    public class AccumulativelyResourceUsedAssetStatisticsData : IStatisticsData { }
    public class AccumulativelyResearchUsedAssetStatisticsData : IStatisticsData { }


    public class ArrivedWaveStatisticsData : IStatisticsData { }
    public class ArrivedLevelStatisticsData : IStatisticsData { }
    public class MaxArrivedLevelStatisticsData : IStatisticsData { }
    public class Lv1ArrivedLevelStatisticsData : IStatisticsData { }



    #region ##### Unit & Workshop #####


    public class CreateUnitStatisticsData : IStatisticsData { }
#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
    public class TestCreateUnitStatisticsData : IStatisticsData { }
#endif

    public class DestroyUnitStatisticsData : IStatisticsData { }
#if UNITY_EDITOR || UNITY_INCLUDE_TEST
    public class TestDestroyUnitStatisticsData : IStatisticsData { }
#endif
    public class UpgradeUnitStatisticsData : IStatisticsData { }
#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
    public class TestUpgradeUnitStatisticsData : IStatisticsData { }
#endif

    public class ExpandWorkshopLineStatisticsData : IStatisticsData { }

    #endregion


    #region ##### Enemy #####
    public class DestroyEnemyStatisticsData : IStatisticsData { }
    public class DestroyBossStatisticsData : IStatisticsData { }
    public class DestroyThemeBossStatisticsData : IStatisticsData { }
#if UNITY_EDITOR || UNITY_INCLUDE_TEST
    public class TestDestroyEnemyStatisticsData : IStatisticsData { }
#endif

    #endregion


    public class SuccessResearchStatisticsData : IStatisticsData { }




    public class TechUnitStatisticsData : IStatisticsData { }
#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
    public class TestTechUnitStatisticsData : IStatisticsData { }
#endif

    public class UpgradeSmithyStatisticsData : IStatisticsData { }
    public class TechSmithyStatisticsData : IStatisticsData { }
    public class UpgradeVillageStatisticsData : IStatisticsData { }
    public class TechVillageStatisticsData : IStatisticsData { }
    public class UpgradeMineStatisticsData : IStatisticsData { }
    public class TechMineStatisticsData : IStatisticsData { }
    public class ExpandMineStatisticsData : IStatisticsData { }



    public class AchievedDailyStatisticsData : IStatisticsData { }
    public class AchievedWeeklyStatisticsData : IStatisticsData { }
    public class AchievedChallengeStatisticsData : IStatisticsData { }
    public class AchievedGoalStatisticsData : IStatisticsData { }





#if UNITY_EDITOR
    public class TestStatisticsData : IStatisticsData { }
#endif

}