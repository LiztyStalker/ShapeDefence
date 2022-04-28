namespace SDefence.UI
{
    using Packet;
    using UnityEngine;

    public class UIAsset : MonoBehaviour, IBattlePacketUser, IEntityPacketUser
    {

        private UIAssetContainer _uiAssetContainer;

        public void Initialize()
        {
            _uiAssetContainer = GetComponentInChildren<UIAssetContainer>(true);

            _uiAssetContainer.Initialize();
        }

        public void CleanUp() 
        {
            _uiAssetContainer.CleanUp();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        //AssetEntityPacket
        public void OnEntityPacketEvent(IEntityPacket packet)
        {
            _uiAssetContainer.OnEntityPacketEvent(packet);
        }

        //AssetBattlePacket
        public void OnBattlePacketEvent(IBattlePacket packet)
        {
            _uiAssetContainer.OnBattlePacketEvent(packet);
        }
    }
}