namespace SDefence.Tech
{
    using SDefence.Tech.Generator;
    using System.Collections.Generic;
    using UnityEngine;
    using Utility;

    //public class TechData : ScriptableObjectData
    //{
    //    [SerializeField]
    //    private TechRawData[] _techRawDataArray;

    //    public override void AddData(string[] arr)
    //    {
    //        var list = new List<TechRawData>(_techRawDataArray);
    //        var data = TechRawData.Create();
    //        data.SetData(
    //            arr[(int)TechDataGenerator.TYPE_SHEET_COLUMNS.TechDataKey],
    //            arr[(int)TechDataGenerator.TYPE_SHEET_COLUMNS.TypeTechData],
    //            arr[(int)TechDataGenerator.TYPE_SHEET_COLUMNS.TypeAssetData],
    //            arr[(int)TechDataGenerator.TYPE_SHEET_COLUMNS.TechAssetValue]
    //        );
    //        list.Add(data);
    //        _techRawDataArray = list.ToArray();
    //    }

    //    public override bool HasDataArray()
    //    {
    //        return true;
    //    }

    //    public override void SetData(string[] arr)
    //    {
    //        Key = arr[(int)TechDataGenerator.TYPE_SHEET_COLUMNS.Key];

    //        _techRawDataArray = new TechRawData[1];
    //        var data = TechRawData.Create();
    //        data.SetData(
    //            arr[(int)TechDataGenerator.TYPE_SHEET_COLUMNS.TechDataKey],
    //            arr[(int)TechDataGenerator.TYPE_SHEET_COLUMNS.TypeTechData],
    //            arr[(int)TechDataGenerator.TYPE_SHEET_COLUMNS.TypeAssetData],
    //            arr[(int)TechDataGenerator.TYPE_SHEET_COLUMNS.TechAssetValue]
    //        );
    //        _techRawDataArray[0] = data;

    //    }
    //}

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
        private string _typeAssetData;
        [SerializeField]
        private int _techAssetValue;

        public string TypeTechData => _typeTechData;
        public string TechDataKey => _techDataKey;
        //public int TypeConditionTech => _typeConditionTech;
        //public int ConditionTechValue => _conditionTechValue;
        public string TypeAssetData => _typeAssetData;
        public int TechAssetValue => _techAssetValue;


        public static TechRawElement Create() => new TechRawElement();
        public TechRawElement()
        {
            _typeTechData = "Test";
            _techDataKey = "Test";
            //_typeConditionTech = typeConditionTech;
            //_conditionTechValue = conditionTechValue;
            _typeAssetData = "Neutral";
            _techAssetValue = 100;
        }
        public void SetData(string typeTechData, string techDataKey, string typeAssetData, string techAssetValue)
        {
            _typeTechData = typeTechData;
            _techDataKey = techDataKey;
            //_typeConditionTech = typeConditionTech;
            //_conditionTechValue = conditionTechValue;
            _typeAssetData = typeAssetData;
            _techAssetValue = int.Parse(techAssetValue);
        }

        public TechRawElement Clone()
        {
            var data = Create();
            data.SetData(_typeTechData, _techDataKey, _typeAssetData, _techAssetValue.ToString());
            return data;
        }
    }



    [System.Serializable]
    public class TechRawData : ISheetData
    {
        [SerializeField]
        private TechRawElement[] _techRawElements;

        public TechRawElement[] TechRawElements => _techRawElements;
#if UNITY_EDITOR
        
        public TechRawData()
        {
            
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