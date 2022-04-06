
namespace SDefence.Durable.Usable
{
    using System.Numerics;
    public class DurableUsableCase : IDurableUsableData
    {
        private IDurableUsableData _maxDurableUsableData;
        private IDurableUsableData _nowDurableUsableData;

        public BigDecimal Value => _nowDurableUsableData.Value;
        public new System.Type GetType() => _nowDurableUsableData.GetType();

        public bool IsZero => _nowDurableUsableData.IsZero;
        public void SetZero() => _nowDurableUsableData.SetZero();


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
        public void Add(int value) => _nowDurableUsableData.Add(value);

        public void Add(UniversalUsableData dData)
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


        public void Subject(int value) => _nowDurableUsableData.Subject(value);

        public void Subject(UniversalUsableData dData)
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
            var dData = new UniversalUsableData(value);
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
                _nowDurableUsableData.Set(value);
            }

        }

        public void Set(IDurableUsableData dData)
        {
            if (_nowDurableUsableData.IsOverflowMaxValue(_maxDurableUsableData, dData.CreateUniversalUsableData()))
            {
                _nowDurableUsableData.Set(_maxDurableUsableData.Clone());
            }
            else if (_maxDurableUsableData.IsUnderflowZero(dData.CreateUniversalUsableData()))
            {
                _nowDurableUsableData.SetZero();
            }
            else
            {
                _nowDurableUsableData.Set(dData);
            }
        }

      
        public float GetRate() => (float)(_nowDurableUsableData.Value / _maxDurableUsableData.Value);


        public void SetData(string startValue, string increaseValue, string increaseRate, int upgrade) => _nowDurableUsableData.SetData(startValue, increaseValue, increaseRate, upgrade);

        public string ToString(string format) => $"{_nowDurableUsableData.ToString(format)} / {_maxDurableUsableData.ToString(format)}";

        public IDurableUsableData Clone()
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning($"{GetType().Name} 사용하지 않음");
#endif
            return null;
        }


        public bool IsOverflowMaxValue(IDurableUsableData maxValue, UniversalUsableData value) => _nowDurableUsableData.IsOverflowMaxValue(maxValue, value);

        public bool IsUnderflowZero(UniversalUsableData value) => _nowDurableUsableData.IsUnderflowZero(value);

        public int Compare(UniversalUsableData value) => _nowDurableUsableData.Compare(value);

        public UniversalUsableData CreateUniversalUsableData() => _nowDurableUsableData.CreateUniversalUsableData();
    }
}