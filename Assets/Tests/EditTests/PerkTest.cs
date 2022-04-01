#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
namespace TestFrameworks
{
    using SDefence;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;
    using System.Numerics;
    using Utility.Number;
    using Utility.IO;
    using SDefence.Perk.Usable;
    using SDefence.Perk.Entity;
    using SDefence.Perk;

    public class PerkTest
    {

        #region ##### Test Perk #####

        public class TestPerkUsableData : AbstractPerkUsableData, IPerkUsableData 
        {         
            public override string SavableKey() => typeof(TestPerkUsableData).Name;
        }
        #endregion

        [Test]
        public void PerkTest_Usable_Add() 
        {
            var usable = new TestPerkUsableData();
            usable.AddPerk(10);
            Debug.Log(usable.GetValue());
            Assert.AreEqual(usable.GetValue(), 10);

            var addusable = new TestPerkUsableData();
            addusable.SetPerk(10);

            usable.AddPerk(addusable);
            Debug.Log(usable.GetValue());
            Assert.AreEqual(usable.GetValue(), 20);
        }

        [Test]
        public void PerkTest_Usable_Set() 
        {
            var usable = new TestPerkUsableData();
            usable.SetPerk(5);
            Debug.Log(usable.GetValue());
            Assert.AreEqual(usable.GetValue(), 5);

            var addusable = new TestPerkUsableData();
            addusable.SetPerk(10);

            usable.SetPerk(addusable);
            Debug.Log(usable.GetValue());
            Assert.AreEqual(usable.GetValue(), 10);
        }

        [Test]
        public void PerkTest_Usable_GetValue() 
        {
            var usable = new TestPerkUsableData();
            usable.SetPerk(5);
            Debug.Log(usable.GetValue());
            Assert.AreEqual(usable.GetValue(), 5);
        }

        [Test]
        public void PerkTest_Usable_SaveLoad() 
        {
            var usable = new HealthPerkUsableData();
            usable.SetPerk(5);

            Debug.Log(usable.GetValue());
            Assert.AreEqual(usable.GetValue(), 5);

            var savable = usable.GetSavableData();
            var loadUsable = new HealthPerkUsableData();
            loadUsable.SetSavableData(savable);

            Debug.Log(loadUsable.GetValue());
            Assert.AreEqual(loadUsable.GetValue(), 5);
        }


        [Test]
        public void PerkTest_Entity_Add() 
        {
            var entity = PerkUsableEntity.Create();

            var usable = new TestPerkUsableData();
            usable.AddPerk(5);

            entity.AddPerk(usable);


            Debug.Log(entity.GetPerk<TestPerkUsableData>());
            Assert.AreEqual(entity.GetPerk<TestPerkUsableData>(), 5);

        }

        [Test]
        public void PerkTest_Entity_Set() 
        {
            var entity = PerkUsableEntity.Create();

            var usable = new TestPerkUsableData();
            usable.SetPerk(5);

            entity.SetPerk(usable);


            Debug.Log(entity.GetPerk<TestPerkUsableData>());
            Assert.AreEqual(entity.GetPerk<TestPerkUsableData>(), 5);
        }

        [Test]
        public void PerkTest_Entity_GetValue() 
        {
            var entity = PerkUsableEntity.Create();

            var usable = new TestPerkUsableData();
            usable.SetPerk(5);

            entity.SetPerk(usable);


            Debug.Log(entity.GetPerk<TestPerkUsableData>());
            Assert.AreEqual(entity.GetPerk<TestPerkUsableData>(), 5);
        }

        [Test]
        public void PerkTest_Entity_SaveLoad() 
        {
            var entity = PerkUsableEntity.Create();

            var usable = new HealthPerkUsableData();
            usable.SetPerk(5);

            entity.SetPerk(usable);

            var savable = entity.GetSavableData();

            var loadEntity = PerkUsableEntity.Create();
            loadEntity.SetSavableData(savable);

            Debug.Log(entity.GetPerk<HealthPerkUsableData>());
            Assert.AreEqual(entity.GetPerk<HealthPerkUsableData>(), 5);
        }



        [Test]
        public void PerkTest_Utility_GetUsableData() 
        {
            var usable = PerkDataUtility.Create<HealthPerkUsableData>(5);
            Debug.Log(usable.GetValue());
            Assert.AreEqual(usable.GetValue(), 5);
        }
    }
}
#endif