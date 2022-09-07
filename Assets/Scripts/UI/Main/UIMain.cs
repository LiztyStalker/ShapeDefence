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
            //AssetBundle �ҷ�����
            DataStorage.Initialize(_uiLoad.ShowLoad, result =>
            {

                switch (result)
                {
                    case TYPE_IO_RESULT.Success:
                        //�α��� ��� ���� UI �ʿ�
                        //�α��� ��� UI -> UILoginSelector -> UICommon
                        //GPGS Facebook Guest ...

                        //UILoginSelector
                        //GPGS �α���
                        InitializeGPGS();
                        break;
                    case TYPE_IO_RESULT.DataProcessingError:
                    case TYPE_IO_RESULT.ConnectionError:
                    case TYPE_IO_RESULT.ProtocolError:
                        //Sys_AsBd_Error 
                        UICommon.Current.ShowPopup("AssetBundle\n������ �߻��߽��ϴ�.\n���ø����̼��� �����մϴ�", Quit);
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
                    UICommon.Current.ShowPopup($"{signInStatus}\n���� ������ ������ �� �����ϴ�.\n��õ� �Ͻðڽ��ϱ�?", "��", "�ƴϿ�", InitializeGPGS, Quit);
                    break;
            }
        }

        private void InitializeSavableLoad()
        {
            //������ �ҷ�����
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
                        UICommon.Current.ShowPopup($"{result}\n���� �ҷ����⿡ �����߽��ϴ�.\n���ø����̼��� �����մϴ�", Quit);
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