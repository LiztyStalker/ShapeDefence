namespace SDefence.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UILevelWave : MonoBehaviour
    {
        [SerializeField]
        private Slider _waveSlider;

        [SerializeField]
        private Text _waveText;

        [SerializeField]
        private Image _bossIcon;

        private float _targetValue;

        public void Initialize()
        {

        }

        public void CleanUp()
        {

        }

        public void SetIcon(Sprite sprite)
        {
            //BossIcon
            _bossIcon.sprite = sprite;
        }

        public void SetData(int wave, int maxWave)
        {
            //LevelWave
            _waveText.text = $"{wave + 1}/{maxWave}";
            _targetValue = (float)wave / (float)(maxWave - 1);
            if (_targetValue < 0.01f)
                _waveSlider.value = _targetValue;
        }

        private void Update()
        {
            _waveSlider.value = Mathf.Lerp(_waveSlider.value, _targetValue, Time.deltaTime);
        }
    }
}