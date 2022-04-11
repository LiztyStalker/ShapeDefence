#if UNITY_EDITOR
namespace SDefence.Enemy.Generator
{
    using UnityEditor;
    using Utility.Generator;
    using UtilityManager;

    public class EnemyDataGenerator
    {
        public enum TYPE_SHEET_COLUMNS
        {
            Key,
            IconKey,
            GraphicObjectKey,
            TypeEnemy,
            StartHealthValue,
            IncreaseHealthValue,
            IncreaseHealthRate,
            StartArmorValue,
            IncreaseArmorValue,
            IncreaseArmorRate,
            StartShieldValue,
            IncreaseShieldValue,
            IncreaseShieldRate,
            StartFloorShieldValue,
            DecreaseFloorShieldValue,
            DecreaseFloorShieldRate,
            TypeRewardAssetData,
            StartRewardAssetValue,
            IncreaseRewardAssetValue,
            IncreaseRewardAssetRate,
            IsAttack,
            StartAttackValue,
            IncreaseAttackValue,
            IncreaseAttackRate,
            StartAttackDelayValue,
            DecreaseAttackDelayValue,
            DecreaseAttackDelayRate,
            StartAttackRangeValue,
            DecreaseAttackRangeValue,
            DecreaseAttackRangeRate,
            StartMovementValue,
            IncreaseMovementValue,
            IncreaseMovementRate,
            TypeMovement,
            Accuracy,
            BulletDataKey,
        }

        private readonly static string _dataPath = "Assets/Data/Enemy";
        private readonly static string _bundleName = "data/enemy";
        private readonly static string _sheetKey = "1RwNsRfdv78BLc1ziQQeTqIRMVC2bmNU8lnYVHycnrgo";
        private readonly static string _worksheetKey = "Enemy_Data";

        [MenuItem("Data/Enemy/Create And Update All EnemyData")]
        private static void CreateAndUpdateAllData()
        {
            GoogleSheetGenerator.CreateAndUpdateAllData<EnemyData>(_sheetKey, _worksheetKey, _dataPath, _bundleName);
        }


        //[MenuItem("Data/Bullets/Upload All Bullets")]
        //private static void UploadAllData()
        //{
        //    GoogleSheetGenerator.UploadAllData<BulletData>(_sheetKey, _worksheetKey, _dataPath, () => UnityEngine.Debug.Log("Upload End"));
        //}
    }
}
#endif