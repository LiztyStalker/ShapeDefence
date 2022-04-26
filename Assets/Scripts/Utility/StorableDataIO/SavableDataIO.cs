namespace Utility.IO
{
    //https://ubuntuanakramli.blogspot.com/2015/04/c-encrypt-decrypt-serialized-object.html?m=1
    using UnityEngine;
    using System;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Runtime.Serialization;
    using System.IO;
    using System.Security.Cryptography;
    using System.Xml.Serialization;

    public enum TYPE_IO_RESULT
    {
        InProgress = 0,
        Success = 1,
        ConnectionError = 2,
        ProtocolError = 3,
        DataProcessingError = 4
    };



    public class SavableDataIO
    {
        private readonly string FILE_EXTENTION = "txt";

        private static SavableDataIO _current;

        public static SavableDataIO Current
        {
            get
            {
                if (_current == null)
                    _current = new SavableDataIO();
                return _current;
            }
        }

        public static void Dispose()
        {
            _current = null;
        }

        private readonly string FilePath = Application.persistentDataPath;

        private ICryptoTransform _encryptor;
        private ICryptoTransform _decryptor;


        private SavableDataIO()
        {            
            //암호제공 제작
            AssembleCryptoServiceProvider();
        }


        private void AssembleCryptoServiceProvider()
        {
            var des = new DESCryptoServiceProvider();
            des.Padding = PaddingMode.PKCS7;
            des.Mode = CipherMode.CBC;
            des.Key = System.Text.Encoding.UTF8.GetBytes("12345678");
            des.IV = System.Text.Encoding.UTF8.GetBytes("12345678");
            //des.GenerateIV();
            //des.GenerateKey();

            _encryptor = des.CreateEncryptor();
            _decryptor = des.CreateDecryptor();
        }

        /// <summary>
        /// 파일의 유무 판별
        /// </summary>
        public bool isFile(string fileName) => File.Exists(string.Format("{0}/{1}.{2}", FilePath, fileName, FILE_EXTENTION));


        /// <summary>
        /// 저장 데이터 변환하기 
        /// Serial -> Byte
        /// </summary>
        public byte[] DataConvertSerialToByte(object storableData)
        {
            byte[] data = new byte[0];

            try
            {

                using (MemoryStream memory = new MemoryStream())
                {
                    IFormatter bf = new BinaryFormatter();
                    bf.Serialize(memory, storableData);
                    data = memory.ToArray();
                    memory.Close();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Serial -> Byte 변환 오류 : " + e.Message);
            }

            return data;
        }

        /// <summary>
        /// 저장 데이터 변환하기 
        /// Serial -> string
        /// </summary>
        /// <param name="storableData"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string DataConvertSerialToString(object storableData)
        {
            string data = "";

            try
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    IFormatter bf = new BinaryFormatter();
                    bf.Serialize(memory, storableData);
                    data = Convert.ToBase64String(memory.ToArray());
                    memory.Close();
                }
            }
            catch (Exception e)
            {
                //Prep.LogError("Serial -> string 변환 오류 : ", e.Message, GetType());
            }

            return data;
        }

        /// <summary>
        /// 저장된 데이터 변환하기
        /// string -> Serial 변환
        /// </summary>
        /// <param name="dataString"></param>
        /// <returns></returns>
        public SavableData DataConvertStringToSerial(string dataString)
        {
            try
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    byte[] data = Convert.FromBase64String(dataString);
                    Debug.Log("length : " + BitConverter.ToString(data));
                    memory.Write(data, 0, data.Length);
                    memory.Position = 0;


                    IFormatter bf = new BinaryFormatter();

                    string str = Convert.ToBase64String(memory.ToArray());
                    Debug.Log("convert : " + str);

                    SavableData accountData = (SavableData)bf.Deserialize(memory);

                    memory.Close();
                    return accountData;


                }

            }
            catch (Exception e)
            {
                //Prep.LogError("string -> Serial 변환 오류 : ", e.Message, GetType());

            }

            return null;
        }

        /// <summary>
        /// 저장된 데이터 변환하기
        /// Byte -> Serial
        /// </summary>
        /// <param name="dataByte"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public SavableData DataConvertByteToSerial(byte[] data)
        {
            try
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    memory.Write(data, 0, data.Length);
                    memory.Position = 0;

                    IFormatter bf = new BinaryFormatter();

                    string str = Convert.ToBase64String(memory.ToArray());
                    Debug.Log("convert : " + str);

                    SavableData accountData = (SavableData)bf.Deserialize(memory);

                    memory.Close();
                    return accountData;
                }
            }
            catch (Exception e)
            {
                //Prep.LogError("Byte -> Serial 변환 오류 : ", e.Message, GetType());
            }

            return null;
        }


        /// <summary>
        /// 마지막으로 저장한 시간 가져오기
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Nullable<DateTime> GetLastWriteTime(string fileName)
        {
            if (isFile(fileName))
            {
                return TimeZoneInfo.ConvertTimeToUtc(File.GetLastWriteTime(string.Format("{0}/{1}.{2}", FilePath, fileName, FILE_EXTENTION)));
            }
            return null;
        }


        #region ########## 파일 입출력 ##########

        /// <summary>
        /// 파일 입출력 저장 - Formatter
        /// 암호화 할당
        /// </summary>
        public void SaveFileData(object data, string fileName, System.Action<TYPE_IO_RESULT> endCallback)
        {
            //비동기 필요 async await
            try
            {
                using(MemoryStream mem = new MemoryStream())
                {
                    IFormatter bf = new BinaryFormatter();
                    bf.Serialize(mem, data);

                    mem.Seek(0, SeekOrigin.Begin);
                    
                    using (FileStream file = new FileStream(string.Format("{0}/{1}.{2}", FilePath, fileName, FILE_EXTENTION), FileMode.Create, FileAccess.Write))
                    {
                        byte[] arr = new byte[mem.Length];
                        mem.Read(arr, 0, arr.Length);

                        using (CryptoStream crypto = new CryptoStream(file, _encryptor, CryptoStreamMode.Write))
                        {
                            crypto.Write(arr, 0, arr.Length);
                            crypto.FlushFinalBlock();
                            crypto.Close();
                        }
                        file.Close();
                    }
                    mem.Flush();
                    mem.Close();
                }
                endCallback?.Invoke(TYPE_IO_RESULT.Success);
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.Log("저장 실패 " + e.Message);
#endif
                endCallback?.Invoke(TYPE_IO_RESULT.DataProcessingError);
            }
        }

        /// <summary>
        /// 파일 입출력 저장 - Formatter
        /// 비암호화
        /// </summary>
        public void SaveFileData_NotCrypto(object data, string fileName, System.Action<TYPE_IO_RESULT> endCallback)
        {
            try
            {
                using (FileStream file = new FileStream(string.Format("{0}/{1}.{2}", FilePath, fileName, FILE_EXTENTION), FileMode.Create, FileAccess.Write))
                {
                    IFormatter bf = new BinaryFormatter();
                    bf.Serialize(file, data);
                    file.Close();
                    endCallback?.Invoke(TYPE_IO_RESULT.Success);
                }
            }
            catch (Exception e)
            {
                endCallback?.Invoke(TYPE_IO_RESULT.DataProcessingError);
            }
        }



        /// <summary>
        /// 파일 입출력 불러오기 - Formatter
        /// 데이터 불러오기
        /// </summary>
        public void LoadFileData(string fileName, Action<float> processCallback, Action<TYPE_IO_RESULT, object> endCallback)
        {
            //비동기 필요 async await
            //파일 유무 판단
            if (isFile(fileName))
            {
                try
                {
                    object data = null;
                    using (FileStream file = new FileStream(string.Format("{0}/{1}.{2}", FilePath, fileName, FILE_EXTENTION), FileMode.Open, FileAccess.Read))
                    {

                        using (CryptoStream cryptoStream = new CryptoStream(file, _decryptor, CryptoStreamMode.Read))
                        {
                            byte[] arr = new byte[file.Length];
                            cryptoStream.Read(arr, 0, arr.Length);
                            using (MemoryStream mem = new MemoryStream())
                            {
                                mem.Write(arr, 0, arr.Length);
                                mem.Seek(0, SeekOrigin.Begin);

                                IFormatter bf = new BinaryFormatter();
                                data = bf.Deserialize(mem);
                                //Debug.Log(data);

                                mem.Flush();
                                mem.Close();
                            }
                            cryptoStream.Flush();
                            cryptoStream.Close();
                        }
                        file.Close();
                    }
                    endCallback?.Invoke(TYPE_IO_RESULT.Success, data);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogWarning("파일 입출력 불러오기 오류 : " + e.Message + " " + e.HelpLink);
                    endCallback?.Invoke(TYPE_IO_RESULT.DataProcessingError, null);
                }
            }
            else
            {
                Debug.LogWarning("파일을 찾을 수 없습니다");
                endCallback?.Invoke(TYPE_IO_RESULT.DataProcessingError, null);
            }
        }


        /// <summary>
        /// 파일 입출력 불러오기 - Formatter
        /// 데이터 불러오기
        /// </summary>
        public void LoadFileData_NotCrypto(string fileName, Action<float> processCallback, Action<TYPE_IO_RESULT, object> endCallback)
        {
            //파일 유무 판단
            if (isFile(fileName))
            {
                try
                {
                    using (FileStream file = new FileStream(string.Format("{0}/{1}.{2}", FilePath, fileName, FILE_EXTENTION), FileMode.Open, FileAccess.Read))
                    {
                        IFormatter bf = new BinaryFormatter();
                        var data = bf.Deserialize(file);
                        file.Close();
                        endCallback?.Invoke(TYPE_IO_RESULT.Success, data);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("파일 입출력 불러오기 오류 : " + e.Message + " " + GetType());
                    endCallback?.Invoke(TYPE_IO_RESULT.DataProcessingError, null);
                }
            }
            endCallback?.Invoke(TYPE_IO_RESULT.DataProcessingError, null);
        }

        #endregion

    }
    
}