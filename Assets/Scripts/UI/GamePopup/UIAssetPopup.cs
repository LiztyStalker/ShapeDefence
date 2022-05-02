namespace SDefence.UI
{
    using Asset;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIAssetPopup : MonoBehaviour
    {
        [SerializeField]
        private UIAssetContainer _uiAsset;

        [SerializeField]
        private Button _applyBtn;

        [SerializeField]
        private Button _cancelBtn;

        public void Initialize()
        {
            _uiAsset.Initialize();
            _cancelBtn.onClick.AddListener(Hide);
        }

        public void CleanUp()
        {
            _uiAsset.CleanUp();
            _cancelBtn.onClick.AddListener(Hide);
        }

        public void Show(IAssetUsableData assetData, System.Action applyCallback)
        {
            _applyBtn.onClick.AddListener(() => 
            {
                applyCallback?.Invoke();
                Hide();
            });

            _uiAsset.SetData(assetData);

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            _applyBtn.onClick.RemoveAllListeners();
            gameObject.SetActive(false);
            OnClosedEvent();
        }

        public void SetData()
        {
            //Title
            //Text
            //IAssetUsableData
        }

        #region ##### Listener #####

        private System.Action _closedEvent;
        public void SetOnClosedListener(System.Action act) => _closedEvent = act;
        private void OnClosedEvent() => _closedEvent?.Invoke();

        #endregion
    }
}
