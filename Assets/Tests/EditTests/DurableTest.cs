#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
namespace TestFrameworks
{
    using System.Collections;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;
    using SDefence.Durable;
    using SDefence.Durable.Usable;
    using System.Numerics;
    using Utility.Number;
    using SDefence.Durable.Entity;

    public class DurableTest
    {
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
                var sVal = long.Parse(startValue);
                var incVal = int.Parse(increaseValue);
                var incRate = float.Parse(increaseRate);

                Value = new BigDecimal(sVal);
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

            public void Add(IDurableUsableData value)
            {
                Value += ((TestDurableUsableData)value).Value;
            }

            public void Subject(int value)
            {
                Value -= value;
            }
            public void Subject(IDurableUsableData value)
            {
                Value -= ((TestDurableUsableData)value).Value;
            }

            public void SetZero() => Value = 0;

            public bool IsOverflowMaxValue(IDurableUsableData maxValue, IDurableUsableData value)
            {
                var maxVal = ((TestDurableUsableData)maxValue).Value;
                var val = ((TestDurableUsableData)value).Value;
                return (Value + val > maxVal);
            }

            public bool IsUnderflowZero(IDurableUsableData value)
            {
                var val = ((AbstractDurableUsableData)value).Value;
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

            public int Compare(IDurableUsableData value)
            {
                if (Value - ((AbstractDurableUsableData)value).Value > 0) return -1;
                else if (Value - ((AbstractDurableUsableData)value).Value < 0) return 1;
                return 0;
            }
        }


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
            var recShield = new TestDurableRawData(typeof(RecoveryShieldDurableUsableData).AssemblyQualifiedName, "10", "1", "0.1");
            var limShield = new TestDurableRawData(typeof(LimitDamageShieldDurableUsableData).AssemblyQualifiedName, "50", "1", "0.1");

            var healthUsable = health.GetUsableData();
            var armorUsable = armor.GetUsableData();
            var shieldUsable = shield.GetUsableData();
            var recShieldUsable = recShield.GetUsableData();
            var limShieldUsable = limShield.GetUsableData();

            Debug.Log(healthUsable.ToString("{0:0}"));
            Debug.Log(armorUsable.ToString("{0:0}"));
            Debug.Log(shieldUsable.ToString("{0:0}"));
            Debug.Log(recShieldUsable.ToString("{0:0}"));
            Debug.Log(limShieldUsable.ToString("{0:0}"));

            Assert.AreEqual(healthUsable.ToString("{0:0}"), "100");
            Assert.AreEqual(armorUsable.ToString("{0:0}"), "1");
            Assert.AreEqual(shieldUsable.ToString("{0:0}"), "100");
            Assert.AreEqual(recShieldUsable.ToString("{0:0}"), "10");
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

            //var health = new TestDurableRawData(typeof(HealthDurableUsableData).AssemblyQualifiedName, "100", "1", "0.1");
            //var armor = new TestDurableRawData(typeof(ArmorDurableUsableData).AssemblyQualifiedName, "1", "1", "0.1");
            //var shield = new TestDurableRawData(typeof(ShieldDurableUsableData).AssemblyQualifiedName, "100", "1", "0.1");
            //var recShield = new TestDurableRawData(typeof(RecoveryShieldDurableUsableData).AssemblyQualifiedName, "10", "1", "0.1");
            //var limShield = new TestDurableRawData(typeof(LimitDamageShieldDurableUsableData).AssemblyQualifiedName, "50", "1", "0.1");

            //var healthUsable = health.GetUsableData();
            //var armorUsable = armor.GetUsableData();
            //var shieldUsable = shield.GetUsableData();
            //var recShieldUsable = recShield.GetUsableData();
            //var limShieldUsable = limShield.GetUsableData();

            //var healthUsable2 = health.GetUsableData();
            //var armorUsable2 = armor.GetUsableData();
            //var shieldUsable2 = shield.GetUsableData();
            //var recShieldUsable2 = recShield.GetUsableData();
            //var limShieldUsable2 = limShield.GetUsableData();

            //healthUsable.Add(10);
            //armorUsable.Add(10);
            //shieldUsable.Add(10);
            //recShieldUsable.Add(10);
            //limShieldUsable.Add(10);

            //Debug.Log(healthUsable.ToString("{0:0}"));
            //Debug.Log(armorUsable.ToString("{0:0}"));
            //Debug.Log(shieldUsable.ToString("{0:0}"));
            //Debug.Log(recShieldUsable.ToString("{0:0}"));
            //Debug.Log(limShieldUsable.ToString("{0:0}"));

            //Assert.AreEqual(healthUsable.ToString("{0:0}"), "100");
            //Assert.AreEqual(armorUsable.ToString("{0:0}"), "1");
            //Assert.AreEqual(shieldUsable.ToString("{0:0}"), "100");
            //Assert.AreEqual(recShieldUsable.ToString("{0:0}"), "10");
            //Assert.AreEqual(limShieldUsable.ToString("{0:0}"), "50");

            //healthUsable.Subject(10);
            //armorUsable.Subject(10);
            //shieldUsable.Subject(10);
            //recShieldUsable.Subject(10);
            //limShieldUsable.Subject(10);

            //Debug.Log(healthUsable.ToString("{0:0}"));
            //Debug.Log(armorUsable.ToString("{0:0}"));
            //Debug.Log(shieldUsable.ToString("{0:0}"));
            //Debug.Log(recShieldUsable.ToString("{0:0}"));
            //Debug.Log(limShieldUsable.ToString("{0:0}"));

            //Assert.AreEqual(healthUsable.ToString("{0:0}"), "100");
            //Assert.AreEqual(armorUsable.ToString("{0:0}"), "1");
            //Assert.AreEqual(shieldUsable.ToString("{0:0}"), "100");
            //Assert.AreEqual(recShieldUsable.ToString("{0:0}"), "10");
            //Assert.AreEqual(limShieldUsable.ToString("{0:0}"), "50");

            //healthUsable.Set(healthUsable2);
            //armorUsable.Set(armorUsable2);
            //shieldUsable.Set(shieldUsable2);
            //recShieldUsable.Set(recShieldUsable2);
            //limShieldUsable.Set(limShieldUsable2);

            //Debug.Log(healthUsable.ToString("{0:0}"));
            //Debug.Log(armorUsable.ToString("{0:0}"));
            //Debug.Log(shieldUsable.ToString("{0:0}"));
            //Debug.Log(recShieldUsable.ToString("{0:0}"));
            //Debug.Log(limShieldUsable.ToString("{0:0}"));

            //Assert.AreEqual(healthUsable.ToString("{0:0}"), "100");
            //Assert.AreEqual(armorUsable.ToString("{0:0}"), "1");
            //Assert.AreEqual(shieldUsable.ToString("{0:0}"), "100");
            //Assert.AreEqual(recShieldUsable.ToString("{0:0}"), "10");
            //Assert.AreEqual(limShieldUsable.ToString("{0:0}"), "50");

        }



        [Test]
        public void DurableTest_UsableEntity_ToString()
        {
            var health = new TestDurableRawData(typeof(HealthDurableUsableData).AssemblyQualifiedName, "100", "1", "0.1");
            var armor = new TestDurableRawData(typeof(ArmorDurableUsableData).AssemblyQualifiedName, "1", "1", "0.1");
            var shield = new TestDurableRawData(typeof(ShieldDurableUsableData).AssemblyQualifiedName, "100", "1", "0.1");
            var recShield = new TestDurableRawData(typeof(RecoveryShieldDurableUsableData).AssemblyQualifiedName, "10", "1", "0.1");
            var limShield = new TestDurableRawData(typeof(LimitDamageShieldDurableUsableData).AssemblyQualifiedName, "50", "1", "0.1");

            var healthUsable = health.GetUsableData();
            var armorUsable = armor.GetUsableData();
            var shieldUsable = shield.GetUsableData();
            var recShieldUsable = recShield.GetUsableData();
            var limShieldUsable = limShield.GetUsableData();


            var usableEntity = DurableUsableEntity.Create();

            usableEntity.Set(healthUsable);
            usableEntity.Set(armorUsable);
            usableEntity.Set(shieldUsable);
            usableEntity.Set(recShieldUsable);
            usableEntity.Set(limShieldUsable);

            Debug.Log(usableEntity.GetValue<HealthDurableUsableData>("{0:0}"));
            Debug.Log(usableEntity.GetValue<ArmorDurableUsableData>("{0:0}"));
            Debug.Log(usableEntity.GetValue<ShieldDurableUsableData>("{0:0}"));
            Debug.Log(usableEntity.GetValue<RecoveryShieldDurableUsableData>("{0:0}"));
            Debug.Log(usableEntity.GetValue<LimitDamageShieldDurableUsableData>("{0:0}"));

            Assert.AreEqual(usableEntity.GetValue<HealthDurableUsableData>("{0:0}"), "100");
            Assert.AreEqual(usableEntity.GetValue<ArmorDurableUsableData>("{0:0}"), "1");
            Assert.AreEqual(usableEntity.GetValue<ShieldDurableUsableData>("{0:0}"), "100");
            Assert.AreEqual(usableEntity.GetValue<RecoveryShieldDurableUsableData>("{0:0}"), "10");
            Assert.AreEqual(usableEntity.GetValue<LimitDamageShieldDurableUsableData>("{0:0}"), "50");
        }

        [Test]
        public void DurableTest_BattleEntity_ToString()
        {
            var health = new TestDurableRawData(typeof(HealthDurableUsableData).AssemblyQualifiedName, "100", "1", "0.1");
            var armor = new TestDurableRawData(typeof(ArmorDurableUsableData).AssemblyQualifiedName, "1", "1", "0.1");
            var shield = new TestDurableRawData(typeof(ShieldDurableUsableData).AssemblyQualifiedName, "100", "1", "0.1");
            var recShield = new TestDurableRawData(typeof(RecoveryShieldDurableUsableData).AssemblyQualifiedName, "10", "1", "0.1");
            var limShield = new TestDurableRawData(typeof(LimitDamageShieldDurableUsableData).AssemblyQualifiedName, "50", "1", "0.1");

            var healthUsable = health.GetUsableData();
            var armorUsable = armor.GetUsableData();
            var shieldUsable = shield.GetUsableData();
            var recShieldUsable = recShield.GetUsableData();
            var limShieldUsable = limShield.GetUsableData();


            var usableEntity = DurableUsableEntity.Create();

            usableEntity.Set(healthUsable);
            usableEntity.Set(armorUsable);
            usableEntity.Set(shieldUsable);
            usableEntity.Set(recShieldUsable);
            usableEntity.Set(limShieldUsable);

            var battleEntity = usableEntity.CreateDurableBattleEntity();

            Debug.Log(battleEntity.GetValue<HealthDurableUsableData>("{0:0}"));
            Debug.Log(battleEntity.GetValue<ArmorDurableUsableData>("{0:0}"));
            Debug.Log(battleEntity.GetValue<ShieldDurableUsableData>("{0:0}"));
            Debug.Log(battleEntity.GetValue<RecoveryShieldDurableUsableData>("{0:0}"));
            Debug.Log(battleEntity.GetValue<LimitDamageShieldDurableUsableData>("{0:0}"));

            Assert.AreEqual(battleEntity.GetValue<HealthDurableUsableData>("{0:0}"), "100 / 100");
            Assert.AreEqual(battleEntity.GetValue<ArmorDurableUsableData>("{0:0}"), "1");
            Assert.AreEqual(battleEntity.GetValue<ShieldDurableUsableData>("{0:0}"), "100 / 100");
            Assert.AreEqual(battleEntity.GetValue<RecoveryShieldDurableUsableData>("{0:0}"), "10");
            Assert.AreEqual(battleEntity.GetValue<LimitDamageShieldDurableUsableData>("{0:0}"), "50");
        }

        [Test]
        public void DurableTest_BattleEntity_HealthOperate()
        {
            var health = new TestDurableRawData(typeof(HealthDurableUsableData).AssemblyQualifiedName, "100", "1", "0.1");
            var healthUsable = health.GetUsableData();

            var usableEntity = DurableUsableEntity.Create();

            usableEntity.Set(healthUsable);

            var battleEntity = usableEntity.CreateDurableBattleEntity();

            Debug.Log(battleEntity.GetValue<HealthDurableUsableData>("{0:0}"));
            Assert.AreEqual(battleEntity.GetValue<HealthDurableUsableData>("{0:0}"), "100 / 100");

            battleEntity.Subject(UniversalDurableUsableData.Create(20));

            Debug.Log(battleEntity.GetValue<HealthDurableUsableData>("{0:0}"));
            Assert.AreEqual(battleEntity.GetValue<HealthDurableUsableData>("{0:0}"), "80 / 100");

            battleEntity.Add(UniversalDurableUsableData.Create(10));

            Debug.Log(battleEntity.GetValue<HealthDurableUsableData>("{0:0}"));
            Assert.AreEqual(battleEntity.GetValue<HealthDurableUsableData>("{0:0}"), "90 / 100");

            battleEntity.Subject(UniversalDurableUsableData.Create(100));

            Debug.Log(battleEntity.GetValue<HealthDurableUsableData>("{0:0}"));
            Assert.AreEqual(battleEntity.GetValue<HealthDurableUsableData>("{0:0}"), "0 / 100");

        }


        [Test]
        public void DurableTest_BattleEntity_ArmorOperate()
        {
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

            battleEntity.Subject(UniversalDurableUsableData.Create(10));

            Debug.Log(battleEntity.GetValue<HealthDurableUsableData>("{0:0}"));
            Assert.AreEqual(battleEntity.GetValue<HealthDurableUsableData>("{0:0}"), "91 / 100");
        }


        [Test]
        public void DurableTest_BattleEntity_ShieldOperate()
        {
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

            battleEntity.Subject(UniversalDurableUsableData.Create(10));

            Debug.Log($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")} - {battleEntity.GetValue<HealthDurableUsableData>("{0:0}")}");
            Assert.AreEqual($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")} - {battleEntity.GetValue<HealthDurableUsableData>("{0:0}")}", "90 / 100 - 100 / 100");

            battleEntity.Subject(UniversalDurableUsableData.Create(100));

            Debug.Log($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")} - {battleEntity.GetValue<HealthDurableUsableData>("{0:0}")}");
            Assert.AreEqual($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")} - {battleEntity.GetValue<HealthDurableUsableData>("{0:0}")}", "0 / 100 - 91 / 100");
        }


        [Test]
        public void DurableTest_BattleEntity_LimitShieldOperate()
        {
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

            battleEntity.Subject(UniversalDurableUsableData.Create(10));

            Debug.Log($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")}");
            Assert.AreEqual($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")}", "90 / 100");

            battleEntity.Subject(UniversalDurableUsableData.Create(100));

            Debug.Log($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")}");
            Assert.AreEqual($"{battleEntity.GetValue<ShieldDurableUsableData>("{0:0}")}", "40 / 100");
        }

    }
}
#endif