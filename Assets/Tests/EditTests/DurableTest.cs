#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
namespace TestFrameworks
{
    using SDefence;
    using System.Collections;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;
    using SDefence.Durable;
    using SDefence.Durable.Usable;
    using System.Numerics;
    using Utility.Number;
    using SDefence.Durable.Entity;
    using SDefence.Recovery;
    using SDefence.Attack.Usable;
    using SDefence.Attack;
    using GoogleSheetsToUnity;
    using GoogleSheetsToUnity.Utils;
    using SDefence.Durable.Raw;

    public class DurableTest
    {

        #region ##### Test Attack #####
        private class TestAttackRawData
        {
            public string StartValue;
            public string IncreaseValue;
            public string IncreaseRate;
            public string StartAttackDelayValue;
            public string DecreaseAttackDelayValue;
            public string DecreaseAttackDelayRate;


            internal TestAttackRawData()
            {
                StartValue = "10";
                IncreaseValue = "1";
                IncreaseRate = "0.1";
                StartAttackDelayValue = "1";
                DecreaseAttackDelayValue = "0";
                DecreaseAttackDelayRate = "0.1";
            }

            internal TestAttackRawData(string startValue, string increaseValue, string increaseRate)
            {
                StartValue = startValue;
                IncreaseValue = increaseValue;
                IncreaseRate = increaseRate;
                StartAttackDelayValue = "1";
                DecreaseAttackDelayValue = "0";
                DecreaseAttackDelayRate = "0.1";
            }

            internal TestAttackRawData(string startValue, string increaseValue, string increaseRate, string startDelayValue, string decreaseDelayValue, string decreaseDelayRate)
            {
                StartValue = startValue;
                IncreaseValue = increaseValue;
                IncreaseRate = increaseRate;
                StartAttackDelayValue = startDelayValue;
                DecreaseAttackDelayValue = decreaseDelayValue;
                DecreaseAttackDelayRate = decreaseDelayRate;
            }

            internal IAttackUsableData GetUsableData(int upgrade = 0)
            {
                var data = System.Activator.CreateInstance<TestAttackUsableData>();
                data.SetData(StartValue, IncreaseValue, IncreaseRate, "0", "0", "0", upgrade);
                return data;
            }
        }


        private class TestAttackUsableData : IAttackUsableData
        {
            public BigDecimal Value;

            public float Delay;

            public bool IsZero => Value.IsZero;

            public string ToString(string format) => Value.ToString(format);

            public override string ToString() => Value.ToString();

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

            public void Set(IAttackUsableData value)
            {
                Value = ((TestAttackUsableData)value).Value;
            }

            public IAttackUsableData Clone()
            {
                var data = new TestAttackUsableData();
                data.Set(this);
                return data;
            }
            public UniversalUsableData CreateUniversalUsableData() => new UniversalUsableData(Value);
        }
        #endregion

        #region ##### Test Recovery #####
        private class TestRecoveryRawData
        {
            private string typeData;
            public string StartValue;
            public string IncreaseValue;
            public string IncreaseRate;

            internal TestRecoveryRawData()
            {
                typeData = "TestFrameworks.DurableTest+TestRecoveryUsableData";
                StartValue = "10";
                IncreaseValue = "1";
                IncreaseRate = "0.1";
            }

            internal TestRecoveryRawData(string startValue, string increaseValue, string increaseRate)
            {
                typeData = "TestFrameworks.DurableTest+TestRecoveryUsableData";
                StartValue = startValue;
                IncreaseValue = increaseValue;
                IncreaseRate = increaseRate;
            }

            internal TestRecoveryRawData(string type, string startValue, string increaseValue, string increaseRate)
            {
                typeData = type;
                StartValue = startValue;
                IncreaseValue = increaseValue;
                IncreaseRate = increaseRate;
            }

            internal IRecoveryUsableData GetUsableData(int upgrade = 0)
            {
                var type = System.Type.GetType(typeData);
                if (type != null)
                {
                    var data = (IRecoveryUsableData)System.Activator.CreateInstance(type);
                    data.SetData(StartValue, IncreaseValue, IncreaseRate, upgrade);
                    return data;
                }
                else
                {
                    throw new System.Exception($"{typeData} is not found Type");
                }
            }
        }


        private class TestRecoveryUsableData : IRecoveryUsableData
        {
            public BigDecimal Value;

            private string durableKey;

            public bool IsZero => Value.IsZero;

            public string ToString(string format) => Value.ToString(format);

            public override string ToString() => Value.ToString();

            public void SetData(string startValue, string increaseValue, string increaseRate, int length)
            {
                var incVal = int.Parse(increaseValue);
                var incRate = float.Parse(increaseRate);

                Value = new BigDecimal(startValue);
                Value = NumberDataUtility.GetIsolationInterest(Value, incVal, incRate, length);
            }

            public void Set(IRecoveryUsableData value)
            {
                Value = ((TestDurableUsableData)value).Value;
            }
            public void Set(int value)
            {
                Value = value;
            }
            public IRecoveryUsableData Clone()
            {
                var data = new TestRecoveryUsableData();
                data.Set(this);
                return data;
            }

            public void SetDurableKey(string key) => durableKey = key;

            public string DurableKey() => durableKey;

            public UniversalUsableData CreateUniversalUsableData() => new UniversalUsableData(Value);
        }

        #endregion



        #region ##### Test Durable #####


        private class TestDurableRawData
        {
            private string typeDurable;
            public string StartValue;
            public string IncreaseValue;
            public string IncreaseRate;

            internal TestDurableRawData()
            {
                typeDurable = "TestFrameworks.DurableTest+TestDurableUsableData";
                StartValue = "100";
                IncreaseValue = "1";
                IncreaseRate = "0.1";
            }

            internal TestDurableRawData(string startValue, string increaseValue, string increaseRate)
            {
                typeDurable = "TestFrameworks.DurableTest+TestDurableUsableData";
                StartValue = startValue;
                IncreaseValue = increaseValue;
                IncreaseRate = increaseRate;
            }

            internal TestDurableRawData(string type, string startValue, string increaseValue, string increaseRate)
            {
                typeDurable = type;
                StartValue = startValue;
                IncreaseValue = increaseValue;
                IncreaseRate = increaseRate;
            }

            internal IDurableUsableData GetUsableData(int upgrade = 0)
            {
                var type = System.Type.GetType(typeDurable);
                if (type != null)
                {
                    var data = (IDurableUsableData)System.Activator.CreateInstance(type);
                    data.SetData(StartValue, IncreaseValue, IncreaseRate, upgrade);
                    return data;
                }
                else
                {
                    throw new System.Exception($"{typeDurable} is not found Type");
                }
            }
        }

        private class TestDurableUsableData : IDurableUsableData
        {
            public BigDecimal Value;

            public bool IsZero => Value.IsZero;

            public string ToString(string format) => Value.ToString(format);

            public override string ToString() => Value.ToString();

            public void SetData(string startValue, string increaseValue, string increaseRate, int length)
            {
                var incVal = int.Parse(increaseValue);
                var incRate = float.Parse(increaseRate);

                Value = new BigDecimal(startValue);
                Value = NumberDataUtility.GetIsolationInterest(Value, incVal, incRate, length);
            }

            public void Set(IDurableUsableData value)
            {
                Value = ((TestDurableUsableData)value).Value;
            }

            public void Add(int value)
            {
                Value += value;
            }

            public void Add(UniversalUsableData value)
            {
                Value += value.Value;
            }

            public void Subject(int value)
            {
                Value -= value;
            }
            public void Subject(UniversalUsableData value)
            {
                Value -= value.Value;
            }

            public void SetZero() => Value = 0;

            public bool IsOverflowMaxValue(IDurableUsableData maxValue, UniversalUsableData value)
            {
                var maxVal = ((TestDurableUsableData)maxValue).Value;
                var val = value.Value;
                return (Value + val > maxVal);
            }

            public bool IsUnderflowZero(UniversalUsableData value)
            {
                var val = value.Value;
                return (Value - val < 0);
            }

            public IDurableUsableData Clone()
            {
                var data = new TestDurableUsableData();
                data.Set(this);
                return data;
            }

            public void Set(int value)
            {
                Value = value;
            }

            public int Compare(UniversalUsableData data)
            {
                if (Value - data.Value > 0) return -1;
                else if (Value - data.Value < 0) return 1;
                return 0;
            }

            public UniversalUsableData CreateUniversalUsableData() => new UniversalUsableData(Value);
        }

        #endregion


        [Test]
        public void DurableTest_Raw_CreateUsableData()
        {
            var raw = new TestDurableRawData();
            var usable = raw.GetUsableData();

            Debug.Log(usable.ToString("{0:0}"));
            Assert.AreEqual(usable.ToString("{0:0}"), "100");
        }

        [Test]
        public void DurableTest_Raw_CreateUsableData_Upgrade()
        {
            var raw = new TestDurableRawData();
            var usable = raw.GetUsableData(1);

            Debug.Log(usable.ToString("{0:0}"));
            Assert.AreEqual(usable.ToString("{0:0}"), "111");

            usable = raw.GetUsableData(10);
            Debug.Log(usable.ToString());
            Assert.AreEqual(usable.ToString("{0:0}"), "220");

            usable = raw.GetUsableData(100);
            Debug.Log(usable.ToString());
            Assert.AreEqual(usable.ToString("{0:0}"), "2200");
        }

        [Test]
        public void DurableTest_Usable_ToString()
        {
            var health = new TestDurableRawData(typeof(HealthDurableUsableData).AssemblyQualifiedName, "100", "1", "0.1");
            var armor = new TestDurableRawData(typeof(ArmorDurableUsableData).AssemblyQualifiedName, "1", "1", "0.1");
            var shield = new TestDurableRawData(typeof(ShieldDurableUsableData).AssemblyQualifiedName, "100", "1", "0.1");
            //var recShield = new TestDurableRawData(typeof(RecoveryShieldDurableUsableData).AssemblyQualifiedName, "10", "1", "0.1");
            var limShield = new TestDurableRawData(typeof(LimitDamageShieldDurableUsableData).AssemblyQualifiedName, "50", "1", "0.1");

            var healthUsable = health.GetUsableData();
            var armorUsable = armor.GetUsableData();
            var shieldUsable = shield.GetUsableData();
            //var recShieldUsable = recShield.GetUsableData();
            var limShieldUsable = limShield.GetUsableData();

            Debug.Log(healthUsable.ToString("{0:0}"));
            Debug.Log(armorUsable.ToString("{0:0}"));
            Debug.Log(shieldUsable.ToString("{0:0}"));
            //Debug.Log(recShieldUsable.ToString("{0:0}"));
            Debug.Log(limShieldUsable.ToString("{0:0}"));

            Assert.AreEqual(healthUsable.ToString("{0:0}"), "100");
            Assert.AreEqual(armorUsable.ToString("{0:0}"), "1");
            Assert.AreEqual(shieldUsable.ToString("{0:0}"), "100");
            //Assert.AreEqual(recShieldUsable.ToString("{0:0}"), "10");
            Assert.AreEqual(limShieldUsable.ToString("{0:0}"), "50");

        }

        [Test]
        public void DurableTest_Usable_Operate()
        {

            var raw = new TestDurableRawData();
            var usable = raw.GetUsableData(0);
            var usable2 = raw.GetUsableData(0);

            usable.Add(10);

            Debug.Log(usable.ToString("{0:0}"));
            Assert.AreEqual(usable.ToString("{0:0}"), "110");

            usable.Subject(20);

            Debug.Log(usable.ToString("{0:0}"));
            Assert.AreEqual(usable.ToString("{0:0}"), "90");

            usable.Set(usable2);
            Debug.Log(usable.ToString("{0:0}"));
            Assert.AreEqual(usable.ToString("{0:0}"), "100");
        }

        [Test]
        public void DurableTest_Usable_IsOverflowMaxValue()
        {
            var nowRaw = new TestDurableRawData("50", "1", "0.1");
            var addRaw = new TestDurableRawData("60", "1", "0.1");
            var maxRaw = new TestDurableRawData("100", "1", "0.1");
            var nowUsable = nowRaw.GetUsableData();
            var addUsable = addRaw.GetUsableData();
            var maxUsable = maxRaw.GetUsableData();

            UnityEngine.Debug.Log(nowUsable.IsOverflowMaxValue(maxUsable, addUsable.CreateUniversalUsableData()));
            Assert.IsTrue(nowUsable.IsOverflowMaxValue(maxUsable, addUsable.CreateUniversalUsableData()));

            nowRaw = new TestDurableRawData("50", "1", "0.1");
            addRaw = new TestDurableRawData("40", "1", "0.1");
            maxRaw = new TestDurableRawData("100", "1", "0.1");
            nowUsable = nowRaw.GetUsableData();
            addUsable = addRaw.GetUsableData();
            maxUsable = maxRaw.GetUsableData();

            UnityEngine.Debug.Log(nowUsable.IsOverflowMaxValue(maxUsable, addUsable.CreateUniversalUsableData()));
            Assert.IsFalse(nowUsable.IsOverflowMaxValue(maxUsable, addUsable.CreateUniversalUsableData()));
        }

        [Test]
        public void DurableTest_Usable_IsUnderflowZero()
        {
            var nowRaw = new TestDurableRawData("50", "1", "0.1");
            var subjectRaw = new TestDurableRawData("60", "1", "0.1");
            var nowUsable = nowRaw.GetUsableData();
            var subjectUsable = subjectRaw.GetUsableData();

            UnityEngine.Debug.Log(nowUsable.IsUnderflowZero(subjectUsable.CreateUniversalUsableData()));
            Assert.IsTrue(nowUsable.IsUnderflowZero(subjectUsable.CreateUniversalUsableData()));

            nowRaw = new TestDurableRawData("50", "1", "0.1");
            subjectRaw = new TestDurableRawData("40", "1", "0.1");
            nowUsable = nowRaw.GetUsableData();
            subjectUsable = subjectRaw.GetUsableData();

            UnityEngine.Debug.Log(nowUsable.IsUnderflowZero(subjectUsable.CreateUniversalUsableData()));
            Assert.IsFalse(nowUsable.IsUnderflowZero(subjectUsable.CreateUniversalUsableData()));

        }

        [Test]
        public void DurableTest_Usable_IsZero()
        {
            var nowRaw = new TestDurableRawData("50", "1", "0.1");
            var subjectRaw = new TestDurableRawData("50", "1", "0.1");
            var nowUsable = nowRaw.GetUsableData();
            var subjectUsable = subjectRaw.GetUsableData();

            nowUsable.Subject(subjectUsable.CreateUniversalUsableData());

            UnityEngine.Debug.Log(nowUsable.IsZero);
            Assert.IsTrue(nowUsable.IsZero);

            nowRaw = new TestDurableRawData("50", "1", "0.1");
            subjectRaw = new TestDurableRawData("40", "1", "0.1");
            nowUsable = nowRaw.GetUsableData();
            subjectUsable = subjectRaw.GetUsableData();

            nowUsable.Subject(subjectUsable.CreateUniversalUsableData());

            UnityEngine.Debug.Log(nowUsable.IsZero);
            Assert.IsFalse(nowUsable.IsZero);

        }



        [Test]
        public void DurableTest_UsableEntity_ToString()
        {
            var health = new TestDurableRawData(typeof(HealthDurableUsableData).AssemblyQualifiedName, "100", "1", "0.1");
            var armor = new TestDurableRawData(typeof(ArmorDurableUsableData).AssemblyQualifiedName, "1", "1", "0.1");
            var shield = new TestDurableRawData(typeof(ShieldDurableUsableData).AssemblyQualifiedName, "100", "1", "0.1");
            //var recShield = new TestDurableRawData(typeof(RecoveryShieldDurableUsableData).AssemblyQualifiedName, "10", "1", "0.1");
            var limShield = new TestDurableRawData(typeof(LimitDamageShieldDurableUsableData).AssemblyQualifiedName, "50", "1", "0.1");

            var healthUsable = health.GetUsableData();
            var armorUsable = armor.GetUsableData();
            var shieldUsable = shield.GetUsableData();
            //var recShieldUsable = recShield.GetUsableData();
            var limShieldUsable = limShield.GetUsableData();


            var usableEntity = DurableUsableEntity.Create();

            usableEntity.Set(healthUsable);
            usableEntity.Set(armorUsable);
            usableEntity.Set(shieldUsable);
            //usableEntity.Set(recShieldUsable);
            usableEntity.Set(limShieldUsable);

            Debug.Log(usableEntity.GetValue<HealthDurableUsableData>("{0:0}"));
            Debug.Log(usableEntity.GetValue<ArmorDurableUsableData>("{0:0}"));
            Debug.Log(usableEntity.GetValue<ShieldDurableUsableData>("{0:0}"));
            //Debug.Log(usableEntity.GetValue<RecoveryShieldDurableUsableData>("{0:0}"));
            Debug.Log(usableEntity.GetValue<LimitDamageShieldDurableUsableData>("{0:0}"));

            Assert.AreEqual(usableEntity.GetValue<HealthDurableUsableData>("{0:0}"), "100");
            Assert.AreEqual(usableEntity.GetValue<ArmorDurableUsableData>("{0:0}"), "1");
            Assert.AreEqual(usableEntity.GetValue<ShieldDurableUsableData>("{0:0}"), "100");
            //Assert.AreEqual(usableEntity.GetValue<RecoveryShieldDurableUsableData>("{0:0}"), "10");
            Assert.AreEqual(usableEntity.GetValue<LimitDamageShieldDurableUsableData>("{0:0}"), "50");
        }



        [Test]
        public void DurableTest_BattleEntity_ToString()
        {
            var health = new TestDurableRawData(typeof(HealthDurableUsableData).AssemblyQualifiedName, "100", "1", "0.1");
            var armor = new TestDurableRawData(typeof(ArmorDurableUsableData).AssemblyQualifiedName, "1", "1", "0.1");
            var shield = new TestDurableRawData(typeof(ShieldDurableUsableData).AssemblyQualifiedName, "100", "1", "0.1");
            //var recShield = new TestDurableRawData(typeof(RecoveryShieldDurableUsableData).AssemblyQualifiedName, "10", "1", "0.1");
            var limShield = new TestDurableRawData(typeof(LimitDamageShieldDurableUsableData).AssemblyQualifiedName, "50", "1", "0.1");

            var healthUsable = health.GetUsableData();
            var armorUsable = armor.GetUsableData();
            var shieldUsable = shield.GetUsableData();
            //var recShieldUsable = recShield.GetUsableData();
            var limShieldUsable = limShield.GetUsableData();


            var usableEntity = DurableUsableEntity.Create();

            usableEntity.Set(healthUsable);
            usableEntity.Set(armorUsable);
            usableEntity.Set(shieldUsable);
            //usableEntity.Set(recShieldUsable);
            usableEntity.Set(limShieldUsable);

            var battleEntity = usableEntity.CreateDurableBattleEntity();

            Debug.Log(battleEntity.GetValue<HealthDurableUsableData>("{0:0}"));
            Debug.Log(battleEntity.GetValue<ArmorDurableUsableData>("{0:0}"));
            Debug.Log(battleEntity.GetValue<ShieldDurableUsableData>("{0:0}"));
            //Debug.Log(battleEntity.GetValue<RecoveryShieldDurableUsableData>("{0:0}"));
            Debug.Log(battleEntity.GetValue<LimitDamageShieldDurableUsableData>("{0:0}"));

            Assert.AreEqual(battleEntity.GetValue<HealthDurableUsableData>("{0:0}"), "100 / 100");
            Assert.AreEqual(battleEntity.GetValue<ArmorDurableUsableData>("{0:0}"), "1");
            Assert.AreEqual(battleEntity.GetValue<ShieldDurableUsableData>("{0:0}"), "100 / 100");
            //Assert.AreEqual(battleEntity.GetValue<RecoveryShieldDurableUsableData>("{0:0}"), "10");
            Assert.AreEqual(battleEntity.GetValue<LimitDamageShieldDurableUsableData>("{0:0}"), "50");
        }

        [Test]
        public void DurableTest_BattleEntity_HealthOperate()
        {
            var attack = new TestAttackRawData("20", "0", "0"); 
            var attackUsable = attack.GetUsableData();

            var recovery = new TestRecoveryRawData();
            var recoveryUsable = recovery.GetUsableData();
            ((TestRecoveryUsableData)recoveryUsable).SetDurableKey(typeof(HealthDurableUsableData).Name);



            var health = new TestDurableRawData(typeof(HealthDurableUsableData).AssemblyQualifiedName, "100", "1", "0.1");
            var healthUsable = health.GetUsableData();

            var usableEntity = DurableUsableEntity.Create();

            usableEntity.Set(healthUsable);

            var battleEntity = usableEntity.CreateDurableBattleEntity();

            Debug.Log(battleEntity.GetValue<HealthDurableUsableData>("{0:0}"));
            Assert.AreEqual(battleEntity.GetValue<HealthDurableUsableData>("{0:0}"), "100 / 100");

            battleEntity.Subject(attackUsable);

            Debug.Log(battleEntity.GetValue<HealthDurableUsableData>("{0:0}"));
            Assert.AreEqual(battleEntity.GetValue<HealthDurableUsableData>("{0:0}"), "80 / 100");

            battleEntity.Add(recoveryUsable);

            Debug.Log(battleEntity.GetValue<HealthDurableUsableData>("{0:0}"));
            Assert.AreEqual(battleEntity.GetValue<HealthDurableUsableData>("{0:0}"), "90 / 100");

            attack = new TestAttackRawData("100", "0", "0");
            attackUsable = attack.GetUsableData();

            battleEntity.Subject(attackUsable);

            Debug.Log(battleEntity.GetValue<HealthDurableUsableData>("{0:0}"));
            Assert.AreEqual(battleEntity.GetValue<HealthDurableUsableData>("{0:0}"), "0 / 100");

        }


        [Test]
        public void DurableTest_BattleEntity_ArmorOperate()
        {
            var attack = new TestAttackRawData();
            var attackUsable = attack.GetUsableData();

            var health = new TestDurableRawData(typeof(HealthDurableUsableData).AssemblyQualifiedName, "100", "1", "0.1");
            var armor = new TestDurableRawData(typeof(ArmorDurableUsableData).AssemblyQualifiedName, "1", "1", "0.1");
            var healthUsable = health.GetUsableData();
            var armorUsable = armor.GetUsableData();

            var usableEntity = DurableUsableEntity.Create();

            usableEntity.Set(healthUsable);
            usableEntity.Set(armorUsable);

            var battleEntity = usableEntity.CreateDurableBattleEntity();

            Debug.Log(battleEntity.GetValue<HealthDurableUsableData>("{0:0}"));
            Assert.AreEqual(battleEntity.GetValue<HealthDurableUsableData>("{0:0}"), "100 / 100");

            battleEntity.Subject(attackUsable);

            Debug.Log(battleEntity.GetValue<HealthDurableUsableData>("{0:0}"));
            Assert.AreEqual(battleEntity.GetValue<HealthDurableUsableData>("{0:0}"), "91 / 100");
        }


        [Test]
        public void DurableTest_BattleEntity_ShieldOperate()
        {
            var attack = new TestAttackRawData();
            var attackUsable = attack.GetUsableData();

            var health = new TestDurableRawData(typeof(HealthDurableUsableData).AssemblyQualifiedName, "100", "1", "0.1");
            var armor = new TestDurableRawData(typeof(ArmorDurableUsableData).AssemblyQualifiedName, "1", "1", "0.1");
            var shield = new TestDurableRawData(typeof(ShieldDurableUsableData).AssemblyQualifiedName, "100", "1", "0.1");

            var healthUsable = health.GetUsableData();
            var armorUsable = armor.GetUsableData();
            var shieldUsable = shield.GetUsableData();

            var usableEntity = DurableUsableEntity.Create();

            usableEntity.Set(healthUsable);
            usableEntity.Set(armorUsable);
            usableEntity.Set(shieldUsable);

            var battleEntity = usableEntity.CreateDurableBattleEntity();

            Debug.Log($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")} - {battleEntity.GetValue<HealthDurableUsableData>("{0:0}")}");
            Assert.AreEqual($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")} - {battleEntity.GetValue<HealthDurableUsableData>("{0:0}")}", "100 / 100 - 100 / 100");

            battleEntity.Subject(attackUsable);

            Debug.Log($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")} - {battleEntity.GetValue<HealthDurableUsableData>("{0:0}")}");
            Assert.AreEqual($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")} - {battleEntity.GetValue<HealthDurableUsableData>("{0:0}")}", "90 / 100 - 100 / 100");

            attack = new TestAttackRawData("100", "0", "0");
            attackUsable = attack.GetUsableData();


            battleEntity.Subject(attackUsable);

            Debug.Log($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")} - {battleEntity.GetValue<HealthDurableUsableData>("{0:0}")}");
            Assert.AreEqual($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")} - {battleEntity.GetValue<HealthDurableUsableData>("{0:0}")}", "0 / 100 - 91 / 100");
        }


        [Test]
        public void DurableTest_BattleEntity_LimitShieldOperate()
        {
            var attack = new TestAttackRawData();
            var attackUsable = attack.GetUsableData();

            var health = new TestDurableRawData(typeof(HealthDurableUsableData).AssemblyQualifiedName, "100", "1", "0.1");
            var armor = new TestDurableRawData(typeof(ArmorDurableUsableData).AssemblyQualifiedName, "1", "1", "0.1");
            var shield = new TestDurableRawData(typeof(ShieldDurableUsableData).AssemblyQualifiedName, "100", "1", "0.1");
            var limShield = new TestDurableRawData(typeof(LimitDamageShieldDurableUsableData).AssemblyQualifiedName, "50", "1", "0.1");

            var healthUsable = health.GetUsableData();
            var armorUsable = armor.GetUsableData();
            var shieldUsable = shield.GetUsableData();
            var limShieldUsable = limShield.GetUsableData();

            var usableEntity = DurableUsableEntity.Create();

            usableEntity.Set(healthUsable);
            usableEntity.Set(armorUsable);
            usableEntity.Set(shieldUsable);
            usableEntity.Set(limShieldUsable);

            var battleEntity = usableEntity.CreateDurableBattleEntity();

            Debug.Log($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")}");
            Assert.AreEqual($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")}", "100 / 100");

            battleEntity.Subject(attackUsable);

            Debug.Log($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")}");
            Assert.AreEqual($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")}", "90 / 100");

            attack = new TestAttackRawData("100", "0", "0");
            attackUsable = attack.GetUsableData();

            battleEntity.Subject(attackUsable);

            Debug.Log($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")}");
            Assert.AreEqual($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")}", "40 / 100");
        }

        

        [Test]
        public void DurableTest_BattleEntity_RecShieldOperate()
        {
            var attack = new TestAttackRawData("90", "0", "0");
            var attackUsable = attack.GetUsableData();

            var recovery = new TestRecoveryRawData("50", "0", "0");
            var recoveryUsable = recovery.GetUsableData();
            ((TestRecoveryUsableData)recoveryUsable).SetDurableKey(typeof(ShieldDurableUsableData).Name);

            var health = new TestDurableRawData(typeof(HealthDurableUsableData).AssemblyQualifiedName, "100", "1", "0.1");
            var shield = new TestDurableRawData(typeof(ShieldDurableUsableData).AssemblyQualifiedName, "100", "1", "0.1");

            var healthUsable = health.GetUsableData();
            var shieldUsable = shield.GetUsableData();

            var usableEntity = DurableUsableEntity.Create();

            usableEntity.Set(healthUsable);
            usableEntity.Set(shieldUsable);

            var battleEntity = usableEntity.CreateDurableBattleEntity();

            Debug.Log($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")}");
            Assert.AreEqual($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")}", "100 / 100");

            battleEntity.Subject(attackUsable);

            Debug.Log($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")}");
            Assert.AreEqual($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")}", "10 / 100");

            battleEntity.Add(recoveryUsable);

            Debug.Log($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")}");
            Assert.AreEqual($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")}", "60 / 100");

            battleEntity.Add(recoveryUsable);

            Debug.Log($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")}");
            Assert.AreEqual($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")}", "100 / 100");

        }

        [Test]
        public void DurableTest_BattleEntity_RecHealthOperate()
        {
            var attack = new TestAttackRawData("90", "0", "0");
            var attackUsable = attack.GetUsableData();

            var recovery = new TestRecoveryRawData("50", "0", "0");
            var recoveryUsable = recovery.GetUsableData();
            ((TestRecoveryUsableData)recoveryUsable).SetDurableKey(typeof(HealthDurableUsableData).Name);

            var health = new TestDurableRawData(typeof(HealthDurableUsableData).AssemblyQualifiedName, "100", "1", "0.1");

            var healthUsable = health.GetUsableData();

            var usableEntity = DurableUsableEntity.Create();

            usableEntity.Set(healthUsable);

            var battleEntity = usableEntity.CreateDurableBattleEntity();

            Debug.Log($"{battleEntity.GetValue<HealthDurableUsableData>("{0:0}")}");
            Assert.AreEqual($"{battleEntity.GetValue<HealthDurableUsableData>("{0:0}")}", "100 / 100");

            battleEntity.Subject(attackUsable);

            Debug.Log($"{battleEntity.GetValue<HealthDurableUsableData>("{0:0}")}");
            Assert.AreEqual($"{battleEntity.GetValue<HealthDurableUsableData>("{0:0}")}", "10 / 100");

            battleEntity.Add(recoveryUsable);

            Debug.Log($"{battleEntity.GetValue<HealthDurableUsableData>("{0:0}")}");
            Assert.AreEqual($"{battleEntity.GetValue<HealthDurableUsableData>("{0:0}")}", "60 / 100");

            battleEntity.Add(recoveryUsable);

            Debug.Log($"{battleEntity.GetValue<HealthDurableUsableData>("{0:0}")}");
            Assert.AreEqual($"{battleEntity.GetValue<HealthDurableUsableData>("{0:0}")}", "100 / 100");
        }


        private enum TEST_TYPE_GENERATOR { 
            Key,
            StartHealthValue,
            IncreaseHealthValue,
            IncreaseHealthRate,
            StartArmorValue,
            IncreaseArmorValue,
            IncreaseArmorRate,
            StartShieldValue,
            IncreaseShieldValue,
            IncreaseShieldRate,
            StartRecoveryShieldValue,
            IncreaseRecoveryShieldValue,
            IncreaseRecoveryShieldRate,
            StartFloorShieldValue,
            DecreaseFloorShieldValue,
            DecreaseFloorShieldRate,
            TypeAssetData,
            StartUpgradeValue,
            IncreaseUpgradeValue,
            IncreaseUpgradeRate,
            MaxUpgradeCount,
            OrbitCount,
            TechDataKey
        }

        [UnityTest]
        public IEnumerator DurableTest_Generator_CreateData()
        {
            bool isRun = true;

            var search = new GSTU_Search("1SzGjvMX1kac6LzvmQHXQRmNj_7MYDjspwF-wpWJuWks", "Test_Data");

            SpreadsheetManager.Read(search, sheet =>
            {
                var health = DurableRawData.Create();
                var armor = DurableRawData.Create();
                var shield = DurableRawData.Create();
                var limShield = DurableRawData.Create();

                health.SetData(typeof(HealthDurableUsableData).AssemblyQualifiedName, sheet["HQ1", "StartHealthValue"].value, sheet["HQ1", "IncreaseHealthValue"].value, sheet["HQ1", "IncreaseHealthRate"].value);
                armor.SetData(typeof(ArmorDurableUsableData).AssemblyQualifiedName, sheet["HQ1", "StartArmorValue"].value, sheet["HQ1", "IncreaseArmorValue"].value, sheet["HQ1", "IncreaseArmorRate"].value);
                shield.SetData(typeof(ShieldDurableUsableData).AssemblyQualifiedName, sheet["HQ1", "StartShieldValue"].value, sheet["HQ1", "IncreaseShieldValue"].value, sheet["HQ1", "IncreaseShieldRate"].value);
                limShield.SetData(typeof(LimitDamageShieldDurableUsableData).AssemblyQualifiedName, sheet["HQ1", "StartFloorShieldValue"].value, sheet["HQ1", "DecreaseFloorShieldValue"].value, sheet["HQ1", "DecreaseFloorShieldRate"].value);

                var healthUsable = health.GetUsableData();
                var armorUsable = armor.GetUsableData();
                var shieldUsable = shield.GetUsableData();
                var limShieldUsable = limShield.GetUsableData();

                Debug.Log(healthUsable.ToString("{0:0}"));
                Debug.Log(armorUsable.ToString("{0:0}"));
                Debug.Log(shieldUsable.ToString("{0:0}"));
                Debug.Log(limShieldUsable.ToString("{0:0}"));

                Assert.AreEqual(healthUsable.ToString("{0:0}"), "100");
                Assert.AreEqual(armorUsable.ToString("{0:0}"), "1");
                Assert.AreEqual(shieldUsable.ToString("{0:0}"), "100");
                Assert.AreEqual(limShieldUsable.ToString("{0:0}"), "10");

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