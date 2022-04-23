namespace SDefence.UI
{
    using Packet;
    using UnityEngine;

    public class UITurretExpand : MonoBehaviour
    {
        [SerializeField]
        private UIAssetButton _uiExpandBtn;

        private void Awake()
        {
            _uiExpandBtn.onClick.AddListener(OnCommandPacketEvent);
        }

        private void OnDestroy()
        {
            _uiExpandBtn.onClick.RemoveListener(OnCommandPacketEvent);
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        #region ##### Listener #####

        private System.Action<ICommandPacket> _cmdEvent;
        public void SetOnCommandPacketListener(System.Action<ICommandPacket> act) => _cmdEvent = act;

        private void OnCommandPacketEvent()
        {
            var pk = new OpenExpandCommandPacket();
            _cmdEvent?.Invoke(pk);
        }
        #endregion
    }
}