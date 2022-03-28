#if UNITY_EDITOR
namespace Utility.Bullet
{
    using UnityEditor;
    using Data;
    using Generator;

    public class BulletDataGenerator
    {
        public enum TYPE_SHEET_COLUMNS
        {
            Key,                
            TypeBulletAction,
            TypeAttack,
            MoveSpeed,
            Height,
            IsRotate,
            BulletSpriteKey,
            BulletHitKey
        }

        private readonly static string _dataPath = "Assets/Data/Bullets";
        private readonly static string _bundleName = "data/bullets";
        private readonly static string _sheetKey = "16Ps885lj_8ZSY6Gv_WQgehpCxusaz2l9J50JBXKq2DY";
        private readonly static string _worksheetKey = "Bullet_Data";

        [MenuItem("Data/Bullets/Create And Update All Bullets")]
        private static void CreateAndUpdateAllData()
        {
            GoogleSheetGenerator.CreateAndUpdateAllData<BulletData>(_sheetKey, _worksheetKey, _dataPath, _bundleName);
        }


        //[MenuItem("Data/Bullets/Upload All Bullets")]
        //private static void UploadAllData()
        //{
        //    GoogleSheetGenerator.UploadAllData<BulletData>(_sheetKey, _worksheetKey, _dataPath, () => UnityEngine.Debug.Log("Upload End"));
        //}
    }
}
#endif