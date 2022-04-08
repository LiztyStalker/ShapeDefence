namespace SDefence.Attack.Raw {

    using UnityEngine;
    using Usable;

    [System.Serializable]
    public class AttackRawData
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

        [SerializeField]
        private string _startAttackRangeValue;

        [SerializeField]
        private string _increaseAttackRangeValue;

        [SerializeField]
        private string _increaseAttackRangeRate;


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
            _startAttackRangeValue = "5";
            _increaseAttackRangeValue = "0";
            _increaseAttackRangeRate = "0.1";
        }

        public void SetAttack(string startValue, string increaseValue, string increaseRate)
        {
            _startAttackValue = startValue;
            _increaseAttackValue = increaseValue;
            _increaseAttackRate = increaseRate;
        }

        public void SetDelay(string startValue, string decreaseValue, string decreaseRate)
        {
            _startAttackDelayValue = startValue;
            _decreaseAttackDelayValue = decreaseValue;
            _decreaseAttackDelayRate = decreaseRate;
        }

        public void SetRange(string startValue, string increaseValue, string increaseRate)
        {
            _startAttackRangeValue = startValue;
            _increaseAttackRangeValue = increaseValue;
            _increaseAttackRangeRate = increaseRate;
        }

        public void SetData(string startValue, string increaseValue, string increaseRate, string startDelayValue, string decreaseDelayValue, string decreaseDelayRate)
        {
            _startAttackValue = startValue;
            _increaseAttackValue = increaseValue;
            _increaseAttackRate = increaseRate;
            _startAttackDelayValue = startDelayValue;
            _decreaseAttackDelayValue = decreaseDelayValue;
            _decreaseAttackDelayRate = decreaseDelayRate;
        }

        public void SetData(string startAttackRangeValue, string increaseAttackRangeValue, string increaseAttackRangeRate)
        {
            _startAttackRangeValue = startAttackRangeValue;
            _increaseAttackRangeValue = increaseAttackRangeValue;
            _increaseAttackRangeRate = increaseAttackRangeRate;
        }

#endif

        public IAttackUsableData GetUsableData(int upgrade = 0)
        {
            var data = System.Activator.CreateInstance<AttackUsableData>();
            data.SetAttack(_startAttackValue, _increaseAttackValue, _increaseAttackRate, upgrade);
            data.SetDelay(_startAttackDelayValue, _decreaseAttackDelayValue, _decreaseAttackDelayRate, upgrade);
            data.SetRange(_startAttackRangeValue, _increaseAttackRangeValue, _increaseAttackRangeRate, upgrade);
            return data;
        }

    }
}