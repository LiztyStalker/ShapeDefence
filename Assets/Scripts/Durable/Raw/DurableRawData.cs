namespace SDefence.Durable.Raw {

    using UnityEngine;

    [System.Serializable]
    public class DurableRawData
    {
        [SerializeField] //DurablePopup
        private string _typeData;

        [SerializeField]
        private string _startValue;

        [SerializeField]
        private string _increaseValue;

        [SerializeField]
        private string _increaseRate;


#if UNITY_EDITOR

        public static DurableRawData Create()
        {
            return new DurableRawData();
        }
        private DurableRawData()
        {
            _typeData = "SDefence.Durable.Raw.HealthDurableUsableData";
            _startValue = "100";
            _increaseValue = "1";
            _increaseRate = "0.1";
        }

        public void SetData(string typeData, string startValue, string increaseValue, string increaseRate)
        {
            _typeData = typeData;
            _startValue = startValue;
            _increaseValue = increaseValue;
            _increaseRate = increaseRate;
        }
#endif

        public IDurableUsableData GetUsableData(int upgrade = 0)
        {
            var type = System.Type.GetType(_typeData);
            if (type != null)
            {
                var data = (IDurableUsableData)System.Activator.CreateInstance(type);
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