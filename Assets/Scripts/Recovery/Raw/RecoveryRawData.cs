namespace SDefence.Recovery.Raw {

    using UnityEngine;

    [System.Serializable]
    public class RecoveryRawData : ScriptableObject
    {
        [SerializeField] //DurablePopup
        private string _typeRecovery;

        [SerializeField]
        private string _startValue;

        [SerializeField]
        private string _increaseValue;

        [SerializeField]
        private string _increaseRate;


#if UNITY_EDITOR

        public static RecoveryRawData Create()
        {
            return new RecoveryRawData();
        }
        private RecoveryRawData()
        {
            _typeRecovery = "SDefence.Recovery.Raw.ShieldRecoveryUsableData";
            _startValue = "10";
            _increaseValue = "1";
            _increaseRate = "0.1";
        }
#endif

        internal IRecoveryUsableData GetUsableData(int upgrade = 0)
        {
            var type = System.Type.GetType(_typeRecovery);
            if (type != null)
            {
                var data = (IRecoveryUsableData)System.Activator.CreateInstance(type);
                data.SetData(_startValue, _increaseValue, _increaseRate, upgrade);
                return data;
            }
            else
            {
                throw new System.Exception($"{_typeRecovery} is not found Type");
            }
        }
    }
}