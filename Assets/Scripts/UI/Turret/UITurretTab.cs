namespace SDefence.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UITurretTab : Button
    {
        [SerializeField]
        private Text _text;

        private int _index;

        public void SetIndex(int index) => _index = index;
        public void SetText(string text) => _text.text = text;

        protected override void Awake()
        {
            onClick.AddListener(OnClickEvent);
        }

        protected override void OnDestroy()
        {
            onClick.RemoveListener(OnClickEvent);
        }

        #region ##### Listener #####

        private System.Action<int> _clickEvent;
        public void SetOnClickListener(System.Action<int> act) => _clickEvent = act;
        private void OnClickEvent() => _clickEvent?.Invoke(_index);

        #endregion
    }
}