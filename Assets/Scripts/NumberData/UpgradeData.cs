namespace SDefence.Data
{
    using UnityEngine;
    using Utility.Number;

    public class UpgradeData : INumberData
    {
        private int _value;

        public void CleanUp()
        {
            _value = 0;
        }

        public int GetValue() => _value;
        public void IncreaseNumber(int value = 1) => _value += value;
        public void SetValue(int value)
        {
            _value = value;
        }

        public INumberData Clone()
        {
            var data = new UpgradeData();
            data.SetValue(_value);
            return data;
        }

        public static UpgradeData Create() => new UpgradeData();
    }
}