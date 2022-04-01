namespace SDefence.Movement.Usable
{
    using Raw;
    using UnityEngine;

    public abstract class AbstractMovementActionUsableData : IMovementActionUsableData
    {
        //private float _movementWeight;
        //private TYPE_MOVEMENT_ARRIVE _typeArrived;
        //private TYPE_MOVEMENT_COLLISION _typeCollision;
        //private TYPE_MOVEMENT_TARGET _typeTarget;
        private float _accuracy;

        private float _nowMovementTime = 0f;
        private bool _isStart = false;



        //protected float MovementWeight => _movementWeight;
        //protected TYPE_MOVEMENT_ARRIVE TypeArrived => _typeArrived;
        //protected TYPE_MOVEMENT_COLLISION TypeCollision => _typeCollision;
        //protected TYPE_MOVEMENT_TARGET TypeTarget => _typeTarget;
        protected float Accurary => _accuracy;
        protected float NowMovementTime => _nowMovementTime;



        //public void SetData(float movementWeight, TYPE_MOVEMENT_ARRIVE typeArrived, TYPE_MOVEMENT_COLLISION typeCollision, TYPE_MOVEMENT_TARGET typeTarget, float accuracy)
        //{
        //    _movementWeight = movementWeight;
        //    _typeArrived = typeArrived;
        //    _typeCollision = typeCollision;
        //    _typeTarget = typeTarget;
        //    _accuracy = accuracy;

        //    _isStart = false;
        //    _nowMovementTime = 0f;
        //}

        public void SetData(float accuracy)
        {
            _accuracy = accuracy;

            _isStart = false;
            _nowMovementTime = 0f;
        }

        public virtual void RunProcess(IMoveable moveable, IMovementUsableData data, float deltaTime, Vector2 target)
        {
            if (!_isStart)
            {
                OnStartEvent();
                _isStart = true;
            }

            _nowMovementTime += deltaTime;

            if (Vector2.Distance(moveable.NowPosition, target) < _accuracy + 0.01f)
            {
                OnEndedEvent();
                //switch (TypeArrived)
                //{
                //    case TYPE_MOVEMENT_ARRIVE.Destroy:
                //        OnEndedEvent();
                //        break;
                //    case TYPE_MOVEMENT_ARRIVE.Repeat:
                //        break;
                //    case TYPE_MOVEMENT_ARRIVE.Return:
                //        break;
                //    case TYPE_MOVEMENT_ARRIVE.Stop:
                //        OnEndedEvent();
                //        break;
                //}
            }
        }


        #region ##### Listener #####

        private System.Action _startEvent;
        public void SetOnStartActionListener(System.Action act) => _startEvent = act;
        protected void OnStartEvent() => _startEvent?.Invoke();


        private System.Action _endedEvent;
        public void SetOnEndedActionListener(System.Action act) => _endedEvent = act;
        protected void OnEndedEvent() => _endedEvent?.Invoke();


        //private System.Action<Vector2> _movementEvent;
        //public void SetOnMovementActionListener(System.Action<Vector2> act) => _movementEvent = act;
        //protected void OnMovementEvent(Vector2 pos) => _movementEvent?.Invoke(pos);

        #endregion
    }
}