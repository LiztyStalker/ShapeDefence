namespace SDefence.UI
{
    using Packet;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITechButton : Button
    {
        [SerializeField]
        private Text _text;

        protected override void Awake()
        {
            onClick.AddListener(OnUpTechEvent);
        }

        protected override void OnDestroy()
        {
            onClick.RemoveListener(OnUpTechEvent);
        }


        public void Show()
        {
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetData()
        {
            //AssetData
        }

        #region ##### Listener #####

        private System.Action<ICommandPacket> _cmdEvent;
        public void SetOnCommandPacketListener(System.Action<ICommandPacket> act) => _cmdEvent = act;
        private void OnUpTechEvent()
        {
            var pk = new UpTechCommandPacket();
            //TechData
            _cmdEvent?.Invoke(pk);
        }
        #endregion
    }
}