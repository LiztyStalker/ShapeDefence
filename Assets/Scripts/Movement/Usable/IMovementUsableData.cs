namespace SDefence.Movement
{
    public interface IMovementUsableData
    {
        public float MovementValue { get; }
//        public float AccelerateValue { get; }
//        public float MaximumAccelerateTime { get; }
        //public void SetData(float startValue, float increaseValue, float increaseRate, float startAccelerateValue, float increaseAccelerateValue, float increaseAccelerateRate, float maximumAccelerateTime, int upgrade);
        public void SetData(float startValue, float increaseValue, float increaseRate, int upgrade);//, float startAccelerateValue, float increaseAccelerateValue, float increaseAccelerateRate, float maximumAccelerateTime, int upgrade);
    }
}