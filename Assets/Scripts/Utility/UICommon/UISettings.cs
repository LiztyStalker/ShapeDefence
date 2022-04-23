namespace Utility.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using UtilityManager;

    public class UISettings : MonoBehaviour
    {
        private readonly static string UGUI_NAME = "UI@Settings";

        private readonly string SETTINGS_BGM_KEY = "SETTINGS_BGM_VALUE";
        private readonly string SETTINGS_SFX_KEY = "SETTINGS_SFX_VALUE";
        private readonly string SETTINGS_FRAME_KEY = "SETTINGS_FRAME_VALUE";

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
        private UILanguageSheet _langSheet;
        [SerializeField]
        private Text _langLabel;
        [SerializeField]
        private Button _langBtn;

        [SerializeField]
        private UICreditsSheet _creditSheet;
        [SerializeField]
        private Button _creditsBtn;


        [SerializeField]
        private Button _saveBtn;

        [SerializeField]
        private InputField _gpgsIDField;
        [SerializeField]
        private Button _gpgsBtn;
        [SerializeField]
        private Button _qnaBtn;

        [SerializeField]
        private Button _serviceBtn;
        [SerializeField]
        private Button _personalBtn;

        [SerializeField]
        private Button _exitBtn;

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
            Debug.Assert(_exitBtn != null, $"_exitButton 구성하지 못했습니다");

            _langSheet.Initialize();
            _langSheet.Hide();

            _creditSheet.Initialize();
            _creditSheet.Hide();

            _versionLabel.text = Application.version;

            RegisterEvents();

            Load();

            Hide();

        }

        public void CleanUp()
        {
            _closedEvent = null;

            _langSheet.CleanUp();
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
            _bgmTg.onValueChanged.AddListener(OnBGMToggleEvent);
            _sfxTg.onValueChanged.AddListener(OnSFXToggleEvent);
            _frameSlider.onValueChanged.AddListener(OnFrameEvent);
            _langBtn.onClick.AddListener(OnLanguageSheetEvent);
            _langSheet.SetOnClickLanguageListener(OnClickLanguageEvent);
            _creditsBtn.onClick.AddListener(OnCreditSheetEvent);
            _gpgsBtn.onClick.AddListener(OnGPGSEvent);
            _qnaBtn.onClick.AddListener(OnQnAEvent);
            _serviceBtn.onClick.AddListener(OnServiceEvent);
            _personalBtn.onClick.AddListener(OnPersonalEvent);
            _saveBtn.onClick.AddListener(OnCloudSaveEvent);
            _exitBtn.onClick.AddListener(Hide);
        }

        private void UnRegisterEvents()
        {
            _bgmTg.onValueChanged.RemoveListener(OnBGMToggleEvent);
            _sfxTg.onValueChanged.RemoveListener(OnSFXToggleEvent);
            _frameSlider.onValueChanged.RemoveListener(OnFrameEvent);
            _langBtn.onClick.RemoveListener(OnLanguageSheetEvent);
            _creditsBtn.onClick.RemoveListener(OnCreditSheetEvent);
            _qnaBtn.onClick.RemoveListener(OnQnAEvent);
            _serviceBtn.onClick.RemoveListener(OnServiceEvent);
            _personalBtn.onClick.RemoveListener(OnPersonalEvent);
            _gpgsBtn.onClick.RemoveListener(OnGPGSEvent);
            _saveBtn.onClick.RemoveListener(OnCloudSaveEvent);
            _exitBtn.onClick.RemoveListener(Hide);
        }


        private void OnFrameEvent(float value)
        {
            _frameValueLabel.text = string.Format("{0}", value);
            Application.targetFrameRate = (int)value;
        }

        private void OnBGMToggleEvent(bool isOn)
        {
            AudioManager.Current.SetMute(AudioManager.TYPE_AUDIO.BGM, !isOn);
        }

        private void OnSFXToggleEvent(bool isOn)
        {
            AudioManager.Current.SetMute(AudioManager.TYPE_AUDIO.SFX, !isOn);
        }

        private void OnClickLanguageEvent(string text, Sprite sprite)
        {
            _langBtn.GetComponentInChildren<Text>().text = text;
            _langBtn.GetComponentInChildren<Image>().sprite = sprite;
        }

        private void OnLanguageSheetEvent()
        {
            _langSheet.Show();
        }

        private void OnCreditSheetEvent()
        {
            _creditSheet.Show();
        }

        private void SetLanguageLabel()
        {
            _langLabel.text = Storage.TranslateStorage.Instance.GetTranslateData("System_Tr", "Sys_Settings_Language");
        }

        private void OnCloudSaveEvent() 
        {
            //Cloud Save
            Debug.Log("OnSaveEvent");
        }

        private void OnGPGSEvent() 
        {
            //GPGS LogIn
            Debug.Log("OnGPGSEvent");
        }

        private void OnQnAEvent() 
        {
            //QnA Link
            //Application.OpenURL("");
            Debug.Log("OnQnAEvent");
        }

        private void OnServiceEvent()
        {
            //Service Link
            //Application.OpenURL("");
            Debug.Log("OnServiceEvent");
        }

        private void OnPersonalEvent()
        {
            //Personal Link
            //Application.OpenURL("");
            Debug.Log("OnPersonalEvent");
        }


        private void SetText()
        {
            SetLanguageLabel();
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


        private void Load()
        {
            _bgmTg.isOn = (PlayerPrefs.GetInt(SETTINGS_BGM_KEY, 1) == 1) ? true : false ;
            _sfxTg.isOn = (PlayerPrefs.GetInt(SETTINGS_SFX_KEY, 1) == 1) ? true : false;

            OnFrameEvent((float)PlayerPrefs.GetInt(SETTINGS_FRAME_KEY, 30));

            _langSheet.Load();
        }

        private void Save()
        {
            PlayerPrefs.SetInt(SETTINGS_BGM_KEY, (_bgmTg.isOn) ? 1 : 0);
            PlayerPrefs.SetInt(SETTINGS_SFX_KEY, (_sfxTg.isOn) ? 1 : 0);
            PlayerPrefs.SetInt(SETTINGS_FRAME_KEY, (int)_frameSlider.value);

            _langSheet.Save();
        }



        #region ##### Listener #####


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

    }
}