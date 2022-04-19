#if UNITY_EDITOR
namespace SDefence.Tech.Generator
{
    using UnityEditor;
    using Utility.Generator;
    using UtilityManager;

    public class TechDataGenerator
    {
        public enum TYPE_SHEET_COLUMNS
        {
            Key,
            TechDataKey,
            TypeTechData,
            TypeConditionTech,
            ConditionTechKey,
            ConditionTechValue,
            TypeAssetData,
            TechAssetValue
        }

        //private readonly static string _dataPath = "Assets/Data/Tech";
        //private readonly static string _bundleName = "data/tech";
        //private readonly static string _sheetKey = "1RwNsRfdv78BLc1ziQQeTqIRMVC2bmNU8lnYVHycnrgo";
        //private readonly static string _worksheetKey = "Tech_Data";

        //[MenuItem("Data/Tech/Create And Update All TechData")]
        //private static void CreateAndUpdateAllData()
        //{
        //    GoogleSheetGenerator.CreateAndUpdateAllData<TechData>(_sheetKey, _worksheetKey, _dataPath, _bundleName);
        //}


        //[MenuItem("Data/Bullets/Upload All Bullets")]
        //private static void UploadAllData()
        //{
        //    GoogleSheetGenerator.UploadAllData<BulletData>(_sheetKey, _worksheetKey, _dataPath, () => UnityEngine.Debug.Log("Upload End"));
        //}
    }
}
#endif