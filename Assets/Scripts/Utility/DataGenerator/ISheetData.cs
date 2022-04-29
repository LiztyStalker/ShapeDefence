namespace Utility
{
    public interface ISheetData
    {
#if UNITY_EDITOR
        public void SetData(string[] arr);
        public void AddData(string[] arr);
#endif
    }
}