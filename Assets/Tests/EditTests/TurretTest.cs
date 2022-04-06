#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
namespace TestFrameworks
{
    using SDefence;
    using System.Collections;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;
    using GoogleSheetsToUnity;
    using SDefence.Turret;
    using SDefence.Turret.Entity;
    using SDefence.Packet;
    using SDefence.Actor;
    using SDefence.Attack.Raw;
    using SDefence.Durable.Usable;

    public class TurretTest
    {
        [Test]
        public void TurretTest_Data_CreateData()
        {
            var data = TurretData.Create();
            Debug.Log(data.Key);
            Assert.AreEqual(data.Key, "Test");
        }

        [Test]
        public void TurretTest_Entity_Create()
        {
            var data = TurretData.Create();
            var entity = TurretEntity.Create();
            entity.Initialize(data, 0);

            Debug.Log(entity.Key);
            Assert.AreEqual(entity.Key, "Test");
        }

        [Test]
        public void TurretTest_Entity_Upgrade()
        {
            var data = TurretData.Create();
            var entity = TurretEntity.Create();
            entity.Initialize(data, 0);

            Debug.Log(entity.UpgradeValue);
            Assert.AreEqual(entity.UpgradeValue, 0);

            entity.Upgrade();

            Debug.Log(entity.UpgradeValue);
            Assert.AreEqual(entity.UpgradeValue, 1);
        }

        [Test]
        public void TurretTest_Entity_Tech()
        {
            var data = TurretData.Create();
            var entity = TurretEntity.Create();
            entity.Initialize(data, 0);

            entity.UpTech(data);

            Debug.Log(entity.Key);
            Assert.AreEqual(entity.Key, "Test");
        }

        [Test]
        public void TurretTest_Entity_SaveLoad()
        {
            var data = TurretData.Create();

            var entity = TurretEntity.Create();
            entity.Initialize(data, 0);
            entity.Upgrade();

            Debug.Log(entity.UpgradeValue);
            Assert.AreEqual(entity.UpgradeValue, 1);

            var savable = entity.GetSavableData();

            var loadentity = TurretEntity.Create();
            loadentity.SetSavableData(savable);

            Debug.Log(loadentity.UpgradeValue);
            Assert.AreEqual(loadentity.UpgradeValue, 1);
        }


        [Test]
        public void TurretTest_Manager_Create()
        {
            var mgr = TurretManager.Create();
            mgr.Initialize();

            Debug.Log(mgr);
            Assert.NotNull(mgr);

        }
        [Test]
        public void TurretTest_Manager_Upgrade()
        {
            var mgr = TurretManager.Create();
            mgr.Initialize();
            mgr.AddOnEntityPacketListener(packet =>
            {
                Debug.Log("Refresh");
            });
            mgr.Upgrade(0);

            LogAssert.Expect(LogType.Log, "Refresh");
        }
        [Test]
        public void TurretTest_Manager_Tech()
        {
            var mgr = TurretManager.Create();
            mgr.Initialize();
            mgr.AddOnEntityPacketListener(packet =>
            {
                Debug.Log("Refresh");
            });
            mgr.UpTech(0, TurretData.Create());

            LogAssert.Expect(LogType.Log, "Refresh");
        }
        [Test]
        public void TurretTest_Manager_SaveLoad()
        {
            var mgr = TurretManager.Create();
            mgr.Initialize();

            var savable = mgr.GetSavableData();

            var loadmgr = TurretManager.Create();
            loadmgr.Initialize();

            loadmgr.SetSavableData(savable);


        }
        [Test]
        public void TurretTest_Manager_SendPacket()
        {
            var mgr = TurretManager.Create();
            mgr.Initialize();
            mgr.AddOnEntityPacketListener(pk =>
            {
                var packet = (TurretEntityPacket)pk;
                Debug.Log(packet.Entity.Key);
                Debug.Log(packet.IsActiveUpgrade);
                Debug.Log(packet.IsActiveUpTech);
                Assert.AreEqual(packet.Entity.Key, "Test");
                Assert.IsFalse(packet.IsActiveUpgrade);
                Assert.IsFalse(packet.IsActiveUpTech);
            });
            mgr.Upgrade(0);
        }
        [Test]
        public void TurretTest_Manager_Expand()
        {
            var mgr = TurretManager.Create();
            mgr.Initialize();
            mgr.AddOnEntityPacketListener(pk =>
            {
                var packet = (TurretEntityPacket)pk;
                Debug.Log(packet.Entity.OrbitIndex);
                Assert.AreEqual(packet.Entity.OrbitIndex, 1);
            });
            mgr.Expand(1);
        }


        [Test]
        public void TurretTest_Actor_Create()
        {
            var data = TurretData.Create();

            var entity = TurretEntity.Create();
            entity.Initialize(data, 0);

            var actor = TurretActor.Create();
            actor.SetEntity(entity);
            actor.SetDurableBattleEntity();
            actor.Activate();

            Debug.Log(actor.GetDurableValue<HealthDurableUsableData>());
            Assert.AreEqual(actor.GetDurableValue<HealthDurableUsableData>(), "100 / 100");
        }

        [Test]
        public void TurretTest_Actor_Attack()
        {
            var data = TurretData.Create();

            var entity = TurretEntity.Create();
            entity.Initialize(data, 0);

            var actor = TurretActor.Create();
            actor.SetEntity(entity);
            actor.SetDurableBattleEntity();
            actor.Activate();



        }
        [Test]
        public void TurretTest_Actor_Damage()
        {
            var data = TurretData.Create();

            var entity = TurretEntity.Create();
            entity.Initialize(data, 0);

            var actor = TurretActor.Create();
            actor.SetEntity(entity);
            actor.SetDurableBattleEntity();
            actor.Activate();

            actor.AddOnBattlePacketListener(pk =>
            {
                var packet = (TurretBattlePacket)pk;
                Debug.Log(packet.Actor.GetDurableValue<HealthDurableUsableData>());
                Assert.AreEqual(packet.Actor.GetDurableValue<HealthDurableUsableData>(), "90 / 100");
            });

            var attack = AttackRawData.Create();
            actor.SetDamage(attack.GetUsableData());

        }
        [Test]
        public void TurretTest_Actor_Broken()
        {
            var data = TurretData.Create();

            var entity = TurretEntity.Create();
            entity.Initialize(data, 0);

            var actor = TurretActor.Create();
            actor.SetEntity(entity);
            actor.SetDurableBattleEntity();
            actor.Activate();

            actor.AddOnBattlePacketListener(pk =>
            {
                var packet = (TurretBattlePacket)pk;
                Debug.Log(packet.Actor.GetDurableValue<HealthDurableUsableData>());
                Debug.Log(packet.Actor.IsDamagable);
                Assert.AreEqual(packet.Actor.GetDurableValue<HealthDurableUsableData>(), "0 / 100");
                Assert.IsTrue(packet.Actor.IsDamagable);
            });

            var attack = AttackRawData.Create();
            attack.SetData("1000", "0", "0", "0", "0", "0");
            actor.SetDamage(attack.GetUsableData());
        }
        [Test]
        public void TurretTest_Actor_NextWave()
        {
            var data = TurretData.Create();

            var entity = TurretEntity.Create();
            entity.Initialize(data, 0);

            var actor = TurretActor.Create();
            actor.SetEntity(entity);
            actor.SetDurableBattleEntity();
            actor.Activate();

            actor.AddOnBattlePacketListener(pk =>
            {
                Debug.Log("Next");
            });

            actor.NextWave();
        }
    }
}
#endif