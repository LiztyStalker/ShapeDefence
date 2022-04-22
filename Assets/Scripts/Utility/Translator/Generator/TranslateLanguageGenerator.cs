#if UNITY_EDITOR
namespace UtilityManager
{
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    using Utility.Generator;

    public class TranslateLanguageGenerator
    {
        private readonly static string PATH = "Assets/Data/Language";
        private readonly static string BUNDLE_NAME = "language";
        private readonly static string WORKSHEET_KEY = "1RwNsRfdv78BLc1ziQQeTqIRMVC2bmNU8lnYVHycnrgo";
        private readonly static string SHEET_KEY = "Language_Data";


        public enum TYPE_SHEET_COLUMNS
        {
            Group,
            Key,
            IconKey,
            Text,            
        }


        [MenuItem("Data/Language/Create And Update Language Data")]
        static void CreateAndUpdateData()
        {
            string directory = PATH;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            GoogleSheetGenerator.CreateAndUpdateAllData<TranslateLanguageData>(WORKSHEET_KEY, SHEET_KEY, PATH, BUNDLE_NAME);
        }
    }
}
#endif