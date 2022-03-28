namespace SDefence.Durable.Usable
{
    public class DurableUsableCase : IDurableUsableData
    {
        private IDurableUsableData _maxDurableUsableData;
        private IDurableUsableData _nowDurableUsableData;

        public bool IsZero => _nowDurableUsableData.IsZero;
        public new System.Type GetType() => _nowDurableUsableData.GetType();

        public IDurableUsableData NowDurableUsableData => _nowDurableUsableData;

        public static DurableUsableCase Create(IDurableUsableData usableData) => new DurableUsableCase(usableData);

        private DurableUsableCase(IDurableUsableData usableData)
        {
            _maxDurableUsableData = usableData.Clone();
            _nowDurableUsableData = usableData.Clone();
        }

        public void CleanUp()
        {
            _maxDurableUsableData = null;
            _nowDurableUsableData = null;
        }

        public void Add(IDurableUsableData dData)
        {
            if (_nowDurableUsableData.IsOverflowMaxValue(_maxDurableUsableData, dData))
            {
                _nowDurableUsableData.Set(_maxDurableUsableData.Clone());
            }
            else
            {
                _nowDurableUsableData.Add(dData);
            }
        }

        public void Subject(IDurableUsableData dData)
        {
            if (_nowDurableUsableData.IsUnderflowZero(dData))
            {
                _nowDurableUsableData.SetZero();
            }
            else
            {
                _nowDurableUsableData.Subject(dData);
            }
        }

        public void Set(int value)
        {
            var dData = UniversalDurableUsableData.Create(value);
            if (_nowDurableUsableData.IsOverflowMaxValue(_maxDurableUsableData, dData))
            {
                _nowDurableUsableData.Set(_maxDurableUsableData.Clone());
            }
            else if (_maxDurableUsableData.IsUnderflowZero(dData))
            {
                _nowDurableUsableData.SetZero();
            }
            else
            {
                _nowDurableUsableData.Set(dData);
            }

        }

        public void Set(IDurableUsableData dData)
        {
            if (_nowDurableUsableData.IsOverflowMaxValue(_maxDurableUsableData, dData))
            {
                _nowDurableUsableData.Set(_maxDurableUsableData.Clone());
            }
            else if (_maxDurableUsableData.IsUnderflowZero(dData))
            {
                _nowDurableUsableData.SetZero();
            }
            else
            {
                _nowDurableUsableData.Set(dData);
            }
        }

        public void Add(int value)
        {
            _nowDurableUsableData.Add(value);
        }

        public void Subject(int value)
        {
            _nowDurableUsableData.Subject(value);
        }

        public void SetData(string startValue, string increaseValue, string increaseRate, int upgrade) => _nowDurableUsableData.SetData(startValue, increaseValue, increaseRate, upgrade);

        public string ToString(string format) => $"{_nowDurableUsableData.ToString(format)} / {_maxDurableUsableData.ToString(format)}";

        public IDurableUsableData Clone()
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning($"{GetType().Name} 사용하지 않음");
#endif
            return null;
        }

        public void SetZero() => _nowDurableUsableData.SetZero();

        public bool IsOverflowMaxValue(IDurableUsableData maxValue, IDurableUsableData value) => _nowDurableUsableData.IsOverflowMaxValue(maxValue, value);
        public bool IsUnderflowZero(IDurableUsableData value) => _nowDurableUsableData.IsUnderflowZero(value);
        public int Compare(IDurableUsableData value) => _nowDurableUsableData.Compare(value);
    }
}