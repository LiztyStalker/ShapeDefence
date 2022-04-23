namespace Storage
{
    using UnityEngine;
    using LitJson;
    using System.Collections.Generic;
    using UtilityManager;

    public class TranslateStorage
    {
        

        private readonly string SETTINGS_LANGUAGE_KEY = "SETTINGS_LANGUAGE_KEY";
        private readonly string DEFAULT_LANGUAGE_KEY = "Korean";

        private static TranslateStorage _instance;

        private Dictionary<string, JsonData> _dic;

        private TranslateLanguageData _languageData;

        private string _languageKey;

        public static TranslateStorage Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new TranslateStorage();
                }
                return _instance;
            }
        }

        private TranslateStorage() 
        {
            _dic = new Dictionary<string, JsonData>();
            var arr = DataStorage.Instance.GetAllDataArrayOrZero<TextAsset>();

            for(int i = 0; i < arr.Length; i++)
            {
                _dic.Add(arr[i].name, JsonMapper.ToObject(arr[i].text));
            }

            var obj = DataStorage.Instance.GetDataOrNull<ScriptableObject>("TranslateLanguageData_Language");
            _languageData = (TranslateLanguageData)obj;
            _languageKey = DEFAULT_LANGUAGE_KEY;

            //System 언어 적용하기

            Load();
        }

        public TranslateLanguageData GetLanguages() => _languageData;

        public string NowLanguage() => _languageKey;

        public void SetLanguage(string key)
        {
            _languageKey = key;
            OnChangedTranslateEvent();
        }

        public void ChangedLanguage()
        {
            OnChangedTranslateEvent();
        }

        public void Load() 
        {
            var languageKey = PlayerPrefs.GetString(SETTINGS_LANGUAGE_KEY, DEFAULT_LANGUAGE_KEY);

            if (!_languageData.HasKey(languageKey))
            {
                _languageKey = DEFAULT_LANGUAGE_KEY;
            }
            _languageKey = languageKey;

            OnChangedTranslateEvent();
        }
        public void Save() 
        {
            PlayerPrefs.SetString(SETTINGS_LANGUAGE_KEY, _languageKey);
        }

        public string GetTranslateData(string title, string key, string verb = null, int index = 0)
        {
            if (_dic.ContainsKey(title))
            {
                //Debug.Log(title);
                var dicTitle = _dic[title];
                if (dicTitle.ContainsKey(key))
                {
                    //Debug.Log(key);
                    var dicKey = dicTitle[key];
                    if (dicKey.IsArray)
                    {
                        //Debug.Log(index);
                        var dicValues = dicKey[index]["values"];

                        var langVerb = NowLanguage().ToString() + ((!string.IsNullOrEmpty(verb)) ? $"_{verb}" : null);

                        //아직 언어 적용 되어있지 않음
                        //verb = "Language_Verb" Gamesettings - CurrentLanguage
                        if (dicValues.ContainsKey(langVerb))
                        {
                            //Debug.Log(verb);
                            return dicValues[langVerb].ToString();
                        }
                    }
                }
            }
#if UNITY_EDITOR
            return "-";
#else
            return null;
#endif
        }
        public static void Dispose()
        {
            _instance = null;
        }


        #region ##### Listener #####

        private System.Action _changedTranslateEvent;
        public void AddOnChangedTranslateListener(System.Action act) => _changedTranslateEvent += act;
        public void RemoveOnChangedTranslateListener(System.Action act) => _changedTranslateEvent -= act;
        private void OnChangedTranslateEvent()
        {
            _changedTranslateEvent?.Invoke();
        }

        #endregion
    }
}