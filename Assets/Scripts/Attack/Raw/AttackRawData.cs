namespace SDefence.Attack.Raw {

    using UnityEngine;
    using Usable;

    [System.Serializable]
    public class AttackRawData : ScriptableObject
    {
        [SerializeField]
        private string _startAttackValue;

        [SerializeField]
        private string _increaseAttackValue;

        [SerializeField]
        private string _increaseAttackRate;

        [SerializeField]
        private string _startAttackDelayValue;

        [SerializeField]
        private string _decreaseAttackDelayValue;

        [SerializeField]
        private string _decreaseAttackDelayRate;


#if UNITY_EDITOR

        public static AttackRawData Create()
        {
            return new AttackRawData();
        }
        private AttackRawData()
        {
            _startAttackValue = "10";
            _increaseAttackValue = "1";
            _increaseAttackRate = "0.1";
            _startAttackDelayValue = "1";
            _decreaseAttackDelayValue = "0";
            _decreaseAttackDelayRate = "0.1";
        }
#endif

        internal IAttackUsableData GetUsableData(int upgrade = 0)
        {
            var data = System.Activator.CreateInstance<AttackUsableData>();
            data.SetData(_startAttackValue, _increaseAttackValue, _increaseAttackRate, _startAttackDelayValue, _decreaseAttackDelayValue, _decreaseAttackDelayRate, upgrade);
            return data;
        }
    }
}