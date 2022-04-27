namespace SDefence.Attack
{
    public interface IAttackUsableData
    {
        public float Delay { get; }
        public float Range { get; }
        public bool IsZero { get; }
        public void Set(IAttackUsableData value);
        public void SetAttack(string startValue, string increaseValue, string increaseRate, int upgrade);
        public void SetDelay(string startValue, string decreaseValue, string decreaseRate, int upgrade);
        public void SetRange(string startValue, string increaseValue, string increaseRate, int upgrade);
        public string ToString(string format);
        public UniversalUsableData CreateUniversalUsableData();
        public IAttackUsableData Clone();
    }
}