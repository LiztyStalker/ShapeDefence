namespace SDefence.UI
{
    using Packet;
    using Entity;
    using Tech;
    using Storage;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITechPopup : MonoBehaviour
    {

        [SerializeField]
        private Text _text;

        [SerializeField]
        private RectTransform _techFrame;

        [SerializeField]
        private Button _exitBtn;

        [SerializeField]
        private Button _closeBtn;

        private UITechPopupButton _techBtn;

        private List<UITechPopupButton> _list;

        public void Initialize()
        {
            _list = new List<UITechPopupButton>();

            var obj = DataStorage.Instance.GetDataOrNull<GameObject>("UI@TechBtn");
            _techBtn = obj.GetComponent<UITechPopupButton>();
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

        public void Show(TechPacketElement[] elements, int orbitIndex = 0, int index = 0)
        {
            Clear();

            gameObject.SetActive(true);

            for (int i = 0; i < elements.Length; i++)
            {
                var element = elements[i];
                if (i >= _list.Count)
                {
                    _list.Add(Create());
                }
                _list[i].SetIndex(orbitIndex, index);
                _list[i].SetData(element.Element.TypeTechData, element.Element.TechDataKey, element.Element.GetUsableData(), element.IsActiveTech);
                _list[i].Show();            
            }
        }

        public void Hide()
        {
            Clear();
            gameObject.SetActive(false);
            OnClosedEvent();
        }

        private UITechPopupButton Create()
        {
            var btn = Instantiate(_techBtn);
            btn.transform.SetParent(_techFrame);
            btn.transform.localScale = Vector3.one;
            btn.SetOnCommandPacketListener(OnCommandPacketEvent);
            return btn;
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
        private void OnCommandPacketEvent(ICommandPacket pk)
        {
            _cmdEvent?.Invoke(pk);
            Hide();
        }

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