namespace SDefence.Attack.Usable
{
    using Raw;

    public class AttackActionUsableData
    {

        private AttackActionRawData _raw;
        private int _nowAttackCount;
        private float _nowAttackDelayTime;
        private bool _isStart = false;

        public void SetData(AttackActionRawData raw)
        {
            _raw = raw;
            _nowAttackCount = 0;
            _nowAttackDelayTime = 0f;
            _isStart = false;
        }

        public void RunProcess(float deltaTime)
        {
            if (!_isStart)
            {
                OnStartEvent();
                _isStart = true;
            }

            _nowAttackDelayTime += deltaTime;
            if(_nowAttackDelayTime >= _raw.AttackDelayTime)
            {
                OnAttackEvent();
                _nowAttackDelayTime -= _raw.AttackDelayTime;
                _nowAttackCount++;
            }

            if(_nowAttackCount >= _raw.AttackCount)
            {
                OnEndedEvent();
            }
        }


        #region ##### Listener #####

        private System.Action _startEvent;
        public void SetOnStartActionListener(System.Action act) => _startEvent = act;
        private void OnStartEvent() => _startEvent?.Invoke();


        private System.Action _endedEvent;
        public void SetOnEndedActionListener(System.Action act) => _endedEvent = act;
        private void OnEndedEvent() => _endedEvent?.Invoke();


        private System.Action<float, bool> _attackEvent;
        public void SetOnAttackActionListener(System.Action<float, bool> act) => _attackEvent = act;
        private void OnAttackEvent() => _attackEvent?.Invoke(_raw.AttackRange, _raw.IsOverlap);

        #endregion
    }
}