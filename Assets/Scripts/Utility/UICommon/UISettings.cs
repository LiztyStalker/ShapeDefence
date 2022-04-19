namespace Utility.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UISettings : MonoBehaviour
    {
        private readonly static string UGUI_NAME = "UI@Settings";

        private readonly string SETTINGS_BGM_KEY = "BGM_VALUE";
        private readonly string SETTINGS_SFX_KEY = "SFX_VALUE";
        private readonly string SETTINGS_FRAME_KEY = "FRAME_VALUE";
        private readonly string SETTINGS_HIT_KEY = "ISHIT";
        private readonly string SETTINGS_EFFECT_KEY = "ISEFFECT";

        [SerializeField]
        private Text _versionLabel;
        [SerializeField]
        private Toggle _bgmTg;
        [SerializeField]
        private Toggle _sfxTg;

        [SerializeField]
        private Slider _frameSlider;
        [SerializeField]
        private Text _frameValueLabel;

        [SerializeField]
        private GameObject _langSheet;
        [SerializeField]
        private Text _langLabel;
        [SerializeField]
        private Button _langBtn;

        [SerializeField]
        private GameObject _creditSheet;
        [SerializeField]
        private Button _creditsBtn;


        [SerializeField]
        private Button _saveBtn;

        [SerializeField]
        private Button _gpgsBtn;
        [SerializeField]
        private Button _qnaBtn;

        [SerializeField]
        private Button _serviceBtn;
        [SerializeField]
        private Button _personalBtn;

        [SerializeField]
        private Button _exitButton;

        public static UISettings Create()
        {
            var ui = Storage.DataStorage.Instance.GetDataOrNull<GameObject>(UGUI_NAME, null, null);
            if (ui != null)
            {
                return Instantiate(ui.GetComponent<UISettings>());
            }
#if UNITY_EDITOR
            else
            { 
                var obj = new GameObject();
                obj.name = "UI@Settings";
                return obj.AddComponent<UISettings>();
            }
#else
            Debug.LogWarning($"{UGUI_NAME}을 찾을 수 없습니다");
            return null;
#endif

        }

        public void Initialize()
        {

            Debug.Assert(_versionLabel != null, $"_versionLabel 을 구성하지 못했습니다");
            Debug.Assert(_frameSlider != null, $"_frameSlider 을 구성하지 못했습니다");
            Debug.Assert(_frameValueLabel != null, $"_frameValueLabel 구성하지 못했습니다");
            Debug.Assert(_langLabel != null, $"_langLabel 을 구성하지 못했습니다");
            Debug.Assert(_saveBtn != null, $"_saveButton 구성하지 못했습니다");
            Debug.Assert(_gpgsBtn != null, $"_gpgsButton 구성하지 못했습니다");
            Debug.Assert(_qnaBtn != null, $"_qnaButton 구성하지 못했습니다");
            Debug.Assert(_exitButton != null, $"_exitButton 구성하지 못했습니다");

            Load();

            _versionLabel.text = Application.version;

            RegisterEvents();

            Hide();

        }

        public void CleanUp()
        {
            _closedEvent = null;

            UnRegisterEvents();
        }


        private void OnEnable()
        {
            Storage.TranslateStorage.Instance.AddOnChangedTranslateListener(SetText);
        }

        public void OnDisable()
        {
            Storage.TranslateStorage.Instance.RemoveOnChangedTranslateListener(SetText);
        }

        private void RegisterEvents()
        {
            _frameSlider.onValueChanged.AddListener(OnFrameEvent);
            _exitButton.onClick.AddListener(OnExitEvent);
        }

        private void UnRegisterEvents()
        {
            _frameSlider.onValueChanged.RemoveListener(OnFrameEvent);

            _exitButton.onClick.RemoveListener(OnExitEvent);
        }


        private void OnFrameEvent(float value)
        {
            _frameValueLabel.text = string.Format("{0:d0}", (value * 100f));
        }

        private void OnLeftLanguageEvent() 
        {
            Storage.TranslateStorage.Instance.PrevLanguageIndex();
        }

        private void OnRightLanguageEvent() 
        {
            Storage.TranslateStorage.Instance.NextLanguageIndex();
        }

        private void SetLanguageLabel()
        {
            _langLabel.text = Storage.TranslateStorage.Instance.GetTranslateData("System_Tr", "Sys_Settings_Language");
        }

        private void SetToggleText()
        {
            //_uiHitActivateToggle.GetComponentInChildren<Text>().text = Storage.TranslateStorage.Instance.GetTranslateData("System_Tr", "Sys_Settings_Hit"); ;
            //_effectActivateToggle.GetComponentInChildren<Text>().text = Storage.TranslateStorage.Instance.GetTranslateData("System_Tr", "Sys_Settings_Effect"); ;
        }


        private void OnSaveEvent() 
        {
            Debug.Log("OnSaveEvent");
        }

        private void OnLoadEvent()
        {
            Debug.Log("OnLoadEvent");
        }

        private void OnGPGSEvent() 
        {
            Debug.Log("OnGPGSEvent");
        }

        private void OnQnAEvent() 
        {
            Debug.Log("OnQnAEvent");
        }


        private void SetText()
        {
            SetLanguageLabel();
            SetToggleText();
        }

        public void Show(System.Action closedCallback = null)
        {
            gameObject.SetActive(true);
            SetText();
            _closedEvent = closedCallback;
        }


        public void Hide()
        {
            gameObject.SetActive(false);

            OnClosedEvent();
            Save();

            _closedEvent = null;
        }





#region ##### Event #####


        private System.Action _closedEvent;


        private void OnExitEvent()
        {
            Hide();
        }

        private void OnClosedEvent()
        {
            _closedEvent?.Invoke();
        }

#endregion


        private void Load()
        {
            //_bgmVolumeLabel.text = _bgmSlider.value.ToString();

            //_sfxVolumeLabel.text = _sfxSlider.value.ToString();

            //_frameSlider.value = (float)PlayerPrefs.GetInt(SETTINGS_FRAME_KEY, 30);
            //_frameValueLabel.text = _frameSlider.value.ToString();

            ////Translator에서 가져오기 - 이미 불러와져 있음
            //Storage.TranslateStorage.Instance.ChangedLanguage();

            //_uiHitActivateToggle.isOn = (PlayerPrefs.GetInt(SETTINGS_HIT_KEY, 1) == 1) ? true : false;
            //_effectActivateToggle.isOn = (PlayerPrefs.GetInt(SETTINGS_EFFECT_KEY, 1) == 1) ? true : false;
        }

        private void Save()
        {
            //PlayerPrefs.SetInt(SETTINGS_BGM_KEY, (int)_bgmSlider.value);
            //PlayerPrefs.SetInt(SETTINGS_SFX_KEY, (int)_sfxSlider.value);
            //PlayerPrefs.SetInt(SETTINGS_FRAME_KEY, (int)_frameSlider.value);

            ////Translator에서 저장하기
            //Storage.TranslateStorage.Instance.Save();

            //PlayerPrefs.SetInt(SETTINGS_HIT_KEY, (_uiHitActivateToggle.isOn) ? 1 : 0);
            //PlayerPrefs.SetInt(SETTINGS_EFFECT_KEY, (_effectActivateToggle.isOn) ? 1 : 0);
        }
    }
}