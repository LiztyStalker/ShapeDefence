namespace SDefence.BattleGen.Data
{
    using UnityEngine;
    using Utility.ScriptableObjectData;
    using Generator;

    [System.Serializable]
    public class BattleGenLevelData : ScriptableObjectData
    {
        [SerializeField]
        private int _level;

        [HideInInspector]
        private string[] _waveDataKeys;

        [SerializeField]
        private BattleGenWaveData[] _waveDataArray;


        public BattleGenWaveData GetBattleGenWaveData(int wave) 
        {
            if(wave < _waveDataArray.Length)
            {
                return _waveDataArray[wave];
            }
            return null;
        }

#if UNITY_EDITOR

        public string[] WaveDataKeys => _waveDataKeys;

        public static BattleGenLevelData Create() => new BattleGenLevelData();

        private BattleGenLevelData()
        {
            Key = "Test";
            _level = 0;
            _waveDataArray = new BattleGenWaveData[5];
            for (int i = 0; i < _waveDataArray.Length; i++)
            {
                _waveDataArray[i] = BattleGenWaveData.Create();
            }
        }

        public void SetData(string key, string level)
        {
            Key = key;
            _level = int.Parse(level);
        }

        public void SetWaveData(BattleGenWaveData data, int index)
        {
            if (index <= _waveDataArray.Length)
            {
                _waveDataArray[index] = data.Clone();
            }
        }

        public override void SetData(string[] arr)
        {
            Key = arr[(int)BattleGenGenerator.TYPE_SHEET_LEVEL_COLUMNS.Key];
            _level = int.Parse(arr[(int)BattleGenGenerator.TYPE_SHEET_LEVEL_COLUMNS.Level]);

            _waveDataKeys = new string[5];
            for (int i = 0; i < _waveDataKeys.Length; i++)
            {
                _waveDataKeys[i] = arr[(int)BattleGenGenerator.TYPE_SHEET_LEVEL_COLUMNS.Wave1 + i];
            }

            _waveDataArray = new BattleGenWaveData[_waveDataKeys.Length];
        }

        public override void AddData(string[] arr)
        {
        }

        public override bool HasDataArray() => false;
#endif
    }
}