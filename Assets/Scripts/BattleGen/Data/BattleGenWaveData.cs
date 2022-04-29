namespace SDefence.BattleGen.Data
{
    using System.Collections.Generic;
    using UnityEngine;
    using Utility;
#if UNITY_EDITOR
    using Generator;
#endif

    [System.Serializable]
    public class BattleGenWaveData : ISheetData
    {
        [SerializeField]
        private BattleGenWaveElement[] _waveElementArray;

        public bool IsOverflow(int index)
        {
            return index >= _waveElementArray.Length;
        }

        public bool HasWaveData(int index, float nowTime)
        {
            if (IsOverflow(index)) return false;
            return (nowTime >= _waveElementArray[index].WaveAppearDelay);
        }

        public BattleGenWaveElement GetBattleGenWaveElement(int index)
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

        private BattleGenWaveData(BattleGenWaveData data)
        {
            _waveElementArray = new BattleGenWaveElement[data._waveElementArray.Length];
            for(int i = 0; i < _waveElementArray.Length; i++)
            {
                _waveElementArray[i] = data._waveElementArray[i].Clone();
            }
        }

        public BattleGenWaveData Clone()
        {
            return new BattleGenWaveData(this);
        }

        public BattleGenWaveData() { }

        public void SetData(BattleGenWaveElement[] elements)
        {
            _waveElementArray = elements;
        }

        public void SetData(BattleGenWaveElement element)
        {
            _waveElementArray = new BattleGenWaveElement[1];
            _waveElementArray[0] = element;
        }

        public void SetData(string[] arr)
        {
            var element = BattleGenWaveElement.Create();
            element.SetData(
                arr[(int)BattleGenGenerator.TYPE_SHEET_WAVE_COLUMNS.EnemyDataKey],
                arr[(int)BattleGenGenerator.TYPE_SHEET_WAVE_COLUMNS.AppearCount],
                arr[(int)BattleGenGenerator.TYPE_SHEET_WAVE_COLUMNS.Weight],
                arr[(int)BattleGenGenerator.TYPE_SHEET_WAVE_COLUMNS.WaveAppearDelay]
                );

            _waveElementArray = new BattleGenWaveElement[1];
            _waveElementArray[0] = element;
        }

        public void AddData(string[] arr)
        {
            var list = new List<BattleGenWaveElement>(_waveElementArray);
            var element = BattleGenWaveElement.Create();
            element.SetData(
                arr[(int)BattleGenGenerator.TYPE_SHEET_WAVE_COLUMNS.EnemyDataKey],
                arr[(int)BattleGenGenerator.TYPE_SHEET_WAVE_COLUMNS.AppearCount],
                arr[(int)BattleGenGenerator.TYPE_SHEET_WAVE_COLUMNS.Weight],
                arr[(int)BattleGenGenerator.TYPE_SHEET_WAVE_COLUMNS.WaveAppearDelay]
                );
            list.Add(element);
            _waveElementArray = list.ToArray();
        }
#endif
    }
}