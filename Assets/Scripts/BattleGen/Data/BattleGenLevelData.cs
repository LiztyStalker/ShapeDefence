namespace SDefence.BattleGen.Data
{
    using SDefence.Data;
    using UnityEngine;

    [System.Serializable]
    public class BattleGenLevelData : ScriptableObject
    {
        [SerializeField]
        private string _key;

        [SerializeField]
        private int _level;

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
        public static BattleGenLevelData Create_Test() => new BattleGenLevelData();

        private BattleGenLevelData()
        {
            _key = "Test";
            _level = 0;
            _waveDataArray = new BattleGenWaveData[5];
            for (int i = 0; i < _waveDataArray.Length; i++)
            {
                _waveDataArray[i] = BattleGenWaveData.Create_Test();
            }
        }
#endif
    }
}