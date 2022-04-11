namespace Storage
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using System.IO;
    using UtilityManager;
    using Utility.ScriptableObjectData;

    [ExecuteAlways]
    public class DataStorage
    {
        [ExecuteAlways]
        private static DataStorage _instance = null;

        public static DataStorage Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DataStorage();
                return _instance;
            }
        }

        private Dictionary<string, Dictionary<string, Object>> _dataDic = new Dictionary<string, Dictionary<string, Object>>();
        private DataStorage()
        {
            InitializeDatasFromAssetDatabaseDirectory<Sprite>("Images");
            InitializeDatasFromAssetDatabaseDirectory<GameObject>("Prefabs");
            InitializeDatasFromAssetDatabaseDirectory<ScriptableObject>("Data");
            InitializeDatasFromAssetDatabaseDirectory<AudioClip>("Sounds");
            //#if UNITY_EDITOR
            //            InitializeDatasFromAssetDatabase<Sprite>("Images/Icons/Assets");
            //            InitializeDatasFromAssetDatabase<SkeletonDataAsset>("Data/Spine");
            //            InitializeDatasFromAssetDatabase<BulletData>("Data/Bullets");
            //            InitializeDatasFromAssetDatabase<EnemyData>("Data/Enemies");
            //            InitializeDatasFromAssetDatabase<UnitData>("Data/Units");
            //            InitializeDatasFromAssetDatabase<MineData>("Data/Mines");
            //            InitializeDatasFromAssetDatabase<SmithyData>("Data/Smithy");
            //            InitializeDatasFromAssetDatabase<VillageData>("Data/Villages");
            //            InitializeDatasFromAssetDatabase<QuestData>("Data/Quests/Daily");
            //            InitializeDatasFromAssetDatabase<QuestData>("Data/Quests/Weekly");
            //            InitializeDatasFromAssetDatabase<QuestData>("Data/Quests/Goal");
            //            InitializeDatasFromAssetDatabase<QuestData>("Data/Quests/Challenge");
            //            InitializeDatasFromAssetDatabase<GameObject>("Prefabs/UI");
            //            InitializeDatasFromAssetDatabase<TextAsset>("TextAssets");
            //            InitializeDatasFromAssetDatabase<GameLanguageData>("Data/GameLanguage");
            //#else
            //            InitializeDataFromAssetBundle<Sprite>("sprites", null);
            //            InitializeDataFromAssetBundle<SkeletonDataAsset>("spines", null);
            //            InitializeDataFromAssetBundle<BulletData>("bullets", "data");
            //            InitializeDataFromAssetBundle<EnemyData>("enemies", "data");
            //            InitializeDataFromAssetBundle<UnitData>("units", "data");
            //            InitializeDataFromAssetBundle<MineData>("mines", "data");
            //            InitializeDataFromAssetBundle<SmithyData>("smithies", "data");
            //            InitializeDataFromAssetBundle<VillageData>("villages", "data");
            //            InitializeDataFromAssetBundle<QuestData>("daily", "data/quests");
            //            InitializeDataFromAssetBundle<QuestData>("weekly", "data/quests");
            //            InitializeDataFromAssetBundle<QuestData>("goal", "data/quests");
            //            InitializeDataFromAssetBundle<QuestData>("challenge", "data/quests");
            //            InitializeDataFromAssetBundle<GameObject>("ui", null);
            //            InitializeDataFromAssetBundle<TextAsset>("textassets", null);
            //            InitializeDataFromAssetBundle<GameLanguageData>("language", "data");
            //#endif
        }



#if UNITY_EDITOR || UNITY_INCLUDE_TESTS

        public static void Initialize(System.Action<float> loadCallback, System.Action<Utility.IO.TYPE_IO_RESULT> endCallback)
        {
            var loader = DataLoader.Create();
            loader.LoadTest(loadCallback, result => 
            {
                endCallback?.Invoke(result);
                loader.Dispose();
            });
        }
#else
        public static void Initialize(System.Action<float> loadCallback, System.Action<Utility.IO.TYPE_IO_RESULT> endCallback)
        {
            var loader = DataLoader.Create();
            loader.Load(loadCallback, result => 
            {
                endCallback?.Invoke(result);
                loader.Dispose();
            });
        }
#endif

        public static void Dispose()
        {
            _instance = null;
        }

#if UNITY_EDITOR
        private void InitializeDatasFromAssetDatabase(string key, string path)
        {
            var files = System.IO.Directory.GetFiles($"Assets/{path}");
            for (int j = 0; j < files.Length; j++)
            {
                var data = AssetDatabase.LoadAssetAtPath<Object>(files[j]);
                Debug.Log(data + " " + files[j]);
                if (data != null)
                {
                    AddDirectoryInData(key, data.name, data);
                }
            }

            Debug.Log($"{key} : {GetDataCount(key)}");
        }


        /// <summary>
        /// AssetDatabase로 해당 패스 데이터 가져오기
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        private void InitializeDatasFromAssetDatabase<T>(string path) where T : Object
        {
            var files = System.IO.Directory.GetFiles($"Assets/{path}");
            for (int j = 0; j < files.Length; j++)
            {
                var data = AssetDatabase.LoadAssetAtPath<T>(files[j]);
                Debug.Log(data + " " + files[j]);
                if (data != null)
                {
                    AddDirectoryInData(data.name, data);
                }
            }

            Debug.Log($"{typeof(T)} : {GetDataCount<T>()}");
        }
        private void InitializeDatasFromAssetDatabaseDirectory<T>(string path) where T : Object
        {
            var files = System.IO.Directory.GetFiles($"Assets/{path}");
            for (int i = 0; i < files.Length; i++)
            {
                var data = AssetDatabase.LoadAssetAtPath<T>(files[i]);
                if (data != null)
                {
                    Debug.Log($"InitializeDatasFromAssetDatabaseDirectory Data - {data.name}");
                    AddDirectoryInData(data.name, data);
                }
            }

            var directories = System.IO.Directory.GetDirectories($"Assets/{path}");
            for (int i = 0; i < directories.Length; i++)
            {
                Debug.Log($"InitializeDatasFromAssetDatabaseDirectory Directory - {directories[i]}");
                if (Directory.Exists(directories[i]))
                {
                    InitializeDatasFromAssetDatabaseDirectory<T>(directories[i].Replace("Assets/", ""));
                }
            }



            Debug.Log($"DataStorage Loaded - {typeof(T)} : {GetDataCount<T>()}");
        }

        private void InitializeDataFromAssetDatabase<T>(string path, string name) where T : Object
        {
            var data = AssetDatabase.LoadAssetAtPath<T>(path + "/" + name);
            AddDirectoryInData(name, data);
        }


#endif


        private void InitializeDataFromAssetBundle<T>(string path, string directory = null) where T : Object
        {
            string bundlePath = path;
            if (directory != null)
            {
                bundlePath = Path.Combine(directory, path);
            }

#if UNITY_EDITOR
            string dataPath = Application.streamingAssetsPath;
#else
            string dataPath = "jar:file://" + Application.dataPath + "!/assets";
#endif

            try
            {

                var assetbundle = AssetBundle.LoadFromFile(Path.Combine(dataPath, bundlePath));
                if (assetbundle == null)
                {
                    Debug.LogError($"{bundlePath} AssetBundle을 찾을 수 없습니다");
                    return;
                }

                var files = assetbundle.LoadAllAssets<T>();
                for (int i = 0; i < files.Length; i++)
                {
                    var data = files[i];
                    Debug.Log(files[i]);
                    if (data != null)
                    {
                        AddDirectoryInData(data.name, data);
                    }
                }
                assetbundle.Unload(false);
            }

            catch
            {
#if UNITY_EDITOR
                Debug.LogWarning($"{bundlePath} 가 존재하지 않습니다");
#endif
            }
        }


        /// <summary>
        /// 데이터 초기화
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        private void InitializeDataFromAssetBundle<T>(string directory = null) where T : Object
        {
            InitializeDataFromAssetBundle<T>(typeof(T).Name.ToLower(), directory);
        }



#if UNITY_EDITOR
        public T[] GetDataArrayFromAssetDatabase<T>(string path) where T : Object
        {
            var list = new List<T>();
            var files = System.IO.Directory.GetFiles($"Assets/{path}");
            for (int j = 0; j < files.Length; j++)
            {
                var data = AssetDatabase.LoadAssetAtPath<T>(files[j]);
                //Debug.Log(files[j]);
                if (data != null)
                {
                    list.Add(data);
                }
            }
            return list.ToArray();
        }

        public T GetDataFromAssetDatabase<T>(string path) where T : Object
        {
            var data = AssetDatabase.LoadAssetAtPath<T>($"Assets/{path}");
            return data;

        }
#endif

        /// <summary>
        /// 모든 데이터를 가져옵니다
        /// 없으면 0
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T[] GetAllDataArrayOrZero<T>() where T : Object
        {
            List<T> list = new List<T>();
            if (IsHasDataType<T>())
            {
                foreach (var data in _dataDic[ToTypeString<T>()].Values)
                {
                    list.Add((T)data);
                }
            }
            return list.ToArray();
        }

        public static string ToTypeString<T>() => typeof(T).Name.ToString();

        /// <summary>
        /// 데이터 가져오기
        /// 없으면 null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetDataOrNull<T>(string key, string firstVerb = null, string lastVerb = null) where T : Object
        {
            if (IsHasDataType<T>())
            {
                var dic = _dataDic[ToTypeString<T>()];
                var cKey = GetConvertKey(key, firstVerb, lastVerb);
                //#if UNITY_EDITOR
                //                Debug.Log(ToTypeString<T>() + " " + cKey);
                //#endif
                //Debug.Log(cKey);
                return GetDataOrNull<T>(dic, cKey);
            }
            return null;
        }

        /// <summary>
        /// 첫 데이터 가져오기
        /// 없으면 null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="firstVerb"></param>
        /// <param name="lastVerb"></param>
        /// <returns></returns>
        public T GetFirstDataOrNull<T>() where T : Object
        {
            var arr = GetAllDataArrayOrZero<T>();
            if (arr.Length > 0)
                return arr[0];
            return null;
        }

        /// <summary>
        /// 마지막 데이터 가져오기
        /// 없으면 null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="firstVerb"></param>
        /// <param name="lastVerb"></param>
        /// <returns></returns>
        public T GetLastDataOrNull<T>() where T : Object
        {
            var arr = GetAllDataArrayOrZero<T>();
            if (arr.Length > 0)
                return arr[arr.Length - 1];
            return null;
        }



        /// <summary>
        /// 데이터리스트 가져오기 
        /// ex) GetDataArrayOrZero<UnitData>() => return [UnitData_Data]
        /// 없으면 0
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        public T[] GetDataArrayOrZero<T>(string[] keys) where T : Object => GetDataArrayOrZero<T>(keys, ToTypeString<T>(), null);

        /// <summary>
        /// 데이터 리스트 가져오기
        /// 없으면 0
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        public T[] GetDataArrayOrZero<T>(string[] keys, string firstVerb, string lastVerb) where T : Object
        {
            List<T> list = new List<T>();
            if (keys != null && keys.Length > 0)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    if (IsHasDataType<T>())
                    {
                        var dic = _dataDic[ToTypeString<T>()];
                        if (dic.ContainsKey(GetConvertKey(keys[i], ToTypeString<T>())))
                        {
                            var cKey = GetConvertKey(keys[i], firstVerb, lastVerb);
                            var data = GetDataOrNull<T>(dic, cKey);
                            if (data != null)
                            {
                                list.Add(data);
                            }
                        }
                    }
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 데이터 랜덤 가져오기
        /// 없으면 0
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        /// <returns></returns>
        public T[] GetRandomDatasOrZero<T>(int count) where T : Object
        {
            if (count <= 0)
            {
                Debug.LogWarning($"가져올 수량은 0 이하가 될 수 없습니다. 1로 수정된 후 진행합니다");
                count = 1;
            }

            var dataArray = GetAllDataArrayOrZero<T>();
            var list = new List<T>();
            for (int i = 0; i < count; i++)
            {
                list.Add(dataArray[Random.Range(0, dataArray.Length)]);
            }
            return list.ToArray();
        }



        /// <summary>
        /// 데이터가 있는지 확인
        /// 있으면 true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsHasData<T>(string key) where T : Object => IsHasData<T>(key, ToTypeString<T>(), null);
        public bool IsHasData(string key, string name) => IsHasData(name, key, null, null);


        /// <summary>
        /// 데이터가 있는지 확인
        /// 있으면 true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsHasData<T>(string key, string frontVerb, string lastVerb) where T : Object
        {
            if (IsHasDataType<T>())
            {
                var dic = _dataDic[ToTypeString<T>()];
                var cKey = GetConvertKey(key, frontVerb, lastVerb);
                return dic.ContainsKey(cKey);
            }
            return false;
        }

        public bool IsHasData(string key, string name, string frontVerb, string lastVerb)
        {
            if (IsHasDataType(key))
            {
                var dic = _dataDic[key];
                var cKey = GetConvertKey(name, frontVerb, lastVerb);
                return dic.ContainsKey(cKey);
            }
            return false;
        }


        private int GetDataCount(string key)
        {
            if (IsHasDataType(key))
            {
                return _dataDic[key].Count;
            }
            return 0;
        }


        private int GetDataCount<T>() where T : Object
        {
            if (IsHasDataType<T>())
            {
                return _dataDic[ToTypeString<T>()].Count;
            }
            return 0;
        }

        private bool IsHasDataType(string key) => _dataDic.ContainsKey(key);
        private bool IsHasDataType<T>() where T : Object => _dataDic.ContainsKey(ToTypeString<T>());
        private T GetDataOrNull<T>(Dictionary<string, Object> dic, string key) where T : Object
        {
            if (dic.ContainsKey(key))
            {
                //Debug.Log("GetDataOrNull " + key);
                //Debug.Log("GetDataOrNull " + (T)dic[key]);
                return (T)dic[key];
            }
            return null;
        }


        private void AddDirectoryInData<T>(string key, T data) where T : Object
        {
            if (!IsHasDataType<T>())
                _dataDic.Add(ToTypeString<T>(), new Dictionary<string, Object>());

            if (!IsHasData<T>(key))
                _dataDic[ToTypeString<T>()].Add(key, data);
        }

        private void AddDirectoryInData(string key, string name, Object data)
        {
            if (!IsHasDataType(key))
                _dataDic.Add(key, new Dictionary<string, Object>());

            if (!IsHasData(key, name))
                _dataDic[key].Add(name, data);
        }


        private string GetConvertKey(string key, string frontVerb = null, string backVerb = null)
        {
            if (frontVerb != null) frontVerb += "_";
            if (backVerb != null) backVerb = "_" + backVerb;
            return $"{frontVerb}{key}{backVerb}";
        }



#if UNITY_EDITOR
        public static T LoadAssetAtPath<T>(string path) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
#endif
    }
}