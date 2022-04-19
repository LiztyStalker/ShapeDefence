namespace SDefence.UI
{
    using UnityEngine.UI;
    using UnityEngine;
    using SDefence.Packet;

    public class UILobby : MonoBehaviour
    {
        [SerializeField]
        private Button _playBattleBtn;

        [SerializeField]
        private Button _hqBtn;

        [SerializeField]
        private Button _turretBtn;

        public void Initialize()
        {
            _playBattleBtn.onClick.AddListener(OnCommandPacketEvent);
            _hqBtn.onClick.AddListener(OnHQCategoryEvent);
            _turretBtn.onClick.AddListener(OnTurretCategoryEvent);
        }

        public void CleanUp()
        {
            _playBattleBtn.onClick.RemoveListener(OnCommandPacketEvent);
            _hqBtn.onClick.RemoveListener(OnHQCategoryEvent);
            _turretBtn.onClick.RemoveListener(OnTurretCategoryEvent);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        #region ##### Listener #####@

        private System.Action<ICommandPacket> _cmdEvent;
        public void AddOnCommandPacketListener(System.Action<ICommandPacket> act) => _cmdEvent += act;
        public void RemoveOnCommandPacketListener(System.Action<ICommandPacket> act) => _cmdEvent -= act;
        private void OnCommandPacketEvent()
        {
            var pk = new PlayBattleCommandPacket();
            _cmdEvent?.Invoke(pk);
        }

        private void OnHQCategoryEvent()
        {
            var pk = new CategoryCommandPacket();
            pk.Category = typeof(UIHQ).Name;
            _cmdEvent?.Invoke(pk);
        }

        private void OnTurretCategoryEvent()
        {
            var pk = new CategoryCommandPacket();
            pk.Category = typeof(UITurret).Name;
            _cmdEvent?.Invoke(pk);
        }

        #endregion
    }
}