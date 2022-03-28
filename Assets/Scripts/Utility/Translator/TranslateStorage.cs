namespace Storage
{
    using UnityEngine;
    using LitJson;
    using System.Collections.Generic;

    public class TranslateStorage
    {
        private readonly string SETTINGS_LANGUAGE_KEY = "LANGUAGE_KEY";



        private static TranslateStorage _instance;

        private Dictionary<string, JsonData> _dic;

        private UtilityManager.GameLanguageData _gameLangData;

        private int _languageIndex = 0;

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

            _gameLangData = DataStorage.Instance.GetDataOrNull<UtilityManager.GameLanguageData>("GameLanguageData", null, null);

            Load();
        }
        public SystemLanguage NowLanguage()
        {
            //Debug.Log(_languageIndex);
            return _gameLangData.UsableLanguages[_languageIndex];
        }
        public void PrevLanguageIndex()
        {
            if(_languageIndex - 1 < 0)
            {
                _languageIndex = _gameLangData.UsableLanguages.Length - 1;
            }
            else
            {
                _languageIndex--;
            }
            OnChangedTranslateEvent();
        }

        public void NextLanguageIndex()
        {
            if (_languageIndex + 1 >= _gameLangData.UsableLanguages.Length)
            {
                _languageIndex = 0;
            }
            else
            {
                _languageIndex++;
            }
            OnChangedTranslateEvent();
        }

        public void ChangedLanguage()
        {
            OnChangedTranslateEvent();
        }

        public void Load() 
        {
            var language = PlayerPrefs.GetString(SETTINGS_LANGUAGE_KEY, SystemLanguage.Korean.ToString());
            for(int i = 0; i < _gameLangData.UsableLanguages.Length; i++)
            {
                if(_gameLangData.UsableLanguages[i].ToString() == language)
                {
                    _languageIndex = i;
                    return;
                }
            }
            _languageIndex = 0;

        }
        public void Save() 
        {
            PlayerPrefs.SetString(SETTINGS_LANGUAGE_KEY, _gameLangData.UsableLanguages[_languageIndex].ToString());
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