namespace SDefence
{
    using SDefence.Attack;
    using SDefence.Attack.Usable;
    using SDefence.Durable;
    using SDefence.Durable.Usable;
    using SDefence.Recovery;
    using SDefence.Recovery.Usable;
    using System.Numerics;

    public struct UniversalUsableData
    {
        private BigDecimal _value;
        public BigDecimal Value { get => _value; private set => _value = value; }

        public UniversalUsableData(BigDecimal value)
        {
            _value = value;
        }
        public bool IsZero => Value.IsZero;

        public void SetZero() => Value = 0;
        public void Set(int value) => Value = value;
        public void Set(IAttackUsableData value) => Value = ((AttackUsableData)value).Value;
        public void Set(IRecoveryUsableData value) => Value = ((AbstractRecoveryUsableData)value).Value;
        public void Set(IDurableUsableData value) => Value = ((AbstractDurableUsableData)value).Value;

        public void Add(int value) => Value += value;
        public void Add(IAttackUsableData value) => Value += ((AttackUsableData)value).Value;
        public void Add(IRecoveryUsableData value) => Value += ((AbstractRecoveryUsableData)value).Value;
        public void Add(IDurableUsableData value) => Value += ((AbstractDurableUsableData)value).Value;
        public void Add(UniversalUsableData value) => Value += value.Value;

        public void Subject(int value) => Value -= value;
        public void Subject(IAttackUsableData value) => Value -= ((AttackUsableData)value).Value;
        public void Subject(IRecoveryUsableData value) => Value -= ((AbstractRecoveryUsableData)value).Value;
        public void Subject(IDurableUsableData value) => Value -= ((AbstractDurableUsableData)value).Value;
        public void Subject(UniversalUsableData value) => Value -= value.Value;


        public int Compare(int data)
        {
            if (Value - data > 0) return -1;
            else if (Value - data < 0) return 1;
            return 0;
        }

        public int Compare(IAttackUsableData data)
        {
            if (Value - ((AttackUsableData)data).Value > 0) return -1;
            else if (Value - ((AttackUsableData)data).Value < 0) return 1;
            return 0;
        }

        public int Compare(IRecoveryUsableData data)
        {
            if (Value - ((AbstractRecoveryUsableData)data).Value > 0) return -1;
            else if (Value - ((AbstractRecoveryUsableData)data).Value < 0) return 1;
            return 0;
        }

        public int Compare(IDurableUsableData data)
        {
            if (Value - ((AbstractDurableUsableData)data).Value > 0) return -1;
            else if (Value - ((AbstractDurableUsableData)data).Value < 0) return 1;
            return 0;
        }
    }
}