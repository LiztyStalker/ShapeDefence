namespace Utility.IO
{
    public class SavablePackage
    {
        public static SavablePackage _current;

        public static SavablePackage Current
        {
            get
            {
                if(_current == null)
                {
                    _current = new SavablePackage();
                }
                return _current;
            }
        }
        public static void Dispose() => _current = null;



        private SavableData _savableData;
        public SavableData GetSavableData() => _savableData;

        public void SetSavableData(SavableData data) => _savableData = data;

        public void Save(System.Action<TYPE_IO_RESULT> resultCallback)
        {
            SavableDataIO.Current.SaveFileData_NotCrypto(_savableData, "Test", resultCallback);
            //SavableDataIO.Current.SaveFileData(_savableData, "Test", resultCallback);
        }

        public void Load(System.Action<float> processCallback, System.Action<TYPE_IO_RESULT> endCallback)
        {
            SavableDataIO.Current.LoadFileData_NotCrypto("Test", processCallback, (typeResult, result) =>
            {
                if (result != null) _savableData = (SavableData)result;
                endCallback?.Invoke(typeResult);
            });


            //SavableDataIO.Current.LoadFileData("Test", processCallback, (typeResult, result) =>
            //{
            //    if (result != null) _savableData = (SavableData)result;
            //    endCallback?.Invoke(typeResult);
            //});

        }
    }
}