namespace SDefence.Asset
{
    using Utility.IO;

    public interface IAssetUsableData : ISavable
    {
        public bool IsZero { get; }
        public void Add(int value);
        public void Add(IAssetUsableData value);
        public void Subject(int value);
        public void Subject(IAssetUsableData value);
        public void Set(int value);
        public void Set(IAssetUsableData value);
        public void SetZero();
        public void SetData(string startValue, string increaseValue, string increaseRate, int upgrade);
        public string ToString(string format);
        public int Compare(IAssetUsableData value);
        public System.Type GetType();
        public IAssetUsableData Clone();     

    }
}