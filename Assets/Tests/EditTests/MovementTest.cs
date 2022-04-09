#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
namespace TestFrameworks
{
    using SDefence;
    using System.Collections;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;
    using SDefence.Movement;
    using SDefence.Movement.Usable;
    using Utility.Number;
    using Utility.IO;
    using GoogleSheetsToUnity;
    using System.Text;
    using System.Numerics;
    using Vector2 = UnityEngine.Vector2;
    using SDefence.Movement.Raw;

    public class MovementTest
    {

        #region ##### Test Attack #####


        public class TestMovementRawData
        {
            public float _startMovementValue;
            public float _increaseMovementValue;
            public float _increaseMovementRate;
            public float _startMoveAccelerationValue;
            public float _increaseMoveAccelerationValue;
            public float _increaseMoveAccelerationRate;
            public float _maximumAccelerateTime;
            public string _typeMovementAction;
            public float _movementWeight;
            public TYPE_MOVEMENT_ARRIVE _typeArrived;
            public TYPE_MOVEMENT_COLLISION _typeCollision;
            public TYPE_MOVEMENT_TARGET _typeTarget;
            public float _accuracy;

            public TestMovementRawData()
            {
                _startMovementValue = 1f;
                _increaseMovementValue = 0f;
                _increaseMovementRate = 0.1f;
                _startMoveAccelerationValue = 1f;
                _increaseMoveAccelerationValue = 0f;
                _increaseMoveAccelerationRate = 0.1f;
                _maximumAccelerateTime = 1f;
                _typeMovementAction = "Move";
                _movementWeight = 0f;
                _typeArrived = TYPE_MOVEMENT_ARRIVE.Destroy;
                _typeCollision = TYPE_MOVEMENT_COLLISION.Destroy;
                _typeTarget = TYPE_MOVEMENT_TARGET.Important;
                _accuracy = 0f;
            }

            public void SetData(string[] arr)
            {

            }


            public IMovementUsableData GetUsableData(int upgrade = 0)
            {
                var data = System.Activator.CreateInstance<MovementUsableData>();
                //data.SetData(_startMovementValue, _increaseMovementValue, _increaseMovementRate, _startMoveAccelerationValue, _increaseMoveAccelerationValue, _increaseMoveAccelerationRate, _maximumAccelerateTime, upgrade);
                data.SetData(_startMovementValue, _increaseMovementValue, _increaseMovementRate, upgrade);
                return data;
            }

            public IMovementActionUsableData GetActionUsableData()
            {
                System.Type type = System.Type.GetType(typeof(TestMovementActionUsableData).FullName);

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



        public class TestMovementUsableData : IMovementUsableData
        {
            private float _movementValue;
            //private float _accelerateValue;
            //private float _maximumAccelerateTime;

            public float MovementValue => _movementValue;
            //public float AccelerateValue => _accelerateValue;
            //public float MaximumAccelerateTime => _maximumAccelerateTime;

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
        #endregion
    


        #region ##### Test Attack Action #####

        public class TestMovementActionUsableData : IMovementActionUsableData
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
            //protected float Accurary => _accuracy;
            protected float NowMovementTime => _nowMovementTime;


            public void SetData(float accuracy)
            {
                _accuracy = accuracy;

                _isStart = false;
                _nowMovementTime = 0f;
            }

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

                var pos = Vector2.MoveTowards(moveable.NowPosition, target, data.MovementValue);
                moveable.SetPosition(pos);
            }


            #region ##### Listener #####

            private System.Action _startEvent;
            public void SetOnStartActionListener(System.Action act) => _startEvent = act;
            protected void OnStartEvent() => _startEvent?.Invoke();


            private System.Action _endedEvent;
            public void SetOnEndedActionListener(System.Action act) => _endedEvent = act;
            protected void OnEndedEvent() => _endedEvent?.Invoke();

            public void SetCollision()
            {
                throw new System.NotImplementedException();
            }


            //private System.Action<Vector2> _movementEvent;
            //public void SetOnMovementActionListener(System.Action<Vector2> act) => _movementEvent = act;
            //protected void OnMovementEvent(Vector2 pos) => _movementEvent?.Invoke(pos);

            #endregion
        }


        public class TestMoveable : IMoveable
        {
            private Vector2 _pos = Vector2.zero;
            public Vector2 NowPosition => _pos;

            public void SetPosition(Vector2 pos)
            {
                _pos = pos;
            }
        }



        #endregion


        [Test]
        public void MovementTest_Raw_SetData()
        {
            var raw = new TestMovementRawData();

            Debug.Log(raw._startMovementValue);
            Debug.Log(raw._increaseMovementValue);
            Debug.Log(raw._increaseMovementRate);
            Debug.Log(raw._startMoveAccelerationValue);
            Debug.Log(raw._increaseMoveAccelerationValue);
            Debug.Log(raw._increaseMoveAccelerationRate);
            Debug.Log(raw._maximumAccelerateTime);
            Debug.Log(raw._typeMovementAction);
            Debug.Log(raw._movementWeight);
            Debug.Log(raw._typeArrived);
            Debug.Log(raw._typeCollision);
            Debug.Log(raw._typeTarget);
            Debug.Log(raw._accuracy);

            Assert.AreEqual(raw._startMovementValue, 1f);
            Assert.AreEqual(raw._increaseMovementValue, 0f);
            Assert.AreEqual(raw._increaseMovementRate, 0.1f);
            Assert.AreEqual(raw._startMoveAccelerationValue, 1f);
            Assert.AreEqual(raw._increaseMoveAccelerationValue, 0f);
            Assert.AreEqual(raw._increaseMoveAccelerationRate, 0.1f);
            Assert.AreEqual(raw._maximumAccelerateTime, 1f);
            Assert.AreEqual(raw._typeMovementAction, "Move");
            Assert.AreEqual(raw._movementWeight, 0f);
            Assert.AreEqual(raw._typeArrived, TYPE_MOVEMENT_ARRIVE.Destroy);
            Assert.AreEqual(raw._typeCollision, TYPE_MOVEMENT_COLLISION.Destroy);
            Assert.AreEqual(raw._typeTarget, TYPE_MOVEMENT_TARGET.Important);
            Assert.AreEqual(raw._accuracy, 0f);

        }


        [Test]
        public void MovementTest_Raw_CreateUsableData()
        {
            var raw = new TestMovementRawData();
            var usable = raw.GetUsableData();

            Debug.Log(usable.MovementValue);
            //Debug.Log(usable.AccelerateValue);
            Assert.AreEqual(usable.MovementValue, 1f);
            //Assert.AreEqual(usable.AccelerateValue, 1f);
        }

        [Test]
        public void MovementTest_Raw_CreateActionUsableData()
        {
            var moveable = new TestMoveable();

            var raw = new TestMovementRawData();
            var usable = raw.GetUsableData();
            var actionUsable = raw.GetActionUsableData();

            actionUsable.SetOnStartActionListener(() =>
            {
                Debug.Log("Start");
                LogAssert.Expect(LogType.Log, "Start");
            });

            actionUsable.RunProcess(moveable, usable, 1f, Vector2.zero);
        }

        [Test]
        public void MovementTest_Usable_Upgrade()
        {
            var raw = new TestMovementRawData();
            var usable = raw.GetUsableData(1);

            Debug.Log(usable.MovementValue);
            //Debug.Log(usable.AccelerateValue);
            Assert.AreEqual((decimal)usable.MovementValue, (decimal)1.1f);
            //Assert.AreEqual((decimal)usable.AccelerateValue, (decimal)1.1f);

            usable = raw.GetUsableData(10);

            Debug.Log(usable.MovementValue);
            //Debug.Log(usable.AccelerateValue);
            Assert.AreEqual((decimal)usable.MovementValue, (decimal)2f);
            //Assert.AreEqual((decimal)usable.AccelerateValue, (decimal)2f);

            usable = raw.GetUsableData(100);

            Debug.Log(usable.MovementValue);
            //Debug.Log(usable.AccelerateValue);
            Assert.AreEqual((decimal)usable.MovementValue, (decimal)11f);
            //Assert.AreEqual((decimal)usable.AccelerateValue, (decimal)11f);

        }

        [UnityTest]
        public IEnumerator MovementTest_ActionUsable_StartedAction()
        {
            bool isRun = true;

            var moveable = new TestMoveable();

            var raw = new TestMovementRawData();
            var usable = raw.GetUsableData();
            var actionUsable = raw.GetActionUsableData();

            actionUsable.SetOnStartActionListener(() =>
            {
                Debug.Log("Start");
                isRun = false;
            });


            while (isRun)
            {
                actionUsable.RunProcess(moveable, usable, Time.deltaTime, Vector2.zero);
                yield return null;
            }
            LogAssert.Expect(LogType.Log, "Start");

            yield return null;
        }

        [UnityTest]
        public IEnumerator MovementTest_ActionUsable_EndedAction()
        {
            bool isRun = true;

            var moveable = new TestMoveable();

            var raw = new TestMovementRawData();
            var usable = raw.GetUsableData();
            var actionUsable = raw.GetActionUsableData();

            actionUsable.SetOnEndedActionListener(() =>
            {
                Debug.Log("End");
                isRun = false;
            });

            while (isRun)
            {
                actionUsable.RunProcess(moveable, usable, Time.deltaTime, Vector2.zero);
                yield return null;
            }
            LogAssert.Expect(LogType.Log, "End");
        }

        [UnityTest]
        public IEnumerator MovementTest_ActionUsable_Move()
        {
            bool isRun = true;

            var target = new Vector2(0f, 5f);

            var moveable = new TestMoveable();

            var raw = MovementRawData.Create();
            raw.SetData("Move", "0");
            //raw.SetData("Move", "0", "Destroy", "Destroy", "Important", "0");
            var usable = raw.GetUsableData();
            var actionUsable = raw.GetActionUsableData();

            actionUsable.SetOnEndedActionListener(() =>
            {
                Debug.Log("End");
                isRun = false;
            });

            while (isRun)
            {
                actionUsable.RunProcess(moveable, usable, 1f, target);
                Debug.Log(moveable.NowPosition);
                yield return null;
            }

            LogAssert.Expect(LogType.Log, "(0.0, 1.0)");
            LogAssert.Expect(LogType.Log, "(0.0, 2.0)");
            LogAssert.Expect(LogType.Log, "(0.0, 3.0)");
            LogAssert.Expect(LogType.Log, "(0.0, 4.0)");
            LogAssert.Expect(LogType.Log, "(0.0, 5.0)");
            yield return null;
        }
        //[UnityTest]
        //public IEnumerator MovementTest_ActionUsable_Spiral()
        //{
        //    bool isRun = true;
            
        //    var target = new Vector2(0f, 0f);

        //    var moveable = new TestMoveable();
        //    moveable.SetPosition(new Vector2(0f, 5f));

        //    var raw = MovementRawData.Create();
        //    raw.SetData("Spiral", "0", "Destroy", "Destroy", "Important", "0");
        //    var usable = raw.GetUsableData();
        //    var actionUsable = raw.GetActionUsableData();

        //    actionUsable.SetOnEndedActionListener(() =>
        //    {
        //        Debug.Log("End");
        //        isRun = false;
        //    });

        //    while (isRun)
        //    {
        //        actionUsable.RunProcess(moveable, usable, 1f, target);
        //        Debug.Log(moveable.NowPosition);
        //        yield return null;
        //    }

        //    LogAssert.Expect(LogType.Log, "(0.0, 1.0)");
        //    LogAssert.Expect(LogType.Log, "(0.0, 2.0)");
        //    LogAssert.Expect(LogType.Log, "(0.0, 3.0)");
        //    LogAssert.Expect(LogType.Log, "(0.0, 4.0)");
        //    LogAssert.Expect(LogType.Log, "(0.0, 5.0)");
        //    yield return null;
        //}
        //[UnityTest]
        //public IEnumerator MovementTest_ActionUsable_Curve()
        //{
        //    bool isRun = true;

        //    var target = new Vector2(5f, 5f);

        //    var moveable = new TestMoveable();
        //    moveable.SetPosition(new Vector2(0f, 0f));

        //    var raw = MovementRawData.Create();
        //    raw.SetData("Curve", "1", "Destroy", "Destroy", "Important", "0");
        //    var usable = raw.GetUsableData();
        //    var actionUsable = raw.GetActionUsableData();

        //    actionUsable.SetOnEndedActionListener(() =>
        //    {
        //        Debug.Log("End");
        //        isRun = false;
        //    });

        //    while (isRun)
        //    {
        //        actionUsable.RunProcess(moveable, usable, 1f, target);
        //        Debug.Log(moveable.NowPosition);
        //        yield return null;
        //    }

        //    LogAssert.Expect(LogType.Log, "(0.0, 1.0)");
        //    LogAssert.Expect(LogType.Log, "(0.0, 2.0)");
        //    LogAssert.Expect(LogType.Log, "(0.0, 3.0)");
        //    LogAssert.Expect(LogType.Log, "(0.0, 4.0)");
        //    LogAssert.Expect(LogType.Log, "(0.0, 5.0)");
        //    yield return null;
        //}


        //[UnityTest]
        //public IEnumerator MovementTest_ActionUsable_Orbit()
        //{
        //    bool isRun = true;

        //    var target = new Vector2(5f, 5f);

        //    var moveable = new TestMoveable();
        //    moveable.SetPosition(new Vector2(0f, 0f));

        //    var raw = MovementRawData.Create();
        //    raw.SetData("Orbit", "1", "Destroy", "Destroy", "Important", "0");
        //    var usable = raw.GetUsableData();
        //    var actionUsable = raw.GetActionUsableData();

        //    actionUsable.SetOnEndedActionListener(() =>
        //    {
        //        Debug.Log("End");
        //        isRun = false;
        //    });

        //    while (isRun)
        //    {
        //        actionUsable.RunProcess(moveable, usable, 1f, target);
        //        Debug.Log(moveable.NowPosition);
        //        yield return null;
        //    }

        //    LogAssert.Expect(LogType.Log, "(5.0, 5.0)");
        //    yield return null;
        //}

        [UnityTest]
        public IEnumerator MovementTest_ActionUsable_Direct()
        {
            bool isRun = true;

            var target = new Vector2(5f, 5f);

            var moveable = new TestMoveable();
            moveable.SetPosition(new Vector2(0f, 0f));

            var raw = MovementRawData.Create();
            raw.SetData("Direct", "0");
            var usable = raw.GetUsableData();
            var actionUsable = raw.GetActionUsableData();

            actionUsable.SetOnEndedActionListener(() =>
            {
                Debug.Log("End");
                isRun = false;
            });

            while (isRun)
            {
                actionUsable.RunProcess(moveable, usable, 1f, target);
                Debug.Log(moveable.NowPosition);
                yield return null;
            }
            LogAssert.Expect(LogType.Log, "(5.0, 5.0)");
            yield return null;
        }
        //[Test]
        //public void MovementTest_ActionUsable_Wave()
        //{
        //}
        //[Test]
        //public void MovementTest_ActionUsable_Custom()
        //{
        //}

        //[UnityTest]
        //public IEnumerator MovementTest_ActionUsable_Repeat()
        //{
        //    bool isRun = true;

        //    var target = new Vector2(5f, 5f);

        //    var moveable = new TestMoveable();
        //    moveable.SetPosition(new Vector2(0f, 0f));

        //    var raw = MovementRawData.Create();
        //    raw.SetData("Direct", "1", "Repeat", "Destroy", "Important", "0");
        //    var usable = raw.GetUsableData();
        //    var actionUsable = raw.GetActionUsableData();

        //    actionUsable.SetOnEndedActionListener(() =>
        //    {
        //        Debug.Log("End");
        //        isRun = false;
        //    });

        //    while (isRun)
        //    {
        //        actionUsable.RunProcess(moveable, usable, 1f, target);
        //        Debug.Log(moveable.NowPosition);
        //        yield return null;
        //    }
        //    LogAssert.Expect(LogType.Log, "(5.0, 5.0)");
        //    yield return null;
        //}
        //[UnityTest]
        //public IEnumerator MovementTest_ActionUsable_Return()
        //{
        //    bool isRun = true;

        //    var target = new Vector2(5f, 5f);

        //    var moveable = new TestMoveable();
        //    moveable.SetPosition(new Vector2(0f, 0f));

        //    var raw = MovementRawData.Create();
        //    raw.SetData("Direct", "1", "Return", "Destroy", "Important", "0");
        //    var usable = raw.GetUsableData();
        //    var actionUsable = raw.GetActionUsableData();

        //    actionUsable.SetOnEndedActionListener(() =>
        //    {
        //        Debug.Log("End");
        //        isRun = false;
        //    });

        //    while (isRun)
        //    {
        //        actionUsable.RunProcess(moveable, usable, 1f, target);
        //        Debug.Log(moveable.NowPosition);
        //        yield return null;
        //    }
        //    LogAssert.Expect(LogType.Log, "(5.0, 5.0)");
        //    yield return null;
        //}
        //[UnityTest]
        //public IEnumerator MovementTest_ActionUsable_Stop()
        //{
        //    bool isRun = true;

        //    var target = new Vector2(5f, 5f);

        //    var moveable = new TestMoveable();
        //    moveable.SetPosition(new Vector2(0f, 0f));

        //    var raw = MovementRawData.Create();
        //    raw.SetData("Direct", "1", "Stop", "Destroy", "Important", "0");
        //    var usable = raw.GetUsableData();
        //    var actionUsable = raw.GetActionUsableData();

        //    actionUsable.SetOnEndedActionListener(() =>
        //    {
        //        Debug.Log("End");
        //        isRun = false;
        //    });

        //    while (isRun)
        //    {
        //        actionUsable.RunProcess(moveable, usable, 1f, target);
        //        Debug.Log(moveable.NowPosition);
        //        yield return null;
        //    }
        //    LogAssert.Expect(LogType.Log, "(5.0, 5.0)");
        //    yield return null;
        //}


        [UnityTest]
        public IEnumerator MovementTest_Generator_CreateData()
        {
            bool isRun = true;

            var search = new GSTU_Search("1SzGjvMX1kac6LzvmQHXQRmNj_7MYDjspwF-wpWJuWks", "Test_Movement_Data");

            SpreadsheetManager.Read(search, sheet =>
            {
                var movement = MovementRawData.Create();

                movement.SetData(sheet["NormalBullet", "StartMovementValue"].value, sheet["NormalBullet", "IncreaseMovementValue"].value, sheet["NormalBullet", "IncreaseMovementRate"].value);
                movement.SetData(sheet["NormalBullet", "TypeMovement"].value, sheet["NormalBullet", "Accuracy"].value);

                var movementUsable = movement.GetUsableData();

                Debug.Log(movementUsable.MovementValue);

                Assert.AreEqual(movementUsable.MovementValue, 1f);

                isRun = false;

            });

            while (isRun)
            {
                yield return null;
            }
        }
    }
}
#endif