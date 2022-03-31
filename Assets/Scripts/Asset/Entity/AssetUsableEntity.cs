namespace SDefence.Asset.Entity
{
    using System.Collections.Generic;
    using Utility.IO;
    using Usable;


    public class AssetUsableEntity : ISavable
    {
        private Dictionary<string, IAssetUsableData> _dic;

        public static AssetUsableEntity Create() => new AssetUsableEntity();
        private AssetUsableEntity()
        {
            _dic = new Dictionary<string, IAssetUsableData>();
        }

        public void CleanUp()
        {
            _dic.Clear();
            _dic = null;
        }

        private string GetKey(IAssetUsableData uData) => uData.GetType().Name;

        private string GetKey<T>() => typeof(T).Name;

        public string GetValue<T>(string format = null) where T : IAssetUsableData
        {
            string key = typeof(T).Name;
            if (_dic.ContainsKey(key))
            {
                return _dic[key].ToString(format);
            }
#if UNITY_EDITOR
            return "-Empty-";
#else
            return "";
#endif

        }

        public void Add(IAssetUsableData uData)
        {
            string key = GetKey(uData);
            if (!_dic.ContainsKey(key))
            {
                _dic.Add(key, uData);
            }
            else
            {
                _dic[key].Add(uData);
            }
        }


        public void Subject(IAssetUsableData uData)
        {
            string key = GetKey(uData);
            if (!_dic.ContainsKey(key))
            {
                _dic.Add(key, uData);
                _dic[key].Subject(uData);
            }
            _dic[key].Subject(uData);
        }

        public void Set(IAssetUsableData uData)
        {
            string key = GetKey(uData);
            if (!_dic.ContainsKey(key))
            {
                _dic.Add(key, uData);
            }
            else
            {
                _dic[key] = uData;
            }
        }
              

        public bool IsEnough(IAssetUsableData uData)
        {
            string key = GetKey(uData);
            if (_dic.ContainsKey(key))
            {
                return (_dic[key].Compare(uData) <= 0);
            }
            return false;
        }


        #region ##### Savable #####

        public void SetSavableData(SavableData data)
        {
            var unpackData = data;
            foreach(var key in unpackData.Children.Keys)
            {
                var uData = AbstractAssetUsableData.Create(key);
                if (uData != null)
                {
                    var savableData = unpackData.Children[key];
                    uData.SetSavableData((SavableData)savableData);
                    Set(uData);
                }

#if UNITY_EDITOR
                else
                {
                    UnityEngine.Debug.LogWarning($"{key} failed Savable Loading");
                }
#endif
            }
        }

        public SavableData GetSavableData()
        {
            var data = SavableData.Create();
            foreach(var value in _dic.Values)
            {
                data.AddData(value.SavableKey(), value.GetSavableData());
            }
            return data;
        }

        public string SavableKey() => typeof(AssetUsableEntity).Name;

        #endregion
    }
}