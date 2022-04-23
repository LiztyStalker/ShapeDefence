namespace Utility.UI
{
    using Storage;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;

    public class UILanguageSheet : MonoBehaviour
    {
        [SerializeField]
        private Button _exitBtn;

        private ScrollRect _scrollRect;

        private UILanguageButton _uiBtn;

        private List<UILanguageButton> _list;

        public void Initialize()
        {
            _list = new List<UILanguageButton>();
            _scrollRect = GetComponentInChildren<ScrollRect>(true);

            var obj = DataStorage.Instance.GetDataOrNull<GameObject>("UI@LanguageButton");
            _uiBtn = obj.GetComponent<UILanguageButton>();

            _exitBtn.onClick.AddListener(Hide);
        }

        public void CleanUp()
        {
            _list.Clear();
            _exitBtn.onClick.RemoveListener(Hide);
        }

        public void Load()
        {
            TranslateStorage.Instance.Load();
        }

        public void Save()
        {
            TranslateStorage.Instance.Save();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            SetButtons();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void SetButtons()
        {            
            var languageData = TranslateStorage.Instance.GetLanguages();

            Debug.Log(languageData.UsableLanguages);

            if(languageData.UsableLanguages.Length >= _list.Count)
            {
                for (int i = 0; i < languageData.UsableLanguages.Length; i++)
                {
                    var element = languageData.UsableLanguages[i];
                    var btn = Create();
                    var key = element.Key;
                    var text = element.Value;
                    var icon = DataStorage.Instance.GetDataOrNull<Sprite>(element.IconKey, "Icon_Language");
                    btn.SetData(key, text);
                    btn.SetIcon(icon);
                    btn.SetActive(true);
                    _list.Add(btn);
                }
            }
        }


        private UILanguageButton Create()
        {
            var btn = Instantiate(_uiBtn);
            btn.transform.SetParent(_scrollRect.content);
            btn.transform.localScale = Vector3.one;
            btn.SetOnClickListener(OnClickLanguageEvent);
            _list.Add(btn);
            return btn;
        }

        #region ##### Listener #####

        private System.Action<string, Sprite> _clickEvent;

        public void SetOnClickLanguageListener(System.Action<string, Sprite> act) => _clickEvent = act;
        private void OnClickLanguageEvent(string key, string text, Sprite sprite)
        {
            TranslateStorage.Instance.SetLanguage(key);
            _clickEvent?.Invoke(text, sprite);            
            Hide();
        }
        #endregion
    }
}