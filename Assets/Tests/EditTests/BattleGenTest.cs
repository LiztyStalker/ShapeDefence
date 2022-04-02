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
    using SDefence.BattleGen.Data;
    using SDefence.BattleGen.Entity;
    using SDefence.Data;
    using System.Collections;

    public class BattleGenTest
    {

        [Test]
        public void BattleGenTest_Data_WaveElement() 
        {
            var waveElement = BattleGenWaveElement.Create_Test();
            Debug.Log(waveElement.EnemyDataKey);
            Debug.Log(waveElement.AppearCount);
            Debug.Log(waveElement.Weight);
            Debug.Log(waveElement.WaveAppearDelay);

            Assert.AreEqual(waveElement.EnemyDataKey, "Test");
            Assert.AreEqual(waveElement.AppearCount, 1);
            Assert.AreEqual(waveElement.Weight, 1);
            Assert.AreEqual(waveElement.WaveAppearDelay, 0f);

        }

        [Test]
        public void BattleGenTest_Data_WaveData() 
        {
            var waveData = BattleGenWaveData.Create_Test();
            Debug.Log(waveData.HasWaveData(0, 0));
            Assert.IsTrue(waveData.HasWaveData(0, 0));

            Debug.Log(waveData.HasWaveData(3, 0));
            Assert.IsFalse(waveData.HasWaveData(3, 0));
        }

        [Test]
        public void BattleGenTest_Data_LevelData() 
        {
            var levelData = BattleGenLevelData.Create_Test();

            Debug.Log(levelData.GetBattleGenWaveData(0));
            Debug.Log(levelData.GetBattleGenWaveData(5));
            Assert.IsNotNull(levelData.GetBattleGenWaveData(0));
            Assert.IsNull(levelData.GetBattleGenWaveData(5));
        }


        [Test]
        public void BattleGenTest_Entity_SetData() 
        {
            var levelData = BattleGenLevelData.Create_Test();

            var entity = BattleGenEntity.Create();
            entity.SetData(levelData);

            Debug.Log(entity.HasBattleGenLevelData());
            Assert.IsTrue(entity.HasBattleGenLevelData());

            entity.SetData(null);
            Debug.Log(entity.HasBattleGenLevelData());
            Assert.IsFalse(entity.HasBattleGenLevelData());

        }
        [Test]
        public void BattleGenTest_Entity_RunProcess() 
        {
            var levelWaveData = new LevelWaveData();
            var levelData = BattleGenLevelData.Create_Test();
            var entity = BattleGenEntity.Create();

            entity.SetData(levelData);
            entity.SetLevelWave(levelWaveData);
            entity.SetOnBattleGenListener(gen =>
            {
                Debug.Log("Gen");
            });

            entity.RunProcess(1f);
            LogAssert.Expect(LogType.Log, "Gen");
        }

        [Test]
        public void BattleGenTest_Entity_SetLevelWave() 
        {
            var levelWaveData = new LevelWaveData();
            var levelData = BattleGenLevelData.Create_Test();

            var entity = BattleGenEntity.Create();
            entity.SetData(levelData);
            entity.SetLevelWave(levelWaveData);

            Debug.Log(entity.HasBattleGenWaveData());
            Assert.IsTrue(entity.HasBattleGenWaveData());
        }

        [UnityTest]
        public IEnumerator BattleGenTest_Entity_BattleGenWave() 
        {
            bool isRun = true;
            float nowTime = 0;
            int genCount = 0;

            var levelWaveData = new LevelWaveData();
            var levelData = BattleGenLevelData.Create_Test();

            var entity = BattleGenEntity.Create();
            entity.SetData(levelData);
            entity.SetLevelWave(levelWaveData);
            entity.SetOnBattleGenListener(gen =>
            {
                Debug.Log(gen.EnemyDataKey);
                genCount++;
            });

            while (isRun)
            {
                nowTime += Time.deltaTime;
                entity.RunProcess(Time.deltaTime);

                if(nowTime > 1f)
                {
                    levelWaveData.IncreaseNumber();
                    entity.SetLevelWave(levelWaveData);
                    nowTime = 0f;
                }

                if (levelWaveData.GetLevel() == 1)
                    break;

                yield return null;
            }

            Debug.Log(genCount);
            Assert.AreEqual(genCount, 15);

            yield return null;
        }

    }
}
#endif