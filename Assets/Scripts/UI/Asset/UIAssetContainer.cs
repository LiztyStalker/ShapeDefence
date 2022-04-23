namespace SDefence.UI
{
    using Packet;
    using Storage;
    using System.Collections.Generic;
    using UnityEngine.UI;
    using UnityEngine;

    public class UIAssetContainer : MonoBehaviour, IEntityPacketUser
    {
        private Dictionary<string, UIAssetBlock> _dic;

        public void Initialize()
        {
            _dic = new Dictionary<string, UIAssetBlock>();

            var frame = transform.GetComponentInChildren<LayoutGroup>(true);
            var tr = frame.transform;

            for (int i = 0; i < tr.childCount; i++)
            {
                var block = tr.GetChild(i).GetComponent<UIAssetBlock>();
                if(block != null) _dic.Add(block.AssetKey, block);
            }

            foreach (var key in _dic.Keys)
            {
                var icon = DataStorage.Instance.GetDataOrNull<Sprite>(key, "Icon_Asset");
                _dic[key].SetIcon(icon);
            }
        }

        public void CleanUp()
        {
            _dic.Clear();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnEntityPacketEvent(IEntityPacket packet)
        {
            if(packet is AssetEntityPacket)
            {
                var pk = (AssetEntityPacket)packet;
                var entity = pk.Entity;

                //NullException ���� ����
                try
                {
                    foreach (var key in _dic.Keys)
                    {
                        if (entity.HasKey(key))
                        {
                            _dic[key].SetText(entity.GetValue(key));
                        }
                    }
                }
                catch
                {

                }
            }
        }
    }
}