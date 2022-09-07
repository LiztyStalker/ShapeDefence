namespace SDefence.UI
{
    using System.Collections;
    using UnityEngine;
    using Storage;
    using Utility.IO;
    using Utility.UI;
    using Utility;
    using GooglePlayGames.BasicApi;

    public class UIMain : MonoBehaviour
    {

        private readonly string SCENE_GAME_NAME = "Scene_Game";

        [SerializeField]
        private UIStart _uiStart;

        [SerializeField]
        private UILoad _uiLoad;

#if UNITY_EDITOR

        public static UIMain Create()
        {
            var obj = new GameObject();
            obj.name = "UI@Main";
            obj.AddComponent<Canvas>();
            return obj.AddComponent<UIMain>();
        }
#endif

        private void Start()
        {
            InitializeAssetBundle();
        }

        private void InitializeAssetBundle()
        {
            Initialize();
            //AssetBundle 불러오기
            DataStorage.Initialize(_uiLoad.ShowLoad, result =>
            {

                switch (result)
                {
                    case TYPE_IO_RESULT.Success:
                        //로그인 방식 선택 UI 필요
                        //로그인 방식 UI -> UILoginSelector -> UICommon
                        //GPGS Facebook Guest ...

                        //UILoginSelector
                        //GPGS 로그인
                        InitializeGPGS();
                        break;
                    case TYPE_IO_RESULT.DataProcessingError:
                    case TYPE_IO_RESULT.ConnectionError:
                    case TYPE_IO_RESULT.ProtocolError:
                        //Sys_AsBd_Error 
                        UICommon.Current.ShowPopup("AssetBundle\n에러가 발생했습니다.\n어플리케이션을 종료합니다", Quit);
                        break;
                }

            });
        }

        private void InitializeGPGS()
        {
            GPGSManager.Instance.SetOnAuthenticateListener(OnAutenticateEvent);
            GPGSManager.Instance.SetOnManuallyAutenticateListener(OnManuallyAutenticateEvent);
            GPGSManager.Instance.GPGSLogin();
        }

        private void OnAutenticateEvent(bool success, string msg)
        {
#if UNITY_EDITOR
            Debug.LogWarning("Editor Autenticate");
            InitializeSavableLoad();
#else
            if (success)
            {
                InitializeSavableLoad();
            }
            else
            {
                GPGSManager.Instance.ManuallyAutenticate();
            }
#endif
        }

        private void OnManuallyAutenticateEvent(SignInStatus signInStatus)
        {
            switch (signInStatus) 
            {
                case SignInStatus.Success:
                    InitializeSavableLoad();
                    break;
                case SignInStatus.InternalError:
                case SignInStatus.Canceled:
                    //Sys_GPGS_Error Sys_Yes Sys_No
                    UICommon.Current.ShowPopup($"{signInStatus}\n구글 계정에 접속할 수 없습니다.\n재시도 하시겠습니까?", "예", "아니오", InitializeGPGS, Quit);
                    break;
            }
        }

        private void InitializeSavableLoad()
        {
            //데이터 불러오기
            SavablePackage.Current.Load(_uiLoad.ShowLoad, result =>
            {
                switch (result)
                {
                    case TYPE_IO_RESULT.Success:
                        _uiLoad.Hide();
                        _uiStart.ShowStart(GameStart);
                        break;
                    case TYPE_IO_RESULT.DataProcessingError:
                    case TYPE_IO_RESULT.ConnectionError:
                    case TYPE_IO_RESULT.ProtocolError:
                        //Sys_Savable_Error 
                        UICommon.Current.ShowPopup($"{result}\n저장 불러오기에 실패했습니다.\n어플리케이션을 종료합니다", Quit);
                        break;
                }
            });
        }

        public void Initialize()
        {
            _uiStart.Initialize();
            _uiLoad.Initialize();
        }


        private void GameStart()
        {
            StartCoroutine(LoadAsyncCoroutine());
        }

        private IEnumerator LoadAsyncCoroutine()
        {
            _uiStart.Hide();

            var async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SCENE_GAME_NAME);

            while (!async.isDone)
            {
                _uiLoad.ShowLoad(async.progress);
                yield return null;
            }
        }

        private void Quit()
        {
            Application.Quit();
        }

        public void CleanUp()
        {
            _uiStart.CleanUp();
            _uiLoad.CleanUp();

        }
    }
}