namespace SDefence.UI
{
    using UnityEngine.UI;
    using UnityEngine;
    using PoolSystem;
    using Actor;
    using Durable.Usable;

    public class UIDurableBlock : MonoBehaviour, IPoolElement
    {

        private const float BLOCK_LIFE_TIME = 1f;

        [SerializeField]
        private Slider _shieldSlider;

        [SerializeField]
        private Text _shieldText;

        [SerializeField]
        private Slider _hpSlider;

        [SerializeField]
        private Text _healthText;

        [SerializeField]
        private bool _isHold;

        private IActor _actor;

        private float _nowTime;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            _actor = null;
            gameObject.SetActive(false);
        }
        public void SetData(IActor actor)
        {
            _actor = actor;

            _hpSlider.value = actor.GetDurableRate<HealthDurableUsableData>();
            _shieldSlider.value = actor.GetDurableRate<ShieldDurableUsableData>();

            if (_isHold)
            {
                _shieldText.gameObject.SetActive(_shieldSlider.value > 0f);

                _healthText.text = actor.GetDurableValue<HealthDurableUsableData>();
                _shieldText.text = actor.GetDurableValue<ShieldDurableUsableData>();
            }
            else
            {
                _nowTime = BLOCK_LIFE_TIME;
            }
        }

        private void Update()
        {
            if (!_isHold)
            {
                _nowTime -= Time.deltaTime;
                transform.position = _actor.NowPosition;
                if (_nowTime <= 0f)
                {
                    OnRetrieveEvent();
                }
            }
        }

        #region ##### Listener #####

        private System.Action<IActor> _retrieveEvent;
        public void SetOnRetrieveListener(System.Action<IActor> act) => _retrieveEvent = act;
        private void OnRetrieveEvent() => _retrieveEvent?.Invoke(_actor);

        #endregion
    }
}