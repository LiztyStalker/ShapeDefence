namespace UnityEngine.Advertisements
{

    public class UnityAdvertisement : IUnityAdsLoadListener, IUnityAdsShowListener, IUnityAdsInitializationListener
    {
#if UNITY_ANDROID
        private string _adsID = "4731229";
#elif UNITY_IOS
        private string _adsID = "4731228";
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
            Advertisement.Initialize(_adsID, true, false, this);
#endif
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

        public void ShowAds()
        {
            OnAdsActivateEvent(false);
            Advertisement.Show(_adsID, this);
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
            if(placementId == _adsID)
            {
                //showAd
                OnAdsActivateEvent(true);
            }
            else
            {
                OnAdsActivateEvent(false);
            }
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            Debug.Log($"Error loading Ad Unit {placementId}: {error.ToString()} - {message}");
        }

        public void OnUnityAdsShowClick(string placementId){}

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            if(placementId.Equals(_adsID) && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
            {
                Debug.Log("Ads Show Complete");
                OnAdsRewardedEvent(true);

                //load another ads
                Advertisement.Load(_adsID, this);
            }
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            Debug.Log($"Error showing Ad Unit {placementId}: {error} - {message}");
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