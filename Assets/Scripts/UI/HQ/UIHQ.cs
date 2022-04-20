namespace SDefence.UI
{
    using Packet;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIHQ : MonoBehaviour, ICategory
    {

        [SerializeField]
        private UIAsset _uiAsset;

        [SerializeField]
        private Button _helpBtn;

        [SerializeField]
        private Button _exitBtn;

        [SerializeField]
        private UIAssetButton _upgradeBtn;

        [SerializeField]
        private UIAssetButton _techBtn;

       

        public void Initialize()
        {
            _upgradeBtn.onClick.AddListener(OnUpgradeCommandPacketEvent);
            _techBtn.onClick.AddListener(OnTechCommandPacketEvent);

            _helpBtn.onClick.AddListener(OnHelpCommandPacketEvent);
            _exitBtn.onClick.AddListener(Hide);
        }
        public void CleanUp()
        {
            _upgradeBtn.onClick.RemoveListener(OnUpgradeCommandPacketEvent);
            _techBtn.onClick.RemoveListener(OnTechCommandPacketEvent);

            _helpBtn.onClick.RemoveListener(OnHelpCommandPacketEvent);
            _exitBtn.onClick.RemoveListener(Hide);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }        

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnEntityPacketEvent(IEntityPacket pk)
        {
            RefreshUI();
        }

        private void RefreshUI()
        {
            //Name - 본부 
            //Tech - 등급[3 / 10]
            //Level - Lv 45 / 50
            //Health - 체력
            //Armor - 방어력
            //Shield - 실드
            //ShieldRec - 실드회복량
            //LimShield - 실드한계값
            //Orbit - 궤도 수

            //Upgrade - Asset
            //Tech - Asset
        }

        #region ##### Listener #####

        private System.Action<ICommandPacket> _cmdEvent;
        public void SetOnCommandPacketListener(Action<ICommandPacket> act) => _cmdEvent = act;
        private void OnHelpCommandPacketEvent()
        {
            //HelpCommandPacket
            var pk = new HelpCommandPacket();
            _cmdEvent?.Invoke(pk);
        }

        private void OnUpgradeCommandPacketEvent()
        {
            //UpgradeCommandPacket
            var pk = new UpgradeCommandPacket();
            _cmdEvent?.Invoke(pk);
        }

        private void OnTechCommandPacketEvent()
        {
            //OpenTechCommandPacket
            var pk = new OpenTechCommandPacket();
            _cmdEvent?.Invoke(pk);
        }

        #endregion
    }
}