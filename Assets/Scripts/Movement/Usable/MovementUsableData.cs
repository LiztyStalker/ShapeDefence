namespace SDefence.Movement.Usable
{
    using Utility.Number;
    public class MovementUsableData : IMovementUsableData
    {
        private float _movementValue;
//        private float _accelerateValue;
//        private float _maximumAccelerateTime;

        public float MovementValue => _movementValue;
//        public float AccelerateValue => _accelerateValue;
//        public float MaximumAccelerateTime => _maximumAccelerateTime;

        public void SetData(float startValue, float increaseValue, float increaseRate, int upgrade)
        {
            _movementValue = startValue;
            _movementValue = NumberDataUtility.GetIsolationInterest(startValue, increaseValue, increaseRate, upgrade);
        }

        //public void SetData(float startValue, float increaseValue, float increaseRate, float startAccelerateValue, float increaseAccelerateValue, float increaseAccelerateRate, float maximumAccelerateTime, int upgrade)
        //{
        //    _movementValue = startValue;
        //    _movementValue = NumberDataUtility.GetIsolationInterest(startValue, increaseValue, increaseRate, upgrade);

        //    _accelerateValue = startAccelerateValue;
        //    _accelerateValue = NumberDataUtility.GetIsolationInterest(startValue, increaseAccelerateValue, increaseAccelerateRate, upgrade);

        //    _maximumAccelerateTime = maximumAccelerateTime;
        //} 
    }
}