namespace Utility.UI
{
    using UnityEngine;

    public class UICommon : MonoBehaviour
    {
        private readonly static string UGUI_NAME = "UI@Common";

        private static UICommon _current;


        public static UICommon Current
        {
            get
            {
                if(_current == null)
                {
                    _current = Create();
                    _current.Initialize();
                }
                return _current;
            }
        }

        private UIPopup _uiPopup;
        private UISettings _uiSettings;
        private static UICommon Create()
        {
            var ui = Storage.DataStorage.Instance.GetDataOrNull<GameObject>(UGUI_NAME);
            if (ui != null)
            {
                return Instantiate(ui.GetComponent<UICommon>());
            }
#if UNITY_EDITOR
            else
            {
                var obj = new GameObject();
                obj.name = "UI@Common";
                return obj.AddComponent<UICommon>();
            }
#else
            Debug.LogWarning($"{UGUI_NAME}을 찾을 수 없습니다");               
            return null;
#endif
        }

        public void Initialize()
        {

            _uiPopup = GetComponentInChildren<UIPopup>(true);

            if (_uiPopup == null)
            {
                _uiPopup = UIPopup.Create();
                _uiPopup.transform.SetParent(transform);
            }

            _uiPopup.Initialize();


            _uiSettings = GetComponentInChildren<UISettings>(true);

            if (_uiSettings == null)
            {
                _uiSettings = UISettings.Create();
                _uiSettings.transform.SetParent(transform);
            }
            _uiSettings.Initialize();

        }


        public void CleanUp()
        {
            _uiPopup.CleanUp();
            _uiSettings.CleanUp();
            _current = null;
        }





        public void ShowPopup(string msg, System.Action closedCallback = null)
        {
            _uiPopup.ShowPopup(msg, closedCallback);
        }


        public void ShowPopup(string msg, string applyText, System.Action applyCallback = null, System.Action closedCallback = null)
        {
            _uiPopup.ShowPopup(msg, applyText, applyCallback, closedCallback);
        }

        public void ShowPopup(string msg, string applyText, string cancelText, System.Action applyCallback = null, System.Action cancelCallback = null, System.Action closedCallback = null)
        {
            _uiPopup.ShowPopup(msg, applyText, cancelText, applyCallback, cancelCallback, closedCallback);
        }

        public void ShowSettings(System.Action closedCallback = null)
        {
            _uiSettings.Show(closedCallback);
        }

    }
}