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

        [SerializeField]
        private Text _turretCountText;

        private UITurretBlock _uiBlock;

        private UITurretExpand _uiExpand;

        private List<UITurretBlock> _list;

        public void Initialize()
        {
            _list = new List<UITurretBlock>();

            var expandObject = DataStorage.Instance.GetDataOrNull<GameObject>("UI@TurretExpand");
            var expandBlock = Instantiate(expandObject);
            _uiExpand = expandBlock.GetComponent<UITurretExpand>();
            _uiExpand.SetOnCommandPacketListener(OnCommandPacketEvent);

            _uiExpand.transform.SetParent(_scrollRect.content);
            _uiExpand.transform.localScale = Vector3.one;

            var block = DataStorage.Instance.GetDataOrNull<GameObject>("UI@TurretBlock");
            _uiBlock = block.GetComponent<UITurretBlock>();
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

        private void SetTurretCountText(int turretCount, int capacity)
        {
            //Translate
            _turretCountText.text = $"포탑 가용 수 : {turretCount} / {capacity}";
        }

        public void OnEntityPacketEvent(IEntityPacket packet)
        {
            switch (packet)
            {
                case TurretEntityPacket pk:
                    _list[pk.Index].OnEntityPacketEvent(pk);
                    break;
                case TurretArrayEntityPacket pk:
                    Clear();
                    var packets = pk.packets;
                    for (int i = 0; i < packets.Length; i++)
                    {
                        if(i >= _list.Count)
                        {
                            CreateBlock();
                        }
                        var block = _list[i];
                        block.OnEntityPacketEvent(packets[i]);
                        block.Show();
                    }

                    _uiExpand.SetIndex(pk.OrbitIndex);
                    _uiExpand.transform.SetAsLastSibling();
                    SetTurretCountText(pk.TurretCount, pk.TurretCapacity);

                    break;
                case TurretExpandEntityPacket pk:
                    _uiExpand.SetActive(!pk.IsMaxExpand);
                    _uiExpand.SetInteractable(pk.IsActivateExpand);
                    _uiExpand.SetAsset(pk.ExpandAssetData);
                    _uiExpand.transform.SetAsLastSibling();
                    break;
            }
        }


        private void Clear()
        {
            for(int i = 0; i < _list.Count; i++)
            {
                _list[i].Hide();
            }
        }

        private UITurretBlock CreateBlock()
        {
            var block = Instantiate(_uiBlock);
            block.name = $"UI@TurretBlock{_list.Count}";
            block.transform.SetParent(_scrollRect.content);
            block.transform.localScale = Vector3.one;
            block.SetOnCommandPacketListener(OnCommandPacketEvent);
            _list.Add(block);
            return block;
        }

        #region ##### Listener #####

        private System.Action<ICommandPacket> _cmdEvent;
        public void SetOnCommandPacketListener(System.Action<ICommandPacket> act) => _cmdEvent = act;
        private void OnCommandPacketEvent(ICommandPacket pk) => _cmdEvent?.Invoke(pk);


        #endregion


    }
}