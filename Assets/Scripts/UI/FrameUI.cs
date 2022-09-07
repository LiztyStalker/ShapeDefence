namespace Utility.UI
{
    using UnityEngine;

    public class FrameUI : MonoBehaviour
    {
        private RectTransform _rectTransform;

        private RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        public void Initialize()
        {
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}