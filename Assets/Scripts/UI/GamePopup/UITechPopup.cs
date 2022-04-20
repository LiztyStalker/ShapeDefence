namespace SDefence.UI
{
    using Packet;
    using Storage;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITechPopup : MonoBehaviour
    {

        [SerializeField]
        private Text _text;

        [SerializeField]
        private UIAsset _uiAsset;

        [SerializeField]
        private RectTransform _techFrame;

        [SerializeField]
        private Button _exitBtn;

        [SerializeField]
        private Button _closeBtn;

        private UITechButton _techBtn;

        private List<UITechButton> _list;

        public void Initialize()
        {
            _list = new List<UITechButton>();

            var obj = DataStorage.Instance.GetDataOrNull<GameObject>("UI@TechBtn");
            _techBtn = obj.GetComponent<UITechButton>();
            _techBtn.SetOnCommandPacketListener(OnCommandPacketEvent);

            _closeBtn.onClick.AddListener(OnCloseEvent);
            _exitBtn.onClick.AddListener(OnCloseEvent);
        }

        public void CleanUp()
        {
            _closeBtn.onClick.AddListener(OnCloseEvent);
            _exitBtn.onClick.AddListener(OnCloseEvent);

            _list.Clear();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            Clear();
            gameObject.SetActive(false);
            OnClosedEvent();
        }

        public void SetData()
        {
            //AssetData
            //TechData - List
        }

        private void Clear()
        {
            for(int i = 0; i < _list.Count; i++)
            {
                _list[i].Hide();
            }
        }

        #region ##### Listener #####

        private System.Action<ICommandPacket> _cmdEvent;
        public void SetOnCommandPacketListener(System.Action<ICommandPacket> act) => _cmdEvent = act;
        private void OnCommandPacketEvent(ICommandPacket pk) => _cmdEvent?.Invoke(pk);
        private void OnCloseEvent()
        {
            Hide();
        }



        private System.Action _closedEvent;
        public void SetOnClosedListener(System.Action act) => _closedEvent = act;
        private void OnClosedEvent() => _closedEvent?.Invoke();
        #endregion
    }
}