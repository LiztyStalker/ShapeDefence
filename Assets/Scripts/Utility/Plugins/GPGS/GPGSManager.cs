namespace Utility
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GooglePlayGames;
    using GooglePlayGames.BasicApi;

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




        #region ##### Listener #####

        private System.Action<bool> _authenticateEvent;
        public void SetOnAuthenticateListener(System.Action<bool> act) => _authenticateEvent = act;
        private void OnAuthenticateEvent(bool success) => _authenticateEvent?.Invoke(success);

        #endregion
    }
}