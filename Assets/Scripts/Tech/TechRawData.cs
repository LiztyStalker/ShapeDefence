namespace SDefence.Tech
{
    using System.Collections.Generic;
    using UnityEngine;
    using Utility;
    using Asset.Raw;
    using SDefence.Asset;
#if UNITY_EDITOR
    using Generator;
#endif


    public interface ITechable
    {
        public TechRawData TechRawData { get; }
    }


    [System.Serializable]
    public class TechRawElement
    {
        [SerializeField]
        private string _typeTechData;
        [SerializeField]
        private string _techDataKey;
        //[SerializeField]
        //private int _typeConditionTech;
        //[SerializeField]
        //private int _conditionTechValue;

        [SerializeField]
        private AssetRawData _assetData;

        public string TypeTechData => _typeTechData;
        public string TechDataKey => _techDataKey;

        //public int TypeConditionTech => _typeConditionTech;
        //public int ConditionTechValue => _conditionTechValue;


        public static TechRawElement Create() => new TechRawElement();

        public TechRawElement() 
        {
            _typeTechData = "Test";
            _techDataKey = "Test";
            //_typeConditionTech = typeConditionTech;
            //_conditionTechValue = conditionTechValue;
#if UNITY_EDITOR
            _assetData = AssetRawData.Create();
#endif
        }

        public void SetTypeTechData(string typeTechData)
        {
            _typeTechData = typeTechData;
        }

        public void SetData(string typeTechData, string techDataKey, string typeAssetData, string techAssetValue)
        {
            _typeTechData = typeTechData;
            _techDataKey = techDataKey;
            //_typeConditionTech = typeConditionTech;
            //_conditionTechValue = conditionTechValue;

#if UNITY_EDITOR
            _assetData = AssetRawData.Create();
            _assetData.SetData(typeAssetData, techAssetValue, "0", "0");
#endif
        }

        private void SetData(string typeTechData, string techDataKey, AssetRawData assetData)
        {
            _typeTechData = typeTechData;
            _techDataKey = techDataKey;
            //_typeConditionTech = typeConditionTech;
            //_conditionTechValue = conditionTechValue;
#if UNITY_EDITOR
            _assetData = assetData.Clone();
#endif
        }

        public IAssetUsableData GetUsableData() => _assetData.GetUsableData();

        public TechRawElement Clone()
        {
            var data = Create();
            data.SetData(_typeTechData, _techDataKey, _assetData);
            return data;
        }
    }



    [System.Serializable]
    public class TechRawData : ISheetData
    {
        [SerializeField]
        private TechRawElement[] _techRawElements;

        public TechRawElement[] TechRawElements => _techRawElements;

        public bool IsEmpty() => _techRawElements == null || _techRawElements.Length == 0;

#if UNITY_EDITOR

        public static TechRawData Create(string type) => new TechRawData(type);

        public TechRawData() { }

        private TechRawData(string type)
        {
            _techRawElements = new TechRawElement[1];
            _techRawElements[0] = TechRawElement.Create();
            _techRawElements[0].SetTypeTechData(type);

        }

        private void SetData(TechRawData raw)
        {
            _techRawElements = new TechRawElement[raw.TechRawElements.Length];
            for(int i = 0; i < _techRawElements.Length; i++)
            {
                _techRawElements[i] = raw.TechRawElements[i].Clone();
            }
        }

        public void SetData(string[] arr)
        {
            _techRawElements = new TechRawElement[1];

            var data = TechRawElement.Create();
            data.SetData(
                arr[(int)TechDataGenerator.TYPE_SHEET_COLUMNS.TypeTechData],
                arr[(int)TechDataGenerator.TYPE_SHEET_COLUMNS.TechDataKey],
                arr[(int)TechDataGenerator.TYPE_SHEET_COLUMNS.TypeAssetData],
                arr[(int)TechDataGenerator.TYPE_SHEET_COLUMNS.TechAssetValue]
                );

            _techRawElements[0] = data;
        }

        public void AddData(string[] arr)
        {
            var data = TechRawElement.Create();
            data.SetData(
                arr[(int)TechDataGenerator.TYPE_SHEET_COLUMNS.TypeTechData],
                arr[(int)TechDataGenerator.TYPE_SHEET_COLUMNS.TechDataKey],
                arr[(int)TechDataGenerator.TYPE_SHEET_COLUMNS.TypeAssetData],
                arr[(int)TechDataGenerator.TYPE_SHEET_COLUMNS.TechAssetValue]
                );

            var list = new List<TechRawElement>(_techRawElements);
            list.Add(data);
            _techRawElements = list.ToArray();
        }

        public void ClearData() => _techRawElements = null;


        public TechRawData Clone()
        {
            var data = new TechRawData();
            data.SetData(this);
            return data;
        }
#endif
    }
}