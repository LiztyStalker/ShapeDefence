#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
namespace TestFrameworks
{
    using SDefence;
    using System.Collections;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;
    using SDefence.Attack;
    using SDefence.Attack.Usable;
    using Utility.Number;
    using Utility.IO;
    using GoogleSheetsToUnity;
    using SDefence.Asset.Raw;
    using System.Text;
    using System.Numerics;
    using SDefence.Attack.Raw;

    public class AttackTest
    {

        #region ##### Test Attack #####
        public class TestAttackRawData
        {
            public string StartAttackValue;
            public string IncreaseAttackValue;
            public string IncreaseAttackRate;
            public string StartAttackDelayValue;
            public string DecreaseAttackDelayValue;
            public string DecreaseAttackDelayRate;


            public TestAttackRawData(string startValue, string increaseValue, string increaseRate)
            {
                StartAttackValue = startValue;
                IncreaseAttackValue = increaseValue;
                IncreaseAttackRate = increaseRate;
                StartAttackDelayValue = "1";
                DecreaseAttackDelayValue = "0";
                DecreaseAttackDelayRate = "0.1";
            }

            public TestAttackRawData(string startValue, string increaseValue, string increaseRate, string startDelayValue, string decreaseDelayValue, string decreaseDelayRate)
            {
                StartAttackValue = startValue;
                IncreaseAttackValue = increaseValue;
                IncreaseAttackRate = increaseRate;
                StartAttackDelayValue = startDelayValue;
                DecreaseAttackDelayValue = decreaseDelayValue;
                DecreaseAttackDelayRate = decreaseDelayRate;
            }

            public TestAttackRawData()
            {
                StartAttackValue = "10";
                IncreaseAttackValue = "1";
                IncreaseAttackRate = "0.1";
                StartAttackDelayValue = "1";
                DecreaseAttackDelayValue = "0";
                DecreaseAttackDelayRate = "0.1";
            }
            internal IAttackUsableData GetUsableData(int upgrade = 0)
            {
                var data = System.Activator.CreateInstance<AttackUsableData>();
                data.SetData(StartAttackValue, IncreaseAttackValue, IncreaseAttackRate, StartAttackDelayValue, DecreaseAttackDelayValue, DecreaseAttackDelayRate, upgrade);
                return data;
            }
        }



        public class TestAttackUsableData : IAttackUsableData
        {

            private BigDecimal _value;
            public BigDecimal Value { get => _value; set => _value = value; }


            private float _delay;
            public float Delay { get => _delay; set => _delay = value; }


            public bool IsZero => Value.IsZero;


            public virtual void Set(IAttackUsableData value)
            {
                Value = ((TestAttackUsableData)value).Value;
                Delay = ((TestAttackUsableData)value).Delay;
            }

            public void SetData(string startValue, string increaseValue, string increaseRate, string startDelayValue, string decreaseDelayValue, string decreaseDelayRate, int upgrade)
            {
                var incVal = int.Parse(increaseValue);
                var incRate = float.Parse(increaseRate);

                Value = new BigDecimal(startValue);
                Value = NumberDataUtility.GetIsolationInterest(Value, incVal, incRate, upgrade);


                var sDelay = float.Parse(startDelayValue);
                var decVal = float.Parse(decreaseDelayValue);
                var decRate = float.Parse(decreaseDelayRate);

                Delay = sDelay;
                Delay = NumberDataUtility.GetIsolationInterest(Delay, decVal, decRate, upgrade);
            }


            protected static T Create<T>() where T : IAttackUsableData
            {
                return System.Activator.CreateInstance<T>();
            }

            private static IAttackUsableData Create(System.Type type)
            {
                return (IAttackUsableData)System.Activator.CreateInstance(type);
            }



            public string ToString(string format) => Value.ToString(format);
            public override string ToString() => Value.ToString();


            public UniversalUsableData CreateUniversalUsableData() => new UniversalUsableData(Value);

            public IAttackUsableData Clone()
            {
                var data = Create(GetType());
                data.Set(this);
                return data;
            }
            #endregion


        }


        #region ##### Test Attack Action #####

        public class TestAttackActionRawData
        {
            private float _attackRange;
            private int _attackCount;
            private float _attackDelayTime;
            private bool _isOverlap;

            public float AttackRange => _attackRange;
            public int AttackCount => _attackCount;
            public float AttackDelayTime => _attackDelayTime;
            public bool IsOverlap => _isOverlap;

            public TestAttackActionRawData()
            {
                _attackRange = 5f;
                _attackCount = 1;
                _attackDelayTime = 0f;
                _isOverlap = false;
            }

            public TestAttackActionRawData(float attackRange, int attackCount, float attackDelayTime, bool isOverlap)
            {
                _attackRange = attackRange;
                _attackCount = attackCount;
                _attackDelayTime = attackDelayTime;
                _isOverlap = isOverlap;
            }


            internal TestAttackActionUsableData GetUsableData()
            {
                var data = System.Activator.CreateInstance<TestAttackActionUsableData>();
                data.SetData(this);
                return data;
            }
        }

        public class TestAttackActionUsableData
        {

            private TestAttackActionRawData _raw;
            private int _nowAttackCount;
            private float _nowAttackDelayTime;
            private bool _isStart = false;

            public void SetData(TestAttackActionRawData raw)
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
                if (_nowAttackDelayTime >= _raw.AttackDelayTime)
                {
                    OnAttackEvent();
                    _nowAttackDelayTime -= _raw.AttackDelayTime;
                    _nowAttackCount++;
                }

                if (_nowAttackCount >= _raw.AttackCount)
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
        #endregion


        [Test]
        public void AttackTest_Raw_SetData() 
        {
            var raw = new TestAttackRawData();
            Debug.Log(raw.StartAttackValue);
            Debug.Log(raw.IncreaseAttackValue);
            Debug.Log(raw.IncreaseAttackRate);
            Debug.Log(raw.StartAttackDelayValue);
            Debug.Log(raw.DecreaseAttackDelayValue);
            Debug.Log(raw.DecreaseAttackDelayRate);

            Assert.AreEqual(raw.StartAttackValue, "10");
            Assert.AreEqual(raw.IncreaseAttackValue, "1");
            Assert.AreEqual(raw.IncreaseAttackRate, "0.1");
            Assert.AreEqual(raw.StartAttackDelayValue, "1");
            Assert.AreEqual(raw.DecreaseAttackDelayValue, "0");
            Assert.AreEqual(raw.DecreaseAttackDelayRate, "0.1");
        }

        [Test]
        public void AttackTest_ActionRaw_SetData() 
        {
            var raw = new TestAttackActionRawData();
            Debug.Log(raw.AttackRange);
            Debug.Log(raw.AttackCount);
            Debug.Log(raw.AttackDelayTime);
            Debug.Log(raw.IsOverlap);

            Assert.AreEqual(raw.AttackRange, 5f);
            Assert.AreEqual(raw.AttackCount, 1);
            Assert.AreEqual(raw.AttackDelayTime, 0f);
            Assert.AreEqual(raw.IsOverlap, false);

        }

        [Test]
        public void AttackTest_Raw_CreateUsableData() 
        {
            var raw = new TestAttackRawData();
            var usable = raw.GetUsableData();
                        
            Debug.Log(usable.ToString());
            Assert.AreEqual(usable.ToString(), "10");
        }

        [Test]
        public void AttackTest_ActionRaw_CreateUsableData() 
        {
            var raw = new TestAttackActionRawData();
            var usable = raw.GetUsableData();

            usable.SetOnStartActionListener(() =>
            {
                Debug.Log("Start");
            });

            usable.SetOnAttackActionListener((range, overlap) =>
            {
                Debug.Log($"Attack : {range} / {overlap}");
                Assert.AreEqual($"Attack : {range} / {overlap}", "Attack : 5 / False");
            });

            usable.SetOnEndedActionListener(() =>
            {
                Debug.Log("End");
            });

            usable.RunProcess(1f);
        }

        [Test]
        public void AttackTest_Usable_Upgrade()
        {
            var raw = new TestAttackRawData();
            var usable = raw.GetUsableData(1);

            Debug.Log(usable.ToString());
            Assert.AreEqual(usable.ToString(), "12");

            usable = raw.GetUsableData(10);

            Debug.Log(usable.ToString());
            Assert.AreEqual(usable.ToString(), "40");


            usable = raw.GetUsableData(100);

            Debug.Log(usable.ToString());
            Assert.AreEqual(usable.ToString(), "1.210K");

        }

        [Test]
        public void AttackTest_Usable_ToString_Digit()
        {
            var raw = new TestAttackRawData("1", "0" ,"0");
            var usable = raw.GetUsableData();

            Debug.Log(usable.ToString());
            Assert.AreEqual(usable.ToString(), "1");


            raw = new TestAttackRawData("1000", "0", "0");
            usable = raw.GetUsableData();

            Debug.Log(usable.ToString());
            Assert.AreEqual(usable.ToString(), "1.000K");


            raw = new TestAttackRawData("1000000", "0", "0");
            usable = raw.GetUsableData();

            Debug.Log(usable.ToString());
            Assert.AreEqual(usable.ToString(), "1.000M");


            raw = new TestAttackRawData("1000000000", "0", "0");
            usable = raw.GetUsableData();

            Debug.Log(usable.ToString());
            Assert.AreEqual(usable.ToString(), "1.000G");


            raw = new TestAttackRawData("1000000000000", "0", "0");
            usable = raw.GetUsableData();

            Debug.Log(usable.ToString());
            Assert.AreEqual(usable.ToString(), "1.000T");


            raw = new TestAttackRawData("1000000000000000", "0", "0");
            usable = raw.GetUsableData();

            Debug.Log(usable.ToString());
            Assert.AreEqual(usable.ToString(), "1.000P");


            raw = new TestAttackRawData("1000000000000000000", "0", "0");
            usable = raw.GetUsableData();

            Debug.Log(usable.ToString());
            Assert.AreEqual(usable.ToString(), "1.000Z");


            raw = new TestAttackRawData("1000000000000000000000", "0", "0");
            usable = raw.GetUsableData();

            Debug.Log(usable.ToString());
            Assert.AreEqual(usable.ToString(), "1.000Y");


            raw = new TestAttackRawData("1000000000000000000000000", "0", "0");
            usable = raw.GetUsableData();

            Debug.Log(usable.ToString());
            Assert.AreEqual(usable.ToString(), "1.000AA");


            raw = new TestAttackRawData("1000000000000000000000000000", "0", "0");
            usable = raw.GetUsableData();

            Debug.Log(usable.ToString());
            Assert.AreEqual(usable.ToString(), "1.000AB");

        }

        [Test]
        public void AttackTest_Usable_ToString_Digit_Dot()
        {
            var raw = new TestAttackRawData("1", "0", "0");
            var usable = raw.GetUsableData();

            Debug.Log(usable.ToString());
            Assert.AreEqual(usable.ToString(), "1");


            raw = new TestAttackRawData("10", "0", "0");
            usable = raw.GetUsableData();

            Debug.Log(usable.ToString());
            Assert.AreEqual(usable.ToString(), "10");


            raw = new TestAttackRawData("100", "0", "0");
            usable = raw.GetUsableData();

            Debug.Log(usable.ToString());
            Assert.AreEqual(usable.ToString(), "100");


            raw = new TestAttackRawData("1000", "0", "0");
            usable = raw.GetUsableData();

            Debug.Log(usable.ToString());
            Assert.AreEqual(usable.ToString(), "1.000K");


            raw = new TestAttackRawData("10000", "0", "0");
            usable = raw.GetUsableData();

            Debug.Log(usable.ToString());
            Assert.AreEqual(usable.ToString(), "10.00K");


            raw = new TestAttackRawData("100000", "0", "0");
            usable = raw.GetUsableData();

            Debug.Log(usable.ToString());
            Assert.AreEqual(usable.ToString(), "100.0K");


            raw = new TestAttackRawData("1000000", "0", "0");
            usable = raw.GetUsableData();

            Debug.Log(usable.ToString());
            Assert.AreEqual(usable.ToString(), "1.000M");

        }

        [Test]
        public void AttackTest_Usable_CreateUniversalUsableData()
        { 
            var raw = new TestAttackRawData();
            var usable = raw.GetUsableData();
            var universal = usable.CreateUniversalUsableData();

            Debug.Log(universal.ToString());
            Assert.AreEqual(universal.ToString(), "10");

            usable = raw.GetUsableData(1);
            universal = usable.CreateUniversalUsableData();

            Debug.Log(universal.ToString());
            Assert.AreEqual(universal.ToString(), "12");

            usable = raw.GetUsableData(10);
            universal = usable.CreateUniversalUsableData();

            Debug.Log(universal.ToString());
            Assert.AreEqual(universal.ToString(), "40");

            usable = raw.GetUsableData(100);
            universal = usable.CreateUniversalUsableData();

            Debug.Log(universal.ToString());
            Assert.AreEqual(universal.ToString(), "1.210K");

        }


        [UnityTest]
        public IEnumerator AttackTest_ActionUsable_StartedAction()
        {
            var raw = new TestAttackActionRawData();
            var usable = raw.GetUsableData();
            bool isRun = true;

            usable.SetOnStartActionListener(() =>
            {
                Debug.Log("Start");
                isRun = false;
            });


            while (isRun) {
                usable.RunProcess(Time.deltaTime);
                yield return null;
            }

            LogAssert.Expect(LogType.Log, "Start");

            yield return null;

        }

        [UnityTest]
        public IEnumerator AttackTest_ActionUsable_RunAction()
        {
            var raw = new TestAttackActionRawData();
            var usable = raw.GetUsableData();
            bool isRun = true;

            usable.SetOnAttackActionListener((range, overlap) =>
            {
                Debug.Log($"Attack : {range} / {overlap}");
                isRun = false;
            });


            while (isRun)
            {
                usable.RunProcess(Time.deltaTime);
                yield return null;
            }

            LogAssert.Expect(LogType.Log, "Attack : 5 / False");

            yield return null;

        }

        [UnityTest]
        public IEnumerator AttackTest_ActionUsable_RunAction_Delay()
        {
            var count = 1;
            var delay = 0.5f;

            var raw = new TestAttackActionRawData(5f, count, delay, false);
            var usable = raw.GetUsableData();
            bool isRun = true;

            usable.SetOnAttackActionListener((range, overlap) =>
            {
                Debug.Log($"Attack : {range} / {overlap}");
                isRun = false;
            });


            float nowDelay = 0;
            while (isRun)
            {
                nowDelay += Time.deltaTime;
                usable.RunProcess(Time.deltaTime);
                yield return null;
            }

            LogAssert.Expect(LogType.Log, "Attack : 5 / False");

            Debug.Log($"Delay : {nowDelay}");
            Assert.GreaterOrEqual(nowDelay, delay);

            yield return null;
        }

        [UnityTest]
        public IEnumerator AttackTest_ActionUsable_RunAction_x3()
        {
            var count = 3;
            var delay = 0.5f;

            var raw = new TestAttackActionRawData(5f, count, delay, false);
            var usable = raw.GetUsableData();
            bool isRun = true;

            var nowCount = 0;
            usable.SetOnAttackActionListener((range, overlap) =>
            {
                Debug.Log($"Attack : {range} / {overlap}");
                nowCount++;

                if (nowCount == count) isRun = false;
            });


            float nowDelay = 0;
            while (isRun)
            {
                nowDelay += Time.deltaTime;
                usable.RunProcess(Time.deltaTime);
                yield return null;

            }

            Debug.Log($"Delay : {nowDelay}");
            Assert.GreaterOrEqual(nowDelay, delay * count);

            yield return null;
        }

        [UnityTest]
        public IEnumerator AttackTest_ActionUsable_EndedAction()
        {
            var count = 1;
            var delay = 1f;

            var raw = new TestAttackActionRawData(5f, count, delay, false);
            var usable = raw.GetUsableData();
            bool isRun = true;

            var nowCount = 0;
            usable.SetOnEndedActionListener(() =>
            {
                Debug.Log($"End");
                nowCount++;

                if (nowCount == count) isRun = false;
            });


            float nowDelay = 0;
            while (isRun)
            {
                nowDelay += Time.deltaTime;
                usable.RunProcess(Time.deltaTime);
                yield return null;

            }

            LogAssert.Expect(LogType.Log, "End");

            yield return null;
        }



        [UnityTest]
        public IEnumerator AttackTest_Generator_CreateData_Attack()
        {
            bool isRun = true;

            var search = new GSTU_Search("1SzGjvMX1kac6LzvmQHXQRmNj_7MYDjspwF-wpWJuWks", "Test_Attack_Data");

            SpreadsheetManager.Read(search, sheet =>
            {
                var attack = AttackRawData.Create();

                attack.SetData(
                    sheet["SimpleTurret", "StartAttackValue"].value, 
                    sheet["SimpleTurret", "IncreaseAttackValue"].value, 
                    sheet["SimpleTurret", "IncreaseAttackRate"].value,
                    sheet["SimpleTurret", "StartAttackDelayValue"].value,
                    sheet["SimpleTurret", "DecreaseAttackDelayValue"].value,
                    sheet["SimpleTurret", "DecreaseAttackDelayRate"].value
                    );

                var usable = attack.GetUsableData();

                Debug.Log(usable.ToString());
                Assert.AreEqual(usable.ToString(), "10");
                       
                isRun = false;

            });

            while (isRun)
            {
                yield return null;
            }
        }


        [UnityTest]
        public IEnumerator AttackTest_Generator_CreateData_AttackAction()
        {
            bool isRun = true;

            var search = new GSTU_Search("1SzGjvMX1kac6LzvmQHXQRmNj_7MYDjspwF-wpWJuWks", "Test_AttackAction_Data");
            AttackActionUsableData usable = null;

            SpreadsheetManager.Read(search, sheet =>
            {
                var attack = AttackActionRawData.Create();

                attack.SetData(
                    sheet["Simple", "Range"].value,
                    sheet["Simple", "AttackCount"].value,
                    sheet["Simple", "AttackDelayTime"].value,
                    sheet["Simple", "IsOverlap"].value
                    );
                   
                usable = attack.GetUsableData();

                usable.SetOnAttackActionListener((range, overlap) =>
                {
                    Debug.Log($"Attack : {range} / {overlap}");
                    isRun = false;
                });
            });

            while (isRun)
            {
                if(usable != null) usable.RunProcess(Time.deltaTime);
                yield return null;
            }

            LogAssert.Expect(LogType.Log, "Attack : 0 / False");

            yield return null;

        }

            







    }
}
#endif