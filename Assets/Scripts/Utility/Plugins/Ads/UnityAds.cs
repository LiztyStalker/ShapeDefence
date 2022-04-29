namespace UnityEngine.Advertisements
{

    public class UnityAdvertisement : IUnityAdsLoadListener, IUnityAdsShowListener, IUnityAdsInitializationListener
    {
#if UNITY_ANDROID
        private string _gameID = "4731229";
#elif UNITY_IOS
        private string _gameID = "4731228";
#endif

#if UNITY_ANDROID
        private string _adsID = "Rewarded_Android";
#elif UNITY_IOS
        private string _adsID = "Rewarded_iOS";
#endif

        private static UnityAdvertisement _instance;
        public static UnityAdvertisement Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new UnityAdvertisement();
                }
                return _instance;
            }
        }

        private UnityAdvertisement()
        {
#if UNITY_EDITOR
            if(!Advertisement.isInitialized)
                Advertisement.Initialize(_gameID, true, true, this);
           
#endif
            LoadAds();
        }

        public static void Dispose() 
        {
            _instance = null;
        }

        public void OnInitializationComplete()
        {
#if UNITY_EDITOR
            Debug.Log("Ads Complete");
#endif
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
#if UNITY_EDITOR
            Debug.Log("Ads Error " + error.ToString() + " " + message);
#endif
        }

        public bool IsInitialized => Advertisement.isInitialized;
        public void LoadAds()
        {
            Advertisement.Load(_adsID, this);
        }

        public void ShowAds()
        {
            if (Advertisement.IsReady())
            {
                OnAdsActivateEvent(false);
                Advertisement.Show(_adsID, this);
            }
            else
            {
                //광고 준비 되지 않음
                OnAdsRewardedEvent(false);
            }
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
            if(placementId == _adsID)
            {
                OnAdsActivateEvent(true);
            }
            else
            {
                OnAdsActivateEvent(false);
            }
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
#if UNITY_EDITOR
            Debug.Log($"Error loading Ad Unit {placementId}: {error.ToString()} - {message}");
#endif
        }

        public void OnUnityAdsShowClick(string placementId){}

#if UNITY_EDITOR
        public void OnUnityAdsShowComplete_Test() => OnUnityAdsShowComplete(_adsID, UnityAdsShowCompletionState.COMPLETED);
        public void OnUnityAdsShowSkipped_Test() => OnUnityAdsShowComplete(_adsID, UnityAdsShowCompletionState.SKIPPED);
#endif

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            if(placementId.Equals(_adsID) && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
            {
                OnAdsRewardedEvent(true);
            }
            else
            {
                OnAdsRewardedEvent(false);
            }
            LoadAds();
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
#if UNITY_EDITOR
            Debug.Log($"Error showing Ad Unit {placementId}: {error} - {message}");
#endif
        }

        public void OnUnityAdsShowStart(string placementId){}



#region ##### Listener #####

        private System.Action<bool> _activateEvent;
        public void AddOnAdsActivateListener(System.Action<bool> act) => _activateEvent += act;
        public void RemoveOnAdsActivateListener(System.Action<bool> act) => _activateEvent -= act;
        private void OnAdsActivateEvent(bool isActive) => _activateEvent?.Invoke(isActive);



        private System.Action<bool> _rewardEvent;
        public void SetOnAdsRewardedListener(System.Action<bool> act) => _rewardEvent = act;
        private void OnAdsRewardedEvent(bool isActive) => _rewardEvent?.Invoke(isActive);

#endregion

    }
}