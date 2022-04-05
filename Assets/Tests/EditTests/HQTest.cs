#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
namespace TestFrameworks
{
    using SDefence;
    using System.Collections;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;
    using GoogleSheetsToUnity;
    using SDefence.HQ;
    using SDefence.HQ.Entity;
    using SDefence.Packet;
    using SDefence.Actor;
    using SDefence.Attack.Raw;
    using SDefence.Durable.Usable;

    public class HQTest
    {
        [Test]
        public void HQTest_Data_CreateData()
        {
            var data = HQData.Create();
            Debug.Log(data.Key);
            Assert.AreEqual(data.Key, "Test");
        }


        [Test]
        public void HQTest_Entity_Create()
        {
            var data = HQData.Create();

            var entity = HQEntity.Create();
            entity.Initialize(data);

            Debug.Log(entity.Key);
            Assert.AreEqual(entity.Key, "Test");

        }
        [Test]
        public void HQTest_Entity_Upgrade()
        {
            var data = HQData.Create();

            var entity = HQEntity.Create();
            entity.Initialize(data);

            Debug.Log(entity.UpgradeValue);
            Assert.AreEqual(entity.UpgradeValue, 0);

            entity.Upgrade();

            Debug.Log(entity.UpgradeValue);
            Assert.AreEqual(entity.UpgradeValue, 1);
        }
        [Test]
        public void HQTest_Entity_Tech()
        {
            var data = HQData.Create();

            var entity = HQEntity.Create();
            entity.Initialize(data);

            entity.UpTech(data);

            Debug.Log(entity.Key);
            Assert.AreEqual(entity.Key, "Test");

        }
        [Test]
        public void HQTest_Entity_SaveLoad()
        {
            var data = HQData.Create();

            var entity = HQEntity.Create();
            entity.Initialize(data);
            entity.Upgrade();

            Debug.Log(entity.UpgradeValue);
            Assert.AreEqual(entity.UpgradeValue, 1);

            var savable = entity.GetSavableData();

            var loadentity = HQEntity.Create();
            loadentity.SetSavableData(savable);

            Debug.Log(loadentity.UpgradeValue);
            Assert.AreEqual(loadentity.UpgradeValue, 1);
        }


        [Test]
        public void HQTest_Manager_Create()
        {
            var mgr = HQManager.Create();
            mgr.Initialize();

            Debug.Log(mgr);
            Assert.NotNull(mgr);

        }
        [Test]
        public void HQTest_Manager_Upgrade()
        {
            var mgr = HQManager.Create();
            mgr.Initialize();
            mgr.AddOnEntityPacketListener(packet =>
            {
                Debug.Log("Refresh");
            });
            mgr.Upgrade();

            LogAssert.Expect(LogType.Log, "Refresh");

        }

        [Test]
        public void HQTest_Manager_Tech()
        {
            var mgr = HQManager.Create();
            mgr.Initialize();
            mgr.AddOnEntityPacketListener(packet =>
            {
                Debug.Log("Refresh");
            });
            mgr.UpTech(HQData.Create());

            LogAssert.Expect(LogType.Log, "Refresh");
        }
        [Test]
        public void HQTest_Manager_SaveLoad()
        {
            var mgr = HQManager.Create();
            mgr.Initialize();

            var savable = mgr.GetSavableData();

            var loadmgr = HQManager.Create();
            loadmgr.Initialize();

            loadmgr.SetSavableData(savable);
        }

        [Test]
        public void HQTest_Manager_SendPacket()
        {
            var mgr = HQManager.Create();
            mgr.Initialize();
            mgr.AddOnEntityPacketListener(packet =>
            {
                var hqPacket = (HQEntityPacket)packet;
                Debug.Log(hqPacket.Entity.Key);
                Debug.Log(hqPacket.IsActiveUpgrade);
                Debug.Log(hqPacket.IsActiveUpTech);
                Assert.AreEqual(hqPacket.Entity.Key, "Test");
                Assert.IsFalse(hqPacket.IsActiveUpgrade);
                Assert.IsFalse(hqPacket.IsActiveUpTech);
            });
            mgr.Upgrade();
        }



        [Test]
        public void HQTest_Actor_Create() 
        {
            var data = HQData.Create();

            var entity = HQEntity.Create();
            entity.Initialize(data);

            var actor = HQActor.Create();
            actor.SetEntity(entity);
            actor.SetDurableBattleEntity();

        }

        [Test]
        public void HQTest_Actor_Orbit() 
        { 

        }

        [Test]
        public void HQTest_Actor_Damage() 
        {
            var data = HQData.Create();

            var entity = HQEntity.Create();
            entity.Initialize(data);

            var actor = HQActor.Create();
            actor.SetEntity(entity);
            actor.SetDurableBattleEntity();

            actor.AddOnBattlePacketListener(packet =>
            {
                var hqPacket = (HQBattlePacket)packet;
                Debug.Log(hqPacket.Actor.GetDurableValue<HealthDurableUsableData>());
                Assert.AreEqual(hqPacket.Actor.GetDurableValue<HealthDurableUsableData>(), "90 / 100");
            });

            var attack = AttackRawData.Create();            
            actor.SetDamage(attack.GetUsableData());


        }

        [Test]
        public void HQTest_Actor_Destroy() 
        {
            var data = HQData.Create();

            var entity = HQEntity.Create();
            entity.Initialize(data);

            var actor = HQActor.Create();
            actor.SetEntity(entity);
            actor.SetDurableBattleEntity();

            actor.AddOnBattlePacketListener(packet =>
            {
                var hqPacket = (HQBattlePacket)packet;
                Debug.Log(hqPacket.Actor.GetDurableValue<HealthDurableUsableData>());
                Assert.AreEqual(hqPacket.Actor.GetDurableValue<HealthDurableUsableData>(), "0 / 100");
            });

            var attack = AttackRawData.Create();
            attack.SetData("1000", "0", "0", "0", "0", "0");
            actor.SetDamage(attack.GetUsableData());
        }

        [Test]
        public void HQTest_Actor_NextWave() 
        {
            var data = HQData.Create();

            var entity = HQEntity.Create();
            entity.Initialize(data);

            var actor = HQActor.Create();
            actor.SetEntity(entity);
            actor.SetDurableBattleEntity();

            actor.AddOnBattlePacketListener(packet =>
            {
                Debug.Log("Next");
            });

            actor.NextWave();
        }



        







    }
}
#endif