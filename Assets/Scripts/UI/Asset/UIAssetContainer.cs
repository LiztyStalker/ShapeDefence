namespace SDefence.UI
{
    using Packet;
    using Storage;
    using System.Collections.Generic;
    using UnityEngine.UI;
    using UnityEngine;
    using Asset.Entity;
    using Asset;

    public class UIAssetContainer : MonoBehaviour, IBattlePacketUser, IEntityPacketUser
    {
        private static UIAssetBlock AssetBlock;

        private Dictionary<string, UIAssetBlock> _baseDic; //setting

        private Dictionary<string, UIAssetBlock> _dic; //instance

        private Transform frameTr;

        public void Initialize()
        {
            _baseDic = new Dictionary<string, UIAssetBlock>();
            _dic = new Dictionary<string, UIAssetBlock>();

            var frame = transform.GetChild(0);
            frameTr = frame.transform;

            for (int i = 0; i < frameTr.childCount; i++)
            {
                var block = frameTr.GetChild(i).GetComponent<UIAssetBlock>();
                if(block != null) _baseDic.Add(block.AssetKey, block);
            }

            foreach (var key in _baseDic.Keys)
            {
                _baseDic[key].SetIcon(GetIcon(key));
            }

            if(AssetBlock == null)
            {
                var block = DataStorage.Instance.GetDataOrNull<GameObject>("UI@AssetBlock");
                AssetBlock = block.GetComponent<UIAssetBlock>();                
            }
        }

        public void CleanUp()
        {
            _baseDic.Clear();
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

        public void SetData(AssetUsableEntity assetEntity)
        {
            if (_baseDic.Count > 0)
            {
                try
                {
                    foreach (var key in _baseDic.Keys)
                    {
                        if (assetEntity.HasKey(key))
                        {
                            _baseDic[key].SetText(assetEntity.GetValue(key));
                        }
                        else
                        {
                            _baseDic[key].SetText("0");
                        }
                    }
                }
                catch
                {

                }
            }
            else
            {
                for(int i = 0; i < assetEntity.Keys.Length; i++) 
                {
                    var key = assetEntity.Keys[i];
                    InstanceAssetBlock(key, assetEntity.GetValue(key));
                }
            }
        }
        
        public void SetData(IAssetUsableData assetData)
        {
            var key = assetData.Key;
            if (_baseDic.Count > 0)
            {
                if (_baseDic.ContainsKey(key))
                    _baseDic[key].SetText(assetData.ToString());
            }
            else
            {
                InstanceAssetBlock(key, assetData.ToString());
            }
        }

        private void InstanceAssetBlock(string key, string value)
        {
            if (!_dic.ContainsKey(key))
            {
                var block = Instantiate(AssetBlock);
                block.transform.SetParent(frameTr);
                block.transform.localScale = Vector3.one;
                block.name = $"UI@AssetBlock_{key}";
                block.SetIcon(GetIcon(key));
                _dic.Add(key, block);
            }
            _dic[key].SetText(value);
        }

        private Sprite GetIcon(string key) => DataStorage.Instance.GetDataOrNull<Sprite>(key, "Icon_Asset");

        //AssetEntityPacket
        public void OnEntityPacketEvent(IEntityPacket packet)
        {
            if(packet is AssetEntityPacket)
            {
                var pk = (AssetEntityPacket)packet;
                var entity = pk.Entity;

                SetData(entity);               
            }
        }

        //AssetBattlePacket
        public void OnBattlePacketEvent(IBattlePacket packet)
        {
            if(packet is AssetBattlePacket)
            {
                var pk = (AssetBattlePacket)packet;
                var entity = pk.AssetEntity;

                SetData(entity);
            }
        }
    }
}