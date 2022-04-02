namespace SDefence.BattleGen.Data
{
    using UnityEngine;

    [System.Serializable]
    public struct BattleGenWaveData
    {
        [SerializeField]
        private BattleGenWaveElement[] _waveElementArray;

        private bool IsOverflow(int index)
        {
            return index >= _waveElementArray.Length;
        }

        public bool HasWaveData(int index, float nowTime)
        {
            if (IsOverflow(index)) return false;
            return (_waveElementArray[index].WaveAppearDelay <= nowTime);
        }

        public BattleGenWaveElement? GetBattleGenWaveElement(int index)
        {
            if (IsOverflow(index)) return null;
            return _waveElementArray[index];
        }

#if UNITY_EDITOR
        public static BattleGenWaveData Create(string key = null) => new BattleGenWaveData(key);

        private BattleGenWaveData(string key)
        {
            _waveElementArray = new BattleGenWaveElement[3];
            for (int i = 0; i < _waveElementArray.Length; i++)
            {
                _waveElementArray[i] = BattleGenWaveElement.Create();
            }
        }

        public void SetData(BattleGenWaveElement[] elements)
        {
            _waveElementArray = elements;
        }

        public void SetData(BattleGenWaveElement element)
        {
            _waveElementArray = new BattleGenWaveElement[1];
            _waveElementArray[0] = element;
        }
#endif
    }
}