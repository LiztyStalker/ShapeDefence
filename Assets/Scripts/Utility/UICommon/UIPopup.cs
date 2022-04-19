namespace Utility.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UIPopup : MonoBehaviour
    {
        private readonly static string UGUI_NAME = "UI@Popup";

        [SerializeField]
        private Text _msgLabel;

        [SerializeField]
        private Button _applyButton;
        [SerializeField]
        private Button _cancelButton;
        [SerializeField]
        private Button _exitButton;


        public static UIPopup Create()
        {
            var ui = Storage.DataStorage.Instance.GetDataOrNull<GameObject>(UGUI_NAME, null, null);
            if (ui != null)
            {
                return Instantiate(ui.GetComponent<UIPopup>());
            }
#if UNITY_EDITOR
            else
            {
                var obj = new GameObject();
                obj.name = "UI@Popup";
                return obj.AddComponent<UIPopup>();
            }
#else
            Debug.LogWarning($"{UGUI_NAME}을 찾을 수 없습니다");
            return null;
#endif
        }

        public void Initialize()
        {

            Debug.Assert(_msgLabel != null, $"_msgLabel 을 구성하지 못했습니다");
            Debug.Assert(_applyButton != null, $"_applyButton 을 구성하지 못했습니다");
            Debug.Assert(_cancelButton != null, $"_cancelButton 구성하지 못했습니다");
            Debug.Assert(_exitButton != null, $"_exitButton 구성하지 못했습니다");


            _applyButton.onClick.AddListener(OnApplyEvent);
            _cancelButton.onClick.AddListener(OnCancelEvent);
            _exitButton.onClick.AddListener(OnExitEvent);

            Hide();
        }

        public void CleanUp()
        {
            _applyEvent = null;
            _cancelEvent = null;
            _closedEvent = null;

            _applyButton.onClick.RemoveListener(OnApplyEvent);
            _cancelButton.onClick.RemoveListener(OnCancelEvent);
            _exitButton.onClick.RemoveListener(OnExitEvent);
        }


        public void ShowPopup(string msg, System.Action closedCallback = null)
        {
            SetPopup(msg, closedCallback);
            _exitButton.gameObject.SetActive(true);
            _cancelButton.gameObject.SetActive(false);
            Activate();
        }

        public void ShowPopup(string msg, string applyText, System.Action applyCallback = null, System.Action closedCallback = null)
        {
            SetPopup(msg, applyText, applyCallback, closedCallback);
            _applyButton.gameObject.SetActive(true);
            _cancelButton.gameObject.SetActive(false);
            _exitButton.gameObject.SetActive(true);
            Activate();
        }
        public void ShowPopup(string msg, string applyText, string cancelText, System.Action applyCallback = null, System.Action cancelCallback = null, System.Action closedCallback = null)
        {
            SetPopup(msg, applyText, cancelText, applyCallback, cancelCallback, closedCallback);
            _applyButton.gameObject.SetActive(true);
            _cancelButton.gameObject.SetActive(true);
            _exitButton.gameObject.SetActive(false);
            Activate();
        }

        private void SetPopup(string msg, System.Action closedCallback = null)
        {
            _msgLabel.text = msg;
            _closedEvent = closedCallback;
        }

        private void SetPopup(string msg, string applyText, System.Action applyCallback = null, System.Action closedCallback = null)
        {
            _applyButton.GetComponentInChildren<Text>().text  = applyText;
            _applyEvent = applyCallback;
            SetPopup(msg, closedCallback);
        }

        private void SetPopup(string msg, string applyText, string cancelText, System.Action applyCallback = null, System.Action cancelCallback = null, System.Action closedCallback = null)
        {
            _cancelButton.GetComponentInChildren<Text>().text = cancelText;
            _cancelEvent = cancelCallback;
            SetPopup(msg, applyText, applyCallback, closedCallback);
        }

        private void Activate()
        {
            gameObject.SetActive(true);
        }


        #region ##### Event #####

        private System.Action _applyEvent;
        private System.Action _cancelEvent;
        private System.Action _closedEvent;
        private void OnApplyEvent()
        {
            _applyEvent?.Invoke();
            Hide();
        }

        private void OnCancelEvent()
        {
            _cancelEvent?.Invoke();
            Hide();
        }

        private void OnExitEvent()
        {
            Hide();
        }

        private void OnClosedEvent()
        {
            _closedEvent?.Invoke();
        }

        #endregion

        public void Hide()
        {

            gameObject.SetActive(false);

            OnClosedEvent();

            _applyEvent = null;
            _cancelEvent = null;
            _closedEvent = null;

        }

    }
}