#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
namespace TestFrameworks
{
    using SDefence;
    using System.Collections;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;
    using SDefence.Asset;
    using SDefence.Asset.Usable;
    using SDefence.Asset.Entity;
    using System.Numerics;
    using Utility.Number;
    using Utility.IO;
    using GoogleSheetsToUnity;
    using SDefence.Asset.Raw;

    public class AssetTest
    {

        #region ##### Test Asset #####


        private class TestAssetRawData
        {
            public string typeData;
            public string StartValue;
            public string IncreaseValue;
            public string IncreaseRate;

            internal TestAssetRawData()
            {
                typeData = "TestFrameworks.AssetTest+TestAssetUsableData";
                StartValue = "100";
                IncreaseValue = "1";
                IncreaseRate = "0.1";
            }

            internal TestAssetRawData(string startValue, string increaseValue, string increaseRate)
            {
                typeData = "TestFrameworks.AssetTest+TestAssetUsableData";
                StartValue = startValue;
                IncreaseValue = increaseValue;
                IncreaseRate = increaseRate;
            }

            internal TestAssetRawData(string type, string startValue, string increaseValue, string increaseRate)
            {
                typeData = type;
                StartValue = startValue;
                IncreaseValue = increaseValue;
                IncreaseRate = increaseRate;
            }

            internal IAssetUsableData GetUsableData(int upgrade = 0)
            {
                var type = System.Type.GetType(typeData);
                if (type != null)
                {
                    var data = (IAssetUsableData)System.Activator.CreateInstance(type);
                    data.SetData(StartValue, IncreaseValue, IncreaseRate, upgrade);
                    return data;
                }
                else
                {
                    throw new System.Exception($"{typeData} is not found Type");
                }
            }
        }

        private class TestAssetUsableData : IAssetUsableData
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

            public void Set(IAssetUsableData value)
            {
                Value = ((TestAssetUsableData)value).Value;
            }

            public void Add(int value)
            {
                Value += value;
            }

            public void Add(IAssetUsableData value)
            {
                Value += ((TestAssetUsableData)value).Value;
            }

            public void Subject(int value)
            {
                Value -= value;
            }
            public void Subject(IAssetUsableData value)
            {
                Value -= ((TestAssetUsableData)value).Value;
            }

            public void SetZero() => Value = 0;

            public IAssetUsableData Clone()
            {
                var data = new TestAssetUsableData();
                data.Set(this);
                return data;
            }

            public void Set(int value)
            {
                Value = value;
            }

            public int Compare(IAssetUsableData value)
            {
                if (Value - ((TestAssetUsableData)value).Value > 0) return -1;
                else if (Value - ((TestAssetUsableData)value).Value < 0) return 1;
                return 0;
            }

            public string SavableKey() => typeof(TestAssetUsableData).AssemblyQualifiedName;

            public SavableData GetSavableData()
            {
                var data = SavableData.Create();
                data.AddData("value", Value.Value);
                data.AddData("decimalPoint", Value.DecimalPoint);
                return data;
            }

            public void SetSavableData(SavableData data)
            {
                Value = new BigDecimal((BigInteger)data.Children["value"], (byte)data.Children["decimalPoint"]);
            }
        }

        #endregion

        [Test]
        public void AssetTest_Raw_SetData()
        {
            var raw = new TestAssetRawData();
            Debug.Log(raw.typeData);
            Debug.Log(raw.StartValue);
            Debug.Log(raw.IncreaseValue);
            Debug.Log(raw.IncreaseRate);

            Assert.AreEqual(raw.typeData, "TestFrameworks.AssetTest+TestAssetUsableData");
            Assert.AreEqual(raw.StartValue, "100");
            Assert.AreEqual(raw.IncreaseValue, "1");
            Assert.AreEqual(raw.IncreaseRate, "0.1");
        }

        [Test]
        public void AssetTest_Raw_CreateUsableData()
        {
            var raw = new TestAssetRawData();
            var usable = raw.GetUsableData();

            Debug.Log(usable.GetType().Name);
            Debug.Log(usable.ToString("{0:0}"));

            Assert.AreEqual(usable.GetType().Name, "TestAssetUsableData");
            Assert.AreEqual(usable.ToString("{0:0}"), "100");
        }

        [Test]
        public void AssetTest_Usable_Upgrade()
        {
            var raw = new TestAssetRawData();
            var usable = raw.GetUsableData(1);

            Debug.Log(usable.ToString("{0:0}"));
            Assert.AreEqual(usable.ToString("{0:0}"), "111");

            usable = raw.GetUsableData(10);

            Debug.Log(usable.ToString("{0:0}"));
            Assert.AreEqual(usable.ToString("{0:0}"), "220");


            usable = raw.GetUsableData(100);

            Debug.Log(usable.ToString("{0:0}"));
            Assert.AreEqual(usable.ToString("{0:0}"), "2200");

        }

        [Test]
        public void AssetTest_Usable_ToString_Digit()
        { }

        [Test]
        public void AssetTest_Usable_ToString_Digit_Dot()
        {
           
        }


        [Test]
        public void AssetTest_Usable_Operate()
        {
            var addraw = new TestAssetRawData("10", "0", "0");
            var subjraw = new TestAssetRawData("20", "0", "0");
            var setraw = new TestAssetRawData("100", "0", "0");
            var raw = new TestAssetRawData("0", "0", "0");

            var addusable = addraw.GetUsableData();
            var subjusable = subjraw.GetUsableData();
            var setusable = setraw.GetUsableData();
            var usable = raw.GetUsableData();

            Debug.Log(usable.ToString("{0:0}"));
            Assert.AreEqual(usable.ToString("{0:0}"), "0");

            usable.Add(addusable);
            Debug.Log(usable.ToString("{0:0}"));
            Assert.AreEqual(usable.ToString("{0:0}"), "10");


            usable.Subject(subjusable);

            Debug.Log(usable.ToString("{0:0}"));
            Assert.AreEqual(usable.ToString("{0:0}"), "-10");


            usable.Set(setusable);

            Debug.Log(usable.ToString("{0:0}"));
            Assert.AreEqual(usable.ToString("{0:0}"), "100");

        }


        [Test]
        public void AssetTest_Usable_SaveLoad()
        {
            var raw = new TestAssetRawData();
            var usable = raw.GetUsableData();

            Debug.Log(usable.ToString("{0:0}"));
            Assert.AreEqual(usable.ToString("{0:0}"), "100");

            var savedData = usable.GetSavableData();

            var loadUsable = AbstractAssetUsableData.Create(usable.SavableKey());
            loadUsable.SetSavableData(savedData);

            Debug.Log(loadUsable.ToString("{0:0}"));
            Assert.AreEqual(loadUsable.ToString("{0:0}"), "100");
        }


        [Test]
        public void AssetTest_Entity_Set()
        {
            var neutral = new TestAssetRawData(typeof(NeutralAssetUsableData).AssemblyQualifiedName, "100", "1", "0.1");
            var star = new TestAssetRawData(typeof(StarAssetUsableData).AssemblyQualifiedName, "100", "1", "0.1");

            var neutralUsable = neutral.GetUsableData();
            var starUsable = star.GetUsableData();

            var entity = AssetUsableEntity.Create();
            entity.Set(neutralUsable);
            entity.Set(starUsable);

            Debug.Log(entity.GetValue<NeutralAssetUsableData>("{0:0}"));
            Debug.Log(entity.GetValue<StarAssetUsableData>("{0:0}"));
            Assert.AreEqual(entity.GetValue<NeutralAssetUsableData>("{0:0}"), "100");
            Assert.AreEqual(entity.GetValue<StarAssetUsableData>("{0:0}"), "100");
        }

        [Test]
        public void AssetTest_Entity_Operate()
        {
            var addNeutral = new TestAssetRawData(typeof(NeutralAssetUsableData).AssemblyQualifiedName, "100", "1", "0.1");
            var subjNeutral = new TestAssetRawData(typeof(NeutralAssetUsableData).AssemblyQualifiedName, "100", "1", "0.1");
            var addNeutralUsable = addNeutral.GetUsableData();
            var subjNeutralUsable = subjNeutral.GetUsableData();

            var addStar = new TestAssetRawData(typeof(StarAssetUsableData).AssemblyQualifiedName, "100", "1", "0.1");
            var subjStar = new TestAssetRawData(typeof(StarAssetUsableData).AssemblyQualifiedName, "100", "1", "0.1");
            var addStarUsable = addStar.GetUsableData();
            var subjStarUsable = subjStar.GetUsableData();

            var neutral = new TestAssetRawData(typeof(NeutralAssetUsableData).AssemblyQualifiedName, "100", "1", "0.1");
            var star = new TestAssetRawData(typeof(StarAssetUsableData).AssemblyQualifiedName, "100", "1", "0.1");

            var neutralUsable = neutral.GetUsableData();
            var starUsable = star.GetUsableData();

            var entity = AssetUsableEntity.Create();
            entity.Set(neutralUsable);
            entity.Set(starUsable);

            Debug.Log(entity.GetValue<NeutralAssetUsableData>("{0:0}"));
            Debug.Log(entity.GetValue<StarAssetUsableData>("{0:0}"));
            Assert.AreEqual(entity.GetValue<NeutralAssetUsableData>("{0:0}"), "100");
            Assert.AreEqual(entity.GetValue<StarAssetUsableData>("{0:0}"), "100");


            entity.Add(addNeutralUsable);
            entity.Add(addStarUsable);

            Debug.Log(entity.GetValue<NeutralAssetUsableData>("{0:0}"));
            Debug.Log(entity.GetValue<StarAssetUsableData>("{0:0}"));
            Assert.AreEqual(entity.GetValue<NeutralAssetUsableData>("{0:0}"), "200");
            Assert.AreEqual(entity.GetValue<StarAssetUsableData>("{0:0}"), "200");


            entity.Subject(subjNeutralUsable);
            entity.Subject(subjStarUsable);

            Debug.Log(entity.GetValue<NeutralAssetUsableData>("{0:0}"));
            Debug.Log(entity.GetValue<StarAssetUsableData>("{0:0}"));
            Assert.AreEqual(entity.GetValue<NeutralAssetUsableData>("{0:0}"), "100");
            Assert.AreEqual(entity.GetValue<StarAssetUsableData>("{0:0}"), "100");
        }

        [Test]
        public void AssetTest_Entity_IsEnough()
        {
            var enoughNeutral1 = new TestAssetRawData(typeof(NeutralAssetUsableData).AssemblyQualifiedName, "50", "1", "0.1");
            var enoughNeutral2 = new TestAssetRawData(typeof(NeutralAssetUsableData).AssemblyQualifiedName, "100", "1", "0.1");
            var notEnoughNeutral = new TestAssetRawData(typeof(NeutralAssetUsableData).AssemblyQualifiedName, "150", "1", "0.1");
            var enoughNeutralUsable1 = enoughNeutral1.GetUsableData();
            var enoughNeutralUsable2 = enoughNeutral2.GetUsableData();
            var notEnoughNeutralUsable = notEnoughNeutral.GetUsableData();

            var neutral = new TestAssetRawData(typeof(NeutralAssetUsableData).AssemblyQualifiedName, "100", "1", "0.1");

            var neutralUsable = neutral.GetUsableData();

            var entity = AssetUsableEntity.Create();
            entity.Set(neutralUsable);

            Debug.Log(entity.GetValue<NeutralAssetUsableData>("{0:0}"));
            Assert.AreEqual(entity.GetValue<NeutralAssetUsableData>("{0:0}"), "100");

            Debug.Log(entity.IsEnough(enoughNeutralUsable1));;
            Assert.IsTrue(entity.IsEnough(enoughNeutralUsable1));

            Debug.Log(entity.IsEnough(notEnoughNeutralUsable)); ;
            Assert.IsFalse(entity.IsEnough(notEnoughNeutralUsable));

            Debug.Log(entity.IsEnough(enoughNeutralUsable2)); ;
            Assert.IsTrue(entity.IsEnough(enoughNeutralUsable2));

        }

        [Test]
        public void AssetTest_Entity_SaveLoad()
        {
            var neutral = new TestAssetRawData(typeof(NeutralAssetUsableData).AssemblyQualifiedName, "100", "1", "0.1");
            var star = new TestAssetRawData(typeof(StarAssetUsableData).AssemblyQualifiedName, "100", "1", "0.1");

            var neutralUsable = neutral.GetUsableData();
            var starUsable = star.GetUsableData();

            var entity = AssetUsableEntity.Create();
            entity.Set(neutralUsable);
            entity.Set(starUsable);

            Debug.Log(entity.GetValue<NeutralAssetUsableData>("{0:0}"));
            Debug.Log(entity.GetValue<StarAssetUsableData>("{0:0}"));
            Assert.AreEqual(entity.GetValue<NeutralAssetUsableData>("{0:0}"), "100");
            Assert.AreEqual(entity.GetValue<StarAssetUsableData>("{0:0}"), "100");

            var saveData = entity.GetSavableData();

            var loadEntity = AssetUsableEntity.Create();
            loadEntity.SetSavableData(saveData);

            Debug.Log(loadEntity.GetValue<NeutralAssetUsableData>("{0:0}"));
            Debug.Log(loadEntity.GetValue<StarAssetUsableData>("{0:0}"));
            Assert.AreEqual(loadEntity.GetValue<NeutralAssetUsableData>("{0:0}"), "100");
            Assert.AreEqual(loadEntity.GetValue<StarAssetUsableData>("{0:0}"), "100");


        }

        [UnityTest]
        public IEnumerator AssetTest_Generator_CreateData()
        {
            bool isRun = true;
    
            var search = new GSTU_Search("1SzGjvMX1kac6LzvmQHXQRmNj_7MYDjspwF-wpWJuWks", "Test_Data");

            SpreadsheetManager.Read(search, sheet =>
            {
                var asset = AssetRawData.Create();
                asset.SetData(sheet["HQ1", "TypeAssetData"].value, sheet["HQ1", "StartUpgradeValue"].value, sheet["HQ1", "IncreaseUpgradeValue"].value, sheet["HQ1", "IncreaseUpgradeRate"].value);

                var assetUsable = asset.GetUsableData();

                Debug.Log(assetUsable.ToString("{0:0}"));
                Assert.AreEqual(assetUsable.ToString("{0:0}"), "10");

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