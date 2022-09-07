namespace Utility
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GooglePlayGames;
    using GooglePlayGames.BasicApi;
    using GooglePlayGames.OurUtils;
    using GooglePlayGames.BasicApi.SavedGame;
    using System;

    public class GPGSManager
    {
        private static GPGSManager _instance;

        public static GPGSManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new GPGSManager();
                    _instance.Initialize();
                }
                return _instance;
            }
        }
        public bool IsAuthenticated => Social.localUser.authenticated;


        private void Initialize()
        {
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();
        }

        public void GPGSAutoLogin()
        {
            if (!IsAuthenticated)
            {
                Social.localUser.Authenticate(OnAuthenticateEvent);
            }
        }

        public void GPGSLogin()
        {
            if (!IsAuthenticated)
            {
                Social.localUser.Authenticate(OnAuthenticateEvent);
            }
        }

        public void GPGSLogout()
        {
            //((PlayGamesPlatform)Social.Active).Sign
        }

        public void RequestServerSideAccess(bool forceRequestToken, System.Action<string> callback)
        {
            PlayGamesPlatform.Instance.RequestServerSideAccess(forceRequestToken, callback);
        }

        public string GetGPGSID()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (Social.localUser.authenticated)
                return Social.localUser.id;
#endif
            return null;
        }

        /// <summary>
        /// 로그인 요청
        /// </summary>
        public void ManuallyAutenticate()
        {
            PlayGamesPlatform.Instance.ManuallyAuthenticate(OnManuallyAutenticateEvent);
        }

        public void CloudSave(ISavedGameMetadata game, byte[] savedData, TimeSpan totalPlaytime)
        {
            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

            SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
            builder = builder
                .WithUpdatedPlayedTime(totalPlaytime)
                .WithUpdatedDescription("Saved game at " + DateTime.Now);
            //if (savedImage != null)
            //{
            //    // This assumes that savedImage is an instance of Texture2D
            //    // and that you have already called a function equivalent to
            //    // getScreenshot() to set savedImage
            //    // NOTE: see sample definition of getScreenshot() method below
            //    byte[] pngData = savedImage.EncodeToPNG();
            //    builder = builder.WithUpdatedPngCoverImage(pngData);
            //}
            SavedGameMetadataUpdate updatedMetadata = builder.Build();
            savedGameClient.CommitUpdate(game, updatedMetadata, savedData, OnSavedGameWritten);
        }

        public void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
        {
            OnCloudSaveEvent(status);
            //if (status == SavedGameRequestStatus.Success)
            //{
            //    // handle reading or writing of saved game.
            //}
            //else
            //{
            //    // handle error
            //}
        }

        //public Texture2D getScreenshot()
        //{
        //    // Create a 2D texture that is 1024x700 pixels from which the PNG will be
        //    // extracted
        //    Texture2D screenShot = new Texture2D(1024, 700);

        //    // Takes the screenshot from top left hand corner of screen and maps to top
        //    // left hand corner of screenShot texture
        //    screenShot.ReadPixels(
        //        new Rect(0, 0, Screen.width, (Screen.width / 1024) * 700), 0, 0);
        //    return screenShot;
        //}
        

        public void CloudLoad(ISavedGameMetadata game)
        {
            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.ReadBinaryData(game, OnSavedGameDataRead);

        }
        private void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data)
        {
            OnCloudLoadEvent(status, data);
        }



        #region ##### Listener #####

        private System.Action<bool, string> _authenticateEvent;
        public void SetOnAuthenticateListener(System.Action<bool, string> act) => _authenticateEvent = act;
        private void OnAuthenticateEvent(bool success, string msg) => _authenticateEvent?.Invoke(success, msg);



        private System.Action<SignInStatus> _manuallyEvent;
        public void SetOnManuallyAutenticateListener(System.Action<SignInStatus> act) => _manuallyEvent = act;
        private void OnManuallyAutenticateEvent(SignInStatus signInStatus) => _manuallyEvent?.Invoke(signInStatus);



        private System.Action<SavedGameRequestStatus> _cloudSaveEvent;
        public void SetOnCloudSaveListener(System.Action<SavedGameRequestStatus> act) => _cloudSaveEvent = act;
        private void OnCloudSaveEvent(SavedGameRequestStatus request) => _cloudSaveEvent?.Invoke(request);



        private System.Action<SavedGameRequestStatus, byte[]> _cloudLoadEvent;
        public void SetOnCloudLoadListener(System.Action<SavedGameRequestStatus, byte[]> act) => _cloudLoadEvent = act;
        private void OnCloudLoadEvent(SavedGameRequestStatus request, byte[] data) => _cloudLoadEvent?.Invoke(request, data);


        #endregion
    }
}