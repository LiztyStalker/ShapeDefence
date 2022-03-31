namespace SDefence.Attack.Raw {

    using UnityEngine;
    using Usable;

    [System.Serializable]
    public class AttackActionRawData
    {
        [SerializeField]
        private float _attackRange;

        [SerializeField]
        private int _attackCount;

        [SerializeField]
        private float _attackDelayTime;

        [SerializeField]
        private bool _isOverlap;

        public float AttackRange => _attackRange;
        public int AttackCount => _attackCount;
        public float AttackDelayTime => _attackDelayTime;
        public bool IsOverlap => _isOverlap;


#if UNITY_EDITOR

        public static AttackActionRawData Create()
        {
            return new AttackActionRawData();
        }
        private AttackActionRawData()
        {
            _attackRange = 5f;
            _attackCount = 1;
            _attackDelayTime = 0f;
            _isOverlap = false;
        }

        public void SetData(string attackRange, string attackCount, string attackDelayTime, string isOverlap)
        {
            _attackRange = float.Parse(attackRange);
            _attackCount = int.Parse(attackCount);
            _attackDelayTime = float.Parse(attackDelayTime);
            _isOverlap = bool.Parse(isOverlap);
        }
#endif

        public AttackActionUsableData GetUsableData()
        {
            var data = System.Activator.CreateInstance<AttackActionUsableData>();
            data.SetData(this);
            return data;
        }
    }
}