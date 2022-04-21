namespace SDefence.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UIAssetBlock : MonoBehaviour
    {
        [SerializeField]
        private Text _text;

        [SerializeField]
        private Image _icon;

        [SerializeField]
        private string _assetKey;

        public string AssetKey => _assetKey;

        public void SetIcon(Sprite icon)
        {
            _icon.sprite = icon;
        }

        public void SetText(string text)
        {
            _text.text = text;
        }
    }
}