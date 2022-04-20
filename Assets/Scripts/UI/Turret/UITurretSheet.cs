namespace SDefence.UI
{
    using Packet;
    using Storage;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITurretSheet : MonoBehaviour
    {
        [SerializeField]
        private ScrollRect _scrollRect;

        private UITurretBlock _uiBlock;

        private UITurretExpand _uiExpand;

        private List<UITurretBlock> _list;

        public void Initialize()
        {
            _list = new List<UITurretBlock>();

            var expandBlock = DataStorage.Instance.GetDataOrNull<GameObject>("UI@TurretExpand");
            _uiExpand = expandBlock.GetComponent<UITurretExpand>();
            _uiExpand.SetOnCommandPacketListener(OnCommandPacketEvent);

            var block = DataStorage.Instance.GetDataOrNull<GameObject>("UI@TurretBlock");
            _uiBlock = block.GetComponent<UITurretBlock>();
            _uiBlock.SetOnCommandPacketListener(OnCommandPacketEvent);
        }
        public void CleanUp()
        {
            _list.Clear();
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
            //Turret을 찾아서 OnEntityPacketEvent 실행
            //_list[index].OnEntityPacketEvent(pk);
        }

        #region ##### Listener #####

        private System.Action<ICommandPacket> _cmdEvent;
        public void SetOnCommandPacketListener(System.Action<ICommandPacket> act) => _cmdEvent = act;
        private void OnCommandPacketEvent(ICommandPacket pk) => _cmdEvent?.Invoke(pk);

        #endregion


    }
}