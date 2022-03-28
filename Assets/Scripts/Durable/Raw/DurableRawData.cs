namespace SDefence.Durable.Raw {

    using UnityEngine;

    [System.Serializable]
    public class DurableRawData : ScriptableObject
    {
        [SerializeField] //DurablePopup
        private string _typeDurable;

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
            _typeDurable = "SDefence.Durable.Raw.HealthDurableUsableData";
            _startValue = "100";
            _increaseValue = "1";
            _increaseRate = "0.1";
        }
#endif

        internal IDurableUsableData GetUsableData(int upgrade = 0)
        {
            var type = System.Type.GetType(_typeDurable);
            if (type != null)
            {
                var data = (IDurableUsableData)System.Activator.CreateInstance(type);
                data.SetData(_startValue, _increaseValue, _increaseRate, upgrade);
                return data;
            }
            else
            {
                throw new System.Exception($"{_typeDurable} is not found Type");
            }
        }
    }
}