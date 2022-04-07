#if UNITY_EDITOR
namespace SDefence.Turret.Generator
{
    using UnityEditor;
    using Utility.Generator;
    using UtilityManager;

    public class TurretDataGenerator
    {
        public enum TYPE_SHEET_COLUMNS
        {
            Key,
            IconKey,
            GraphicObjectKey,
            StartHealthValue,
            IncreaseHealthValue,
            IncreaseHealthRate,
            StartArmorValue,
            IncreaseArmorValue,
            IncreaseArmorRate,
            StartShieldValue,
            IncreaseShieldValue,
            IncreaseShieldRate,
            StartRecoveryShieldValue,
            IncreaseRecoveryShieldValue,
            IncreaseRecoveryShieldRate,
            StartFloorShieldValue,
            DecreaseFloorShieldValue,
            DecreaseFloorShieldRate,
            TypeAssetData,
            StartUpgradeValue,
            IncreaseUpgradeValue,
            IncreaseUpgradeRate,
            MaxUpgradeCount,
            RepairTime,
            StartAttackValue,
            IncreaseAttackValue,
            IncreaseAttackRate,
            StartAttackDelayValue,
            DecreaseAttackDelayValue,
            DecreaseAttackDelayRate,
            BulletDataKey,
            TechDataKey,
        }

        private readonly static string _dataPath = "Assets/Data/Turret";
        private readonly static string _bundleName = "data/turret";
        private readonly static string _sheetKey = "1RwNsRfdv78BLc1ziQQeTqIRMVC2bmNU8lnYVHycnrgo";
        private readonly static string _worksheetKey = "Turret_Data";

        [MenuItem("Data/Turret/Create And Update All TurretData")]
        private static void CreateAndUpdateAllData()
        {
            GoogleSheetGenerator.CreateAndUpdateAllData<TurretData>(_sheetKey, _worksheetKey, _dataPath, _bundleName);
        }


        //[MenuItem("Data/Bullets/Upload All Bullets")]
        //private static void UploadAllData()
        //{
        //    GoogleSheetGenerator.UploadAllData<BulletData>(_sheetKey, _worksheetKey, _dataPath, () => UnityEngine.Debug.Log("Upload End"));
        //}
    }
}
#endif