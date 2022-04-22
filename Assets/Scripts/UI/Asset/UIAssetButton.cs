namespace SDefence.UI
{
    using SDefence.Packet;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIAssetButton : Button
    {

        [SerializeField]
        private Text _text;

        private UIAssetContainer _uiAsset;

        protected override void OnEnable()
        {
            base.OnEnable();
            _uiAsset = GetComponentInChildren<UIAssetContainer>();
        }

        public void SetText(string text)
        {
            _text.text = text;
        }

        public void SetAsset()
        {

        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}