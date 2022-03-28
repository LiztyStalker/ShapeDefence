namespace Utility.ScriptableObjectData
{
    using UnityEngine;

    public abstract class ScriptableObjectData : ScriptableObject
    {
        [SerializeField]
        private string _key;
        public string Key { get => _key; protected set => _key = value; }

#if UNITY_EDITOR

        [SerializeField]
        private int _sortIndex;
        public int SortIndex { get => _sortIndex; protected set => _sortIndex = value; }

        public void SetSortIndex(int index) => SortIndex = index;
        public abstract void SetData(string[] arr);
        public abstract void AddData(string[] arr);
        public abstract bool HasDataArray();
        public abstract string[] GetData();
        public abstract string[][] GetDataArray();
        public virtual void SetAssetBundle(string bundleName)
        {
            var path = UnityEditor.AssetDatabase.GetAssetPath(this);
            UnityEditor.AssetImporter importer = UnityEditor.AssetImporter.GetAtPath(path);
            importer.SetAssetBundleNameAndVariant(bundleName, "");
        }

        [SerializeField]
        private bool _isTest = false;
        public bool IsTest => _isTest;

#endif

    }
}