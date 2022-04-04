namespace SDefence.Tech.Raw
{
    using UnityEngine;

    [System.Serializable]
    public class TechRawData
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


#if UNITY_EDITOR
        public static TechRawData Create() => new TechRawData();

        private TechRawData()
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
#endif
    }
}