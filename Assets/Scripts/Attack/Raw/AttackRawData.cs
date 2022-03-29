namespace SDefence.Attack.Raw {

    using UnityEngine;

    [System.Serializable]
    public class AttackRawData : ScriptableObject
    {
        [SerializeField] //DurablePopup
        private string _type;

        [SerializeField]
        private string _startValue;

        [SerializeField]
        private string _increaseValue;

        [SerializeField]
        private string _increaseRate;


#if UNITY_EDITOR

        public static AttackRawData Create()
        {
            return new AttackRawData();
        }
        private AttackRawData()
        {
            _type = "SDefence.Attack.Usable.AttackUsableData";
            _startValue = "10";
            _increaseValue = "1";
            _increaseRate = "0.1";
        }
#endif

        internal IAttackUsableData GetUsableData(int upgrade = 0)
        {
            var type = System.Type.GetType(_type);
            if (type != null)
            {
                var data = (IAttackUsableData)System.Activator.CreateInstance(type);
                data.SetData(_startValue, _increaseValue, _increaseRate, upgrade);
                return data;
            }
            else
            {
                throw new System.Exception($"{_type} is not found Type");
            }
        }
    }
}