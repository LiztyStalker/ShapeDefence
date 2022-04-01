namespace SDefence.Movement.Raw {

    using UnityEngine;
    using Usable;



    [System.Serializable]
    public class MovementRawData
    {
        [SerializeField]
        private float _startMovementValue;

        [SerializeField]
        private float _increaseMovementValue;

        [SerializeField]
        private float _increaseMovementRate;

        //[SerializeField]
        //private float _startMoveAccelerationValue;

        //[SerializeField]
        //private float _increaseMoveAccelerationValue;

        //[SerializeField]
        //private float _increaseMoveAccelerationRate;

        //[SerializeField]
        //private float _maximumAccelerateTime;

        [SerializeField]
        private string _typeMovementAction;

        //[SerializeField]
        //private float _movementWeight;

        //[SerializeField]
        //private TYPE_MOVEMENT_ARRIVE _typeArrived;

        //[SerializeField]
        //private TYPE_MOVEMENT_COLLISION _typeCollision;

        //[SerializeField]
        //private TYPE_MOVEMENT_TARGET _typeTarget;

        [SerializeField]
        private float _accuracy;

     

#if UNITY_EDITOR

        public static MovementRawData Create()
        {
            return new MovementRawData();
        }
        private MovementRawData()
        {
            _startMovementValue = 1f;
            _increaseMovementValue = 0f;
            _increaseMovementRate = 0.1f;
            //_startMoveAccelerationValue = 1f;
            //_increaseMoveAccelerationValue = 0f;
            //_increaseMoveAccelerationRate = 0.1f;
            //_maximumAccelerateTime = 1f;
            _typeMovementAction = "Move";
            //_movementWeight = 0f;
            //_typeArrived = TYPE_MOVEMENT_ARRIVE.Destroy;
            //_typeCollision = TYPE_MOVEMENT_COLLISION.Destroy;
            //_typeTarget = TYPE_MOVEMENT_TARGET.Important;
            _accuracy = 0f;
        }


        public void SetData(
            string startMovementValue,
            string increaseMovementValue,
            string increaseMovementRate)
        {
            _startMovementValue = float.Parse(startMovementValue);
            _increaseMovementValue = float.Parse(increaseMovementValue);
            _increaseMovementRate = float.Parse(increaseMovementRate);
        }

        //public void SetData(
        //    string startMovementValue, 
        //    string increaseMovementValue, 
        //    string increaseMovementRate, 
        //    string startMoveAccelerationValue, 
        //    string increaseMoveAccelerationValue,
        //    string increaseMoveAccelerationRate,
        //    string maximumAccelerateTime)
        //{
        //    _startMovementValue = float.Parse(startMovementValue);
        //    _increaseMovementValue = float.Parse(increaseMovementValue);
        //    _increaseMovementRate = float.Parse(increaseMovementRate);
        //    _startMoveAccelerationValue = float.Parse(startMoveAccelerationValue);
        //    _increaseMoveAccelerationValue = float.Parse(increaseMoveAccelerationValue);
        //    _increaseMoveAccelerationRate = float.Parse(increaseMoveAccelerationRate);
        //    _maximumAccelerateTime = float.Parse(maximumAccelerateTime);
        //}

        public void SetData(
            string typeMovementAction,
//            string movementWeight,
//            string typeArrived,
//            string typeCollision,
//            string typeTarget,
            string accuracy)
        {
            _typeMovementAction = typeMovementAction;
            _accuracy = float.Parse(accuracy);
        }

        //public void SetData(
        //    string typeMovementAction,
        //    string movementWeight,
        //    string typeArrived,
        //    string typeCollision,
        //    string typeTarget,
        //    string accuracy)
        //{
        //    _typeMovementAction = typeMovementAction;
        //    _movementWeight = float.Parse(movementWeight);
        //    _typeArrived = (TYPE_MOVEMENT_ARRIVE)System.Enum.Parse(typeof(TYPE_MOVEMENT_ARRIVE), typeArrived);
        //    _typeCollision = (TYPE_MOVEMENT_COLLISION)System.Enum.Parse(typeof(TYPE_MOVEMENT_COLLISION), typeCollision);
        //    _typeTarget = (TYPE_MOVEMENT_TARGET)System.Enum.Parse(typeof(TYPE_MOVEMENT_TARGET), typeTarget);
        //    _accuracy = float.Parse(accuracy);
        //}

#endif

        public IMovementUsableData GetUsableData(int upgrade = 0)
        {
            var data = System.Activator.CreateInstance<MovementUsableData>();
            //data.SetData(_startMovementValue, _increaseMovementValue, _increaseMovementRate, _startMoveAccelerationValue, _increaseMoveAccelerationValue, _increaseMoveAccelerationRate, _maximumAccelerateTime, upgrade);
            data.SetData(_startMovementValue, _increaseMovementValue, _increaseMovementRate, upgrade);
            return data;
        }

        public IMovementActionUsableData GetActionUsableData()
        {
            System.Type type = System.Type.GetType($"SDefence.Movement.Usable.{_typeMovementAction}MovementActionUsableData");

            if (type != null)
            {
                var data = (IMovementActionUsableData)System.Activator.CreateInstance(type);
                //data.SetData(_movementWeight, _typeArrived, _typeCollision, _typeTarget, _accuracy);
                data.SetData(_accuracy);
                return data;
            }
#if UNITY_EDITOR
            else
            {
                throw new System.Exception($"{_typeMovementAction} is not found Type");
            }
#endif
        }

    }
}