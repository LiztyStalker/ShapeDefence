namespace SDefence.UI
{
    using Packet;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIButtons : MonoBehaviour
    {
        [SerializeField]
        private Button _settingsBtn;
        public void Initialize()
        {
            _settingsBtn.onClick.AddListener(OnSettingsEvent);
        }

        public void CleanUp()
        {
            _settingsBtn.onClick.RemoveListener(OnSettingsEvent);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }


        #region ##### Listener #####

        private System.Action<ICommandPacket> _cmdEvent;
        public void SetOnCommandPacketListener(System.Action<ICommandPacket> act) => _cmdEvent = act;
        private void OnSettingsEvent()
        {
            var pk = new SettingsCommandPacket();
            _cmdEvent?.Invoke(pk);
        }
        #endregion
    }
}