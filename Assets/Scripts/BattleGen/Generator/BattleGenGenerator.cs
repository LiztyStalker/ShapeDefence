#if UNITY_EDITOR
namespace SDefence.BattleGen.Generator
{
    using SDefence.BattleGen.Data;
    using UnityEditor;
    using Utility.Generator;
    using UtilityManager;

    public class BattleGenGenerator
    {
        public enum TYPE_SHEET_LEVEL_COLUMNS
        {
            Key,
            Level,
            Wave1,
            Wave2,
            Wave3,
            Wave4,
            Wave5
        }

        public enum TYPE_SHEET_WAVE_COLUMNS
        {
            Key,
            EnemyDataKey,
            AppearCount,
            Weight,
            WaveAppearDelay,
            TypeEnemyAppearAction,
            TypeEnemyAppearPosition,
            EnemyAppearDelay
        }

        private readonly static string _dataPath = "Assets/Data/BattleGen";
        private readonly static string _bundleName = "data/battlegen";
        private readonly static string _sheetKey = "1RwNsRfdv78BLc1ziQQeTqIRMVC2bmNU8lnYVHycnrgo";
        private readonly static string _worksheetLevelKey = "Enemy_Level_Data";
        private readonly static string _worksheetWaveKey = "Enemy_Wave_Data";
        

        [MenuItem("Data/BattleGen/Create And Update All BattleGen")]
        private static void CreateAndUpdateAllData()
        {

            GoogleSheetGenerator.SheetAllData<BattleGenWaveData>(_sheetKey, _worksheetWaveKey, waves =>
            {

                GoogleSheetGenerator.CreateAndUpdateAllData<BattleGenLevelData>(_sheetKey, _worksheetLevelKey, _dataPath, _bundleName, levels =>
                {
                    foreach (var level in levels.Values)
                    {
                        for(int i = 0; i < level.WaveDataKeys.Length; i++)
                        {
                            var key = level.WaveDataKeys[i];
                            if (waves.ContainsKey(key))
                            {
                                level.SetWaveData(waves[key], i);
                            }
                            else
                            {
                                UnityEngine.Debug.LogWarning($"-{key}- is not found");
                            }
                        }

                        EditorUtility.SetDirty(level);
                        AssetDatabase.SaveAssets();
                    }
                });
            });

        }


        //[MenuItem("Data/Bullets/Upload All Bullets")]
        //private static void UploadAllData()
        //{
        //    GoogleSheetGenerator.UploadAllData<BulletData>(_sheetKey, _worksheetKey, _dataPath, () => UnityEngine.Debug.Log("Upload End"));
        //}
    }
}
#endif