
namespace UtilityManager
{
    using System.Collections.Generic;
    using UnityEngine;
    using Utility.ScriptableObjectData;


    [System.Serializable]
    public struct TranslateLanguageElement
    {
        public string Key;
        public string IconKey;
        public string Value;
    }

    
    public class TranslateLanguageData : ScriptableObjectData
    {
        [SerializeField]
        private TranslateLanguageElement[] _usableLanguages;      
        public TranslateLanguageElement[] UsableLanguages => _usableLanguages;



        public bool HasKey(string key)
        {
            return (GetIndex(key) < 0);
        }

        /// <summary>
        /// 0 - n : index
        /// -1 : fail
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetIndex(string key)
        {
            for(int i = 0; i < _usableLanguages.Length; i++)
            {
                if (_usableLanguages[i].Key == key) return i;
            }
            return -1;
        }

        public TranslateLanguageElement? GetLanguageElement(string key)
        {
            var index = GetIndex(key);
            if(index >= 0)
            {
                return _usableLanguages[index];
            }
            return null;
        }

        public override void AddData(string[] arr)
        {

            var list = new List<TranslateLanguageElement>(_usableLanguages);
            var data = new TranslateLanguageElement
            {
                Key = arr[(int)TranslateLanguageGenerator.TYPE_SHEET_COLUMNS.Key],
                IconKey = arr[(int)TranslateLanguageGenerator.TYPE_SHEET_COLUMNS.IconKey],
                Value = arr[(int)TranslateLanguageGenerator.TYPE_SHEET_COLUMNS.Text]
            };
            list.Add(data);
            _usableLanguages = list.ToArray();
        }

        public override bool HasDataArray() => true;

        public override void SetData(string[] arr)
        {
            Key = arr[(int)TranslateLanguageGenerator.TYPE_SHEET_COLUMNS.Group];

            _usableLanguages = new TranslateLanguageElement[1];
            var data = new TranslateLanguageElement
            {
                Key = arr[(int)TranslateLanguageGenerator.TYPE_SHEET_COLUMNS.Key],
                IconKey = arr[(int)TranslateLanguageGenerator.TYPE_SHEET_COLUMNS.IconKey],
                Value = arr[(int)TranslateLanguageGenerator.TYPE_SHEET_COLUMNS.Text]
            };
            _usableLanguages[0] = data;
        }
    }
}