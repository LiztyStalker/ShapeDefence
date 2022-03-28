namespace Utility.IO 
{
    [System.Serializable]
    public class StorableData
    {
        [UnityEngine.SerializeField] private StorableData[] _children = null;
        public StorableData[] Children { get => _children; protected set => _children = value; }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}