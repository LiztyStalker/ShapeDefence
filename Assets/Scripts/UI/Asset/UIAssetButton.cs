namespace SDefence.UI
{
    using Asset;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIAssetButton : Button
    {

        [SerializeField]
        private Text _text;

        [SerializeField]
        private UIAssetContainer _uiAsset;

        public void Initialize()
        {
            _uiAsset.Initialize();
        }

        public void CleanUp()
        {
            _uiAsset.CleanUp();
        }

        public void SetText(string text)
        {
            _text.text = text;
        }

        public void SetAsset(IAssetUsableData assetData)
        {
            _uiAsset.SetData(assetData);
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}