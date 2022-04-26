namespace SDefence.UI
{
    using System.Collections;
    using UnityEngine;
    using Storage;
    using Utility.IO;
    using Utility.UI;

    public class UIMain : MonoBehaviour
    {
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
                if (result != TYPE_IO_RESULT.Success)
                {
                    //Success가 아니면 메시지 출력
                    ShowPopup(result);
                }
                InitializeAccount();
            });
        }

        private void InitializeAccount()
        {
            //데이터 불러오기
            SavablePackage.Current.Load(_uiLoad.ShowLoad, result =>
            {
                //if (result != TYPE_IO_RESULT.Success)
                //{
                //Success가 아니면 메시지 출력
                //ShowPopup(result);
                //}
                Debug.Log("LoadData " + _uiLoad + " " + _uiStart);
                _uiLoad.Hide();
                _uiStart.ShowStart(GameStart);
            });
        }

        private void ShowPopup(TYPE_IO_RESULT result)
        {
            UICommon.Current.ShowPopup(result.ToString(), Quit);
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

            var async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Scene_Game");

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