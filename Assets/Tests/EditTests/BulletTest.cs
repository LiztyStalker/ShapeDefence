#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
namespace TestFrameworks
{
    using System.Collections;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;
    using Utility.Bullet.Data;
    using Utility.Bullet;
    using SDefence.Attack;
    using SDefence.Attack.Raw;
    using SDefence.Actor;
    using SDefence.Attack.Usable;

    public class BulletTest
    {
        [Test]
        public void BulletTest_Data_CreateData() 
        {
            var data = BulletData.Create();
            Debug.Log(data.Key);
            Assert.AreEqual(data.Key, "Test");
        }

        [Test]
        public void BulletTest_Actor_Create() 
        {
            var data = BulletData.Create();
            var actor = BulletActor.Create();
            actor.SetData(data);

            Debug.Log(actor);
            Assert.NotNull(actor);
        }

        [UnityTest]
        public IEnumerator BulletTest_Actor_Arrive() 
        {
            bool isRun = true;
            var data = BulletData.Create();
            var actor = BulletActor.Create();
            actor.SetData(data);
            actor.SetOnArrivedListener(actor =>
            {
                Debug.Log("Arrive");
                isRun = false;
            });

            actor.SetPosition(Vector2.zero, Vector2.one);

            while (isRun)
            {
                actor.RunProcess(Time.deltaTime);
                yield return null;
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator BulletTest_Actor_Collision() 
        {
            bool isRun = true;
            var data = BulletData.Create();
            var actor = BulletActor.Create();
            actor.SetData(data);
            actor.SetOnAttackListener((actor, attackable, damagable, actionData, callback) =>
            {
                Debug.Log("Collision");
                isRun = false;
            });


            actor.SetPosition(Vector2.zero, Vector2.one);

            while (isRun)
            {
                actor.RunProcess(Time.deltaTime);
                actor.SetCollsion();
                yield return null;
            }

            yield return null;
        }


        public class TestAttackable : IAttackable
        {
            private Vector2 _pos;

            public Vector2 AttackPos => _pos;

            public IAttackUsableData AttackUsableData
            {
                get
                {
                    var data = AttackRawData.Create();
                    return data.GetUsableData();
                }
            }
        }

        public class TestDamagable : IDamagable
        {
            public bool IsDamagable => true;

            public void SetDamage(IAttackUsableData data)
            {
                Debug.Log("Damage " + data.CreateUniversalUsableData().Value);
            }
        }

        [Test]
        public void BulletTest_Manager_Create()
        {
            var manager = BulletManager.Current;
            Debug.Log(manager);
            Assert.NotNull(manager);
        }

        [UnityTest]
        public IEnumerator BulletTest_Manager_Run()
        {
            bool isRun = true;

            AttackActionUsableData _actionData = null;

            var manager = BulletManager.Current;
            var actor = manager.Activate(new TestAttackable(), BulletData.Create(), 1f, Vector2.zero, Vector2.one, (actor, attackable, damagable, actionData, callback) =>
            {
                actionData.SetOnAttackActionListener((range, isOverlap) =>
                {
                    if (damagable != null) damagable.SetDamage(attackable.AttackUsableData);
                    Debug.Log("Attack " + attackable.AttackUsableData.CreateUniversalUsableData().Value);
                    isRun = false;
                });
                _actionData = actionData;
            }, null);

            while (isRun)
            {
                actor.RunProcess(Time.deltaTime);
                actor.SetCollsion();
                if (_actionData != null) _actionData.RunProcess(Time.deltaTime);
                yield return null;
            }
            
        }

        [UnityTest]
        public IEnumerator BulletTest_Manager_Retrieve()
        {
            bool isRun = true;

            var manager = BulletManager.Current;
            var actor = manager.Activate(new TestAttackable(), BulletData.Create(), 1f, Vector2.zero, Vector2.one, null, actor =>
            {
                Debug.Log("Retrieve");
                isRun = false;
            });

            while (isRun)
            {
                actor.RunProcess(Time.deltaTime);
                Debug.Log("actor" + actor.NowPosition);
                yield return null;
            }
        }



    }
}
#endif