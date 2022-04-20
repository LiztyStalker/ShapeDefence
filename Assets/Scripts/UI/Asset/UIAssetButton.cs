namespace SDefence.UI
{
    using SDefence.Packet;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIAssetButton : Button
    {

        [SerializeField]
        private Text _text;

        private UIAsset _uiAsset;

        protected override void OnEnable()
        {
            base.OnEnable();
            _uiAsset = GetComponentInChildren<UIAsset>();
        }

        public void SetText(string text)
        {
            _text.text = text;
        }

        public void SetAsset()
        {

        }
    }
}