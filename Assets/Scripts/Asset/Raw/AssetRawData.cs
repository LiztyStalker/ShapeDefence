namespace SDefence.Asset.Raw {

    using UnityEngine;

    [System.Serializable]
    public class AssetRawData
    {
        [SerializeField] //AssetUsablePopup
        private string _typeData;

        [SerializeField]
        private string _startValue;

        [SerializeField]
        private string _increaseValue;

        [SerializeField]
        private string _increaseRate;


#if UNITY_EDITOR

        public static AssetRawData Create()
        {
            return new AssetRawData();
        }
        private AssetRawData()
        {
            _typeData = "SDefence.Asset.Usable.NeutralAssetUsableData";
            _startValue = "100";
            _increaseValue = "1";
            _increaseRate = "0.1";
        }

        public void SetData(string typeData, string startValue, string increaseValue, string increaseRate)
        {
            _typeData = $"SDefence.Asset.Usable.{typeData}AssetUsableData";
            _startValue = startValue;
            _increaseValue = increaseValue;
            _increaseRate = increaseRate;
        }


        public AssetRawData Clone()
        {
            var data = Create();
            data.SetData(_typeData, _startValue, _increaseValue, _increaseRate);
            return data;
        }
#endif

        public IAssetUsableData GetUsableData(int upgrade = 0)
        {
            var type = System.Type.GetType(_typeData);
            if (type != null)
            {
                var data = (IAssetUsableData)System.Activator.CreateInstance(type);
                data.SetData(_startValue, _increaseValue, _increaseRate, upgrade);
                return data;
            }
#if UNITY_EDITOR
            else
            {
                throw new System.Exception($"{_typeData} is not found Type");
            }
#endif
        }

    }
}