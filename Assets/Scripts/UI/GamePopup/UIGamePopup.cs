namespace SDefence.UI
{
    using SDefence.Packet;
    using UnityEngine;

    public class UIGamePopup : MonoBehaviour
    {
        private UIClearPopup _uiClearPopup;
        private UIDefeatPopup _uiDefeatPopup;
        private UITechPopup _uiTechPopup;
        private UIDisassemblePopup _uiDisassemblePopup;
        private UIRewardOfflinePopup _uiRewardOfflinePopup;

        public void Initialize()
        {
            _uiClearPopup = GetComponentInChildren<UIClearPopup>(true);
            _uiDefeatPopup = GetComponentInChildren<UIDefeatPopup>(true);
            _uiTechPopup = GetComponentInChildren<UITechPopup>(true);
            _uiDisassemblePopup = GetComponentInChildren<UIDisassemblePopup>(true);
            _uiRewardOfflinePopup = GetComponentInChildren<UIRewardOfflinePopup>(true);

            _uiClearPopup.Initialize();
            _uiDefeatPopup.Initialize();
            _uiTechPopup.Initialize();
            _uiDisassemblePopup.Initialize();
            _uiRewardOfflinePopup.Initialize();

            _uiClearPopup.Hide();
            _uiDefeatPopup.Hide();
            _uiTechPopup.Hide();
            _uiDisassemblePopup.Hide();
            _uiRewardOfflinePopup.Hide();

            _uiClearPopup.SetOnClosedListener(Hide);
            _uiDefeatPopup.SetOnClosedListener(Hide);
            _uiTechPopup.SetOnClosedListener(Hide);
            _uiDisassemblePopup.SetOnClosedListener(Hide);
            _uiRewardOfflinePopup.SetOnClosedListener(Hide);

            _uiClearPopup.SetOnCommandPacketListener(OnCommandPacketEvent);
            _uiDefeatPopup.SetOnCommandPacketListener(OnCommandPacketEvent);
            _uiTechPopup.SetOnCommandPacketListener(OnCommandPacketEvent);
            _uiDisassemblePopup.SetOnCommandPacketListener(OnCommandPacketEvent);
            _uiRewardOfflinePopup.SetOnCommandPacketListener(OnCommandPacketEvent);

        }

        public void CleanUp()
        {
            _uiClearPopup.SetOnClosedListener(null);
            _uiDefeatPopup.SetOnClosedListener(null);
            _uiTechPopup.SetOnClosedListener(null);
            _uiDisassemblePopup.SetOnClosedListener(null);
            _uiRewardOfflinePopup.SetOnClosedListener(null);

            _uiClearPopup.CleanUp();
            _uiDefeatPopup.CleanUp();
            _uiTechPopup.CleanUp();
            _uiDisassemblePopup.CleanUp();
            _uiRewardOfflinePopup.CleanUp();
        }

        public void ShowClearPopup()
        {
            Show();
            _uiClearPopup.Show();
        }

        public void ShowDefeatPopup()
        {
            Show();
            _uiDefeatPopup.Show();
        }

        /// <summary>
        /// TechData
        /// </summary>
        public void ShowTechPopup()
        {
            Show();
            _uiTechPopup.Show();
        }

        public void ShowDisassemblePopup()
        {
            Show();
            _uiDisassemblePopup.Show();
        }

        public void ShowRewardOfflinePopup()
        {
            Show();
            _uiRewardOfflinePopup.Show();
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }


        #region ##### Listener #####
        private System.Action<ICommandPacket> _cmdEvent;
        public void SetOnCommandPacketListener(System.Action<ICommandPacket> act) => _cmdEvent = act;
        private void OnCommandPacketEvent(ICommandPacket pk) => _cmdEvent?.Invoke(pk);

        #endregion
    }
}