namespace Utility.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UILanguageButton : Button
    {
        [SerializeField]
        private Image _icon;

        [SerializeField]
        private Text _text;

        private string _key;

        protected override void Awake()
        {
            onClick.AddListener(OnClickEvent);
        }

        protected override void OnDestroy()
        {
            onClick.RemoveListener(OnClickEvent);
        }

        public void SetData(string key, string text)
        {
            _key = key;
            _text.text = text;
        }

        public void SetIcon(Sprite icon)
        {
            _icon.sprite = icon;
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        #region ##### Listener #####
        private System.Action<string, string, Sprite> _clickEvent;
        public void SetOnClickListener(System.Action<string, string, Sprite> act) => _clickEvent = act;
        private void OnClickEvent() => _clickEvent?.Invoke(_key, _text.text, _icon.sprite);
        #endregion
    }
}