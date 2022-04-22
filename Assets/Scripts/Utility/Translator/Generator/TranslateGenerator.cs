#if UNITY_EDITOR
namespace UtilityManager
{
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    using Utility.Generator;

    public class TranslateGenerator
    {
        private readonly static string PATH = "Assets/TextAssets";
        private readonly static string _bundleName = "textassets";
        private readonly static string _sheetKey = "16Ps885lj_8ZSY6Gv_WQgehpCxusaz2l9J50JBXKq2DY";

        private readonly static string _wsUnitKey = "Unit_Data_Tr";
        private readonly static string _wsEnemyKey = "Enemy_Data_Tr";
        private readonly static string _wsSmithyKey = "Smithy_Data_Tr";
        private readonly static string _wsVillageKey = "Village_Data_Tr";
        private readonly static string _wsMineKey = "Mine_Data_Tr";


        private readonly static string _wsDailyKey = "Quest_Daily_Data_Tr";
        private readonly static string _wsWeeklyKey = "Quest_Weekly_Data_Tr";
        private readonly static string _wsChallengeKey = "Quest_Challenge_Data_Tr";
        private readonly static string _wsGoalKey = "Quest_Goal_Data_Tr";

        private readonly static string _wsSystemKey = "System_Tr";





        //[MenuItem("Data/Translate/Create And Update Unit Translate")]
        //static void CreateAndUpdateUnitTr()
        //{
        //    string directory = PATH;
        //    if (!Directory.Exists(directory))
        //    {
        //        Directory.CreateDirectory(directory);
        //    }
        //    GoogleSheetGenerator.CreateAndUpdateTextAsset(_sheetKey, _wsUnitKey, PATH, _bundleName);
        //}


        //[MenuItem("Data/Translate/Create And Update Enemy Translate")]
        //static void CreateAndUpdateEnemyTr()
        //{
        //    string directory = PATH;
        //    if (!Directory.Exists(directory))
        //    {
        //        Directory.CreateDirectory(directory);
        //    }
        //    GoogleSheetGenerator.CreateAndUpdateTextAsset(_sheetKey, _wsEnemyKey, PATH, _bundleName);
        //}

        //[MenuItem("Data/Translate/Create And Update Smithy Translate")]
        //static void CreateAndUpdateSmithyTr()
        //{
        //    string directory = PATH;
        //    if (!Directory.Exists(directory))
        //    {
        //        Directory.CreateDirectory(directory);
        //    }
        //    GoogleSheetGenerator.CreateAndUpdateTextAsset(_sheetKey, _wsSmithyKey, PATH, _bundleName);
        //}

        //[MenuItem("Data/Translate/Create And Update Village Translate")]
        //static void CreateAndUpdateVillageTr()
        //{
        //    string directory = PATH;
        //    if (!Directory.Exists(directory))
        //    {
        //        Directory.CreateDirectory(directory);
        //    }
        //    GoogleSheetGenerator.CreateAndUpdateTextAsset(_sheetKey, _wsVillageKey, PATH, _bundleName);
        //}

        //[MenuItem("Data/Translate/Create And Update Mine Translate")]
        //static void CreateAndUpdateMineTr()
        //{
        //    string directory = PATH;
        //    if (!Directory.Exists(directory))
        //    {
        //        Directory.CreateDirectory(directory);
        //    }
        //    GoogleSheetGenerator.CreateAndUpdateTextAsset(_sheetKey, _wsMineKey, PATH, _bundleName);
        //}

        //[MenuItem("Data/Translate/Create And Update Quest Daily Translate")]
        //static void CreateAndUpdateDailyTr()
        //{
        //    string directory = PATH;
        //    if (!Directory.Exists(directory))
        //    {
        //        Directory.CreateDirectory(directory);
        //    }
        //    GoogleSheetGenerator.CreateAndUpdateTextAsset(_sheetKey, _wsDailyKey, PATH, _bundleName);
        //}

        //[MenuItem("Data/Translate/Create And Update Quest Weekly Translate")]
        //static void CreateAndUpdateWeeklyTr()
        //{
        //    string directory = PATH;
        //    if (!Directory.Exists(directory))
        //    {
        //        Directory.CreateDirectory(directory);
        //    }
        //    GoogleSheetGenerator.CreateAndUpdateTextAsset(_sheetKey, _wsWeeklyKey, PATH, _bundleName);
        //}

        //[MenuItem("Data/Translate/Create And Update Quest Challenge Translate")]
        //static void CreateAndUpdateChallengeTr()
        //{
        //    string directory = PATH;
        //    if (!Directory.Exists(directory))
        //    {
        //        Directory.CreateDirectory(directory);
        //    }
        //    GoogleSheetGenerator.CreateAndUpdateTextAsset(_sheetKey, _wsChallengeKey, PATH, _bundleName);
        //}

        //[MenuItem("Data/Translate/Create And Update Quest Goal Translate")]
        //static void CreateAndUpdateGoalTr()
        //{
        //    string directory = PATH;
        //    if (!Directory.Exists(directory))
        //    {
        //        Directory.CreateDirectory(directory);
        //    }
        //    GoogleSheetGenerator.CreateAndUpdateTextAsset(_sheetKey, _wsGoalKey, PATH, _bundleName);
        //}

        //[MenuItem("Data/Translate/Create And Update System Translate")]
        //static void CreateAndUpdateSystemTr()
        //{
        //    string directory = PATH;
        //    if (!Directory.Exists(directory))
        //    {
        //        Directory.CreateDirectory(directory);
        //    }
        //    GoogleSheetGenerator.CreateAndUpdateTextAsset(_sheetKey, _wsSystemKey, PATH, _bundleName);
        //}


        //[MenuItem("AssetBundle/Build AssetBundles for Window")]
        //static void BuildAllAssetBundlesWindow()
        //{
        //    string assetBundleDirectory = PATH_BUNDLE;
        //    if (!Directory.Exists(assetBundleDirectory))
        //    {
        //        Directory.CreateDirectory(assetBundleDirectory);
        //    }
        //    BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        //}
    }
}
#endif