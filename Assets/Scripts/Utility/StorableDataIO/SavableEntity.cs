namespace Utility.IO
{
    public class SavableEntity
    {
        private SavableData _savableData;
        public SavableData GetSavableData() => _savableData;

        public void SetSavableData(SavableData data) => _savableData = data;

        public void Save(System.Action<TYPE_IO_RESULT> resultCallback) => SavableDataIO.Current.SaveFileData(_savableData, "Test", resultCallback);

        public void Load(System.Action<float> processCallback, System.Action<TYPE_IO_RESULT> endCallback)
        {
            SavableDataIO.Current.LoadFileData("Test", processCallback, (typeResult, result) =>
            {
                if (result != null) _savableData = (SavableData)result;
                endCallback?.Invoke(typeResult);
            });

        }
    }
}