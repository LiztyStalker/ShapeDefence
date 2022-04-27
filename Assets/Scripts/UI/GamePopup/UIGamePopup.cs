namespace SDefence.UI
{
    using SDefence.Entity;
    using SDefence.Packet;
    using UnityEngine;

    public class UIGamePopup : MonoBehaviour, IBattlePacketUser, IEntityPacketUser
    {
        private UIClearPopup _uiClearPopup;
        private UIDefeatPopup _uiDefeatPopup;
        private UITechPopup _uiTechPopup;
        private UIAssetPopup _uiAssetPopup;
        private UIDisassemblePopup _uiDisassemblePopup;
        private UIRewardOfflinePopup _uiRewardOfflinePopup;

        public void Initialize()
        {
            _uiClearPopup = GetComponentInChildren<UIClearPopup>(true);
            _uiDefeatPopup = GetComponentInChildren<UIDefeatPopup>(true);
            _uiTechPopup = GetComponentInChildren<UITechPopup>(true);
            _uiAssetPopup = GetComponentInChildren<UIAssetPopup>(true);
            _uiDisassemblePopup = GetComponentInChildren<UIDisassemblePopup>(true);
            _uiRewardOfflinePopup = GetComponentInChildren<UIRewardOfflinePopup>(true);


            _uiClearPopup.Initialize();
            _uiDefeatPopup.Initialize();
            _uiTechPopup.Initialize();
            _uiAssetPopup.Initialize();
            _uiDisassemblePopup.Initialize();
            _uiRewardOfflinePopup.Initialize();

            _uiClearPopup.Hide();
            _uiDefeatPopup.Hide();
            _uiAssetPopup.Hide();
            _uiTechPopup.Hide();
            _uiDisassemblePopup.Hide();
            _uiRewardOfflinePopup.Hide();

            _uiClearPopup.SetOnClosedListener(Hide);
            _uiDefeatPopup.SetOnClosedListener(Hide);
            _uiTechPopup.SetOnClosedListener(Hide);
            _uiAssetPopup.SetOnClosedListener(Hide);
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

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
            OnClosedEvent();
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

        private void ShowTechPopup(TechPacketElement[] elements)
        {
            Show();
            _uiTechPopup.Show(elements);
        }

        private void ShowDisassemblePopup(IEntity entity)
        {
            Show();
            _uiDisassemblePopup.Show(entity);
        }

        public void ShowRewardOfflinePopup()
        {
            Show();
            _uiRewardOfflinePopup.Show();
        }

        public void ShowAssetPopup(System.Action applyCallback)
        {
            Show();
            _uiAssetPopup.Show(applyCallback);
        }






        #region ##### Listener #####

        private System.Action<ICommandPacket> _cmdEvent;
        public void SetOnCommandPacketListener(System.Action<ICommandPacket> act) => _cmdEvent = act;
        private void OnCommandPacketEvent(ICommandPacket pk) => _cmdEvent?.Invoke(pk);



        /// <summary>
        /// BattlePacket
        /// </summary>
        /// <param name="packet"></param>
        public void OnBattlePacketEvent(IBattlePacket packet)
        {
            switch (packet)
            {
                case ClearBattlePacket pk:
                    ShowClearPopup();
                    break;
                case DefeatBattlePacket pk:
                    ShowDefeatPopup();
                    break;
            }
        }

        /// <summary>
        /// EntityPacket
        /// </summary>
        /// <param name="packet"></param>
        public void OnEntityPacketEvent(IEntityPacket packet)
        {
            switch (packet)
            {
                case OpenDisassembleEntityPacket pk:
                    ShowDisassemblePopup(pk.Entity);
                    break;
                case OpenTechEntityPacket pk:
                    ShowTechPopup(pk.Elements);
                    break;
            }
        }




        private System.Action _closedvent;
        public void SetOnClosedListener(System.Action act) => _closedvent = act;
        private void OnClosedEvent() => _closedvent?.Invoke();


        #endregion
    }
}