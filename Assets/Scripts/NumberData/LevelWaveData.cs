namespace SDefence.Data
{
    using Utility.Number;

    public class LevelWaveData : INumberData
    {
        private const int WAVE = 5;


        private int _value;


        public int GetLevel() => _value / WAVE;
        public int GetWave() => _value % WAVE;

        private void SetValue(int value) => _value = value;

        public void IncreaseNumber(int value = 1) => _value += value;

        public bool IsLastWave() => GetWave() == WAVE - 1;
        public int MaxWave() => WAVE;
        public void CleanUp()
        {
            _value = 0;
        }

        public INumberData Clone()
        {
            var data = new LevelWaveData();
            data.SetValue(_value);
            return data;
        }
    }
}