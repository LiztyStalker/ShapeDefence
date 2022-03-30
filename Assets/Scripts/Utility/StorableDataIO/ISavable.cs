namespace Utility.IO
{
    public interface ISavable
    {
        public string SavableKey();
        public SavableData GetSavableData();
        public void SetSavableData(SavableData data);
    }
}