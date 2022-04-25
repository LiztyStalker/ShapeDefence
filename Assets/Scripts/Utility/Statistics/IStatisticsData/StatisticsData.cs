namespace Utility.Statistics
{


    #region ##### Account #####
    public class DailyLoginCountStatisticsData : IStatisticsData { }

    #endregion



    #region ##### Entity #####

    public class UpgradeHQStatisticsData : IStatisticsData { }
    public class UpgradeTurretStatisticsData : IStatisticsData { }
    public class UpTechHQStatisticsData : IStatisticsData { }
    public class UpTechTurretStatisticsData : IStatisticsData { }
    public class DisassembleTurretStatisticsData : IStatisticsData { }
    #endregion



    #region ##### Battle #####

    public class MaximumArriveLevelStatisticsData : IStatisticsData { }
    public class DestroyEnemyStatisticsData : IStatisticsData { }
    public class DestroyBossEnemyStatisticsData : IStatisticsData { }
    public class DestroyMiddleBossEnemyStatisticsData : IStatisticsData { }
    public class DestroySpecialEnemyStatisticsData : IStatisticsData { }
    public class DestroyThemeBossEnemyStatisticsData : IStatisticsData { }
    public class ClearCountStatisticsData : IStatisticsData { }
    public class DefeatCountStatisticsData : IStatisticsData { }


    #endregion

    //
    //public abstract class AbstractStatisticsData 
    //{
    //    private string _key;
    //    public string Key { get => _key; set => _key = value; }
    //}


    //#region ##### Account #####
    //public class DailyLoginCountStatisticsData : AbstractStatisticsData, IStatisticsData { }

    //#endregion



    //#region ##### Entity #####

    //public class UpgradeHQStatisticsData : AbstractStatisticsData, IStatisticsData { }
    //public class UpgradeTurretStatisticsData : AbstractStatisticsData, IStatisticsData { }
    //public class UpTechHQStatisticsData : AbstractStatisticsData, IStatisticsData { }
    //public class UpTechTurretStatisticsData : AbstractStatisticsData, IStatisticsData { }
    //public class DisassembleTurretStatisticsData : AbstractStatisticsData, IStatisticsData { }
    //#endregion



    //#region ##### Battle #####

    //public class ArriveLevelStatisticsData : AbstractStatisticsData, IStatisticsData { }
    //public class DestroyEnemyStatisticsData : AbstractStatisticsData, IStatisticsData { }
    //public class DestroyBossEnemyStatisticsData : AbstractStatisticsData, IStatisticsData { }
    //public class DestroyMiddleBossEnemyStatisticsData : AbstractStatisticsData, IStatisticsData { }
    //public class DestroySpecialEnemyStatisticsData : AbstractStatisticsData, IStatisticsData { }
    //public class DestroyThemeBossEnemyStatisticsData : AbstractStatisticsData, IStatisticsData { }
    //public class ClearCountStatisticsData : AbstractStatisticsData, IStatisticsData { }
    //public class DefeatCountStatisticsData : AbstractStatisticsData, IStatisticsData { }


    //#endregion



#if UNITY_EDITOR
    public class TestStatisticsData : IStatisticsData { }
#endif

}