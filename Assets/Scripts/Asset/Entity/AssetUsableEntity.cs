namespace SDefence.Asset.Entity
{
    using System.Collections.Generic;
    using Utility.IO;
    using Usable;
    using System.Linq;

    public class AssetUsableEntity : ISavable
    {
        private Dictionary<string, IAssetUsableData> _dic;
        //Key
        //Neutral / AssetUsableData
        //Star / AssetUsableData

        //Value
        //IAssetUsableData

        public string[] Keys => _dic.Keys.ToArray();

        public static AssetUsableEntity Create() => new AssetUsableEntity();
        private AssetUsableEntity()
        {
            _dic = new Dictionary<string, IAssetUsableData>();
        }

        public void CleanUp()
        {
            Clear();
            _dic = null;
        }

        public void Clear()
        {
            _dic.Clear();
        }


        public bool HasKey(string key) => _dic.ContainsKey(key);
        private string GetKey(IAssetUsableData uData) => uData.GetType().Name.Replace("AssetUsableData", "");
        private string GetKey<T>() => typeof(T).Name.Replace("AssetUsableData", "");

        public string GetValue(string key)
        {
            if (_dic.ContainsKey(key))
            {
                return _dic[key].ToString();
            }
#if UNITY_EDITOR
            return "-Empty-";
#else
            return "0";
#endif
        }

        public string GetValue<T>(string format = null) where T : IAssetUsableData
        {
            string key = typeof(T).Name.Replace("AssetUsableData", "");
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


        public void Refresh()
        {
            OnAssetEntityEvent();
        }

        public void Add(AssetUsableEntity assetEntity)
        {
            foreach(var value in assetEntity._dic.Values)
            {
                Add(value);
            }
        }

        public void Add(IAssetUsableData uData)
        {
            string key = GetKey(uData);
            if (!_dic.ContainsKey(key))
            {
                _dic.Add(key, uData.Clone());
            }
            else
            {
                _dic[key].Add(uData);
            }
            OnAssetEntityEvent();
        }

        public void Subject(IAssetUsableData uData)
        {
            string key = GetKey(uData);
            if (!_dic.ContainsKey(key))
            {
                _dic.Add(key, uData.Clone());
                _dic[key].Subject(uData);
            }
            _dic[key].Subject(uData);
            OnAssetEntityEvent();
        }

        public void Set(AssetUsableEntity assetEntity)
        {
            foreach (var value in assetEntity._dic.Values)
            {
                Set(value);
            }
        }

        public void Set(IAssetUsableData uData)
        {
            string key = GetKey(uData);
            if (!_dic.ContainsKey(key))
            {
                _dic.Add(key, uData.Clone());
            }
            else
            {
                _dic[key] = uData;
            }
            OnAssetEntityEvent();
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

        public int Compare(IAssetUsableData data)
        {
            var key = GetKey(data);
            if (_dic.ContainsKey(key))
            {
                return _dic[key].Compare(data);
            }
            return 1;
        }

        #region ##### Listener #####
        private System.Action<AssetUsableEntity> _entityEvent;

        public void SetOnAssetEntityListener(System.Action<AssetUsableEntity> act) => _entityEvent = act;

        private void OnAssetEntityEvent() => _entityEvent?.Invoke(this);
        #endregion


        #region ##### Savable #####

        public void SetSavableData(SavableData data)
        {
            foreach (var key in data.Children.Keys)
            {
                var uData = AbstractAssetUsableData.Create(key);
                if (uData != null)
                {
                    var savableData = data.Children[key];
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