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
    using GoogleSheetsToUnity;
    using System.Collections.Generic;

    public class BattleGenTest
    {

        [Test]
        public void BattleGenTest_Data_WaveElement() 
        {
            var waveElement = BattleGenWaveElement.Create();
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
            var waveData = BattleGenWaveData.Create();
            Debug.Log(waveData.HasWaveData(0, 0));
            Assert.IsTrue(waveData.HasWaveData(0, 0));

            Debug.Log(waveData.HasWaveData(3, 0));
            Assert.IsFalse(waveData.HasWaveData(3, 0));
        }

        [Test]
        public void BattleGenTest_Data_LevelData() 
        {
            var levelData = BattleGenLevelData.Create();

            Debug.Log(levelData.GetBattleGenWaveData(0));
            Debug.Log(levelData.GetBattleGenWaveData(5));
            Assert.IsNotNull(levelData.GetBattleGenWaveData(0));
            Assert.IsNull(levelData.GetBattleGenWaveData(5));
        }


        [Test]
        public void BattleGenTest_Entity_SetData() 
        {
            var levelData = BattleGenLevelData.Create();

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
            var levelData = BattleGenLevelData.Create();
            var entity = BattleGenEntity.Create();

            entity.SetData(levelData);
            entity.SetBattle(levelWaveData);
            entity.SetOnAppearEnemyListener(enemyDataKey =>
            {
                Debug.Log("Gen");
            });

            entity.RunProcessBattle(1f);
            LogAssert.Expect(LogType.Log, "Gen");
        }

        [Test]
        public void BattleGenTest_Entity_SetLevelWave() 
        {
            var levelWaveData = new LevelWaveData();
            var levelData = BattleGenLevelData.Create();

            var entity = BattleGenEntity.Create();
            entity.SetData(levelData);
            entity.SetBattle(levelWaveData);

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
            var levelData = BattleGenLevelData.Create();

            var entity = BattleGenEntity.Create();
            entity.SetData(levelData);
            entity.SetBattle(levelWaveData);
            entity.SetOnAppearEnemyListener(enemyDataKey =>
            {
                Debug.Log(enemyDataKey);
                genCount++;
            });

            while (isRun)
            {
                nowTime += Time.deltaTime;
                entity.RunProcessBattle(Time.deltaTime);

                if(nowTime > 1f)
                {
                    levelWaveData.IncreaseNumber();
                    entity.SetBattle(levelWaveData);
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


        [UnityTest]
        public IEnumerator BattleGenTest_Generator_CreateData()
        {
            bool isRun = true;

            var search = new GSTU_Search("1SzGjvMX1kac6LzvmQHXQRmNj_7MYDjspwF-wpWJuWks", "Test_Wave_Data");

            var dic = new Dictionary<string, BattleGenWaveData>();

            SpreadsheetManager.Read(search, sheet =>
            {
                var waveElement = BattleGenWaveElement.Create();

                waveElement.SetData(sheet["Simple01", "EnemyDataKey"].value, sheet["Simple01", "AppearCount"].value, sheet["Simple01", "Weight"].value, sheet["Simple01", "WaveAppearDelay"].value);

                var waveData = BattleGenWaveData.Create();
                waveData.SetData(waveElement);

                dic.Add("Simple01", waveData);

                isRun = false;
            });

            while (isRun)
            {
                yield return null;
            }


            Debug.Log(dic.Count);
            Assert.AreEqual(dic.Count, 1);



            search = new GSTU_Search("1SzGjvMX1kac6LzvmQHXQRmNj_7MYDjspwF-wpWJuWks", "Test_Level_Data");

            SpreadsheetManager.Read(search, sheet =>
            {
                var levelData = BattleGenLevelData.Create();

                levelData.SetData(sheet["Level0001", "Key"].value, sheet["Level0001", "Level"].value);

                for (int i = 0; i < 5; i++) 
                {
                    var waveKey = sheet["Level0001", $"Wave{i}"].value;

                    if (dic.ContainsKey(waveKey))
                    {
                        levelData.SetWaveData(dic[waveKey], i);
                    }
                }

                Debug.Log(levelData.GetBattleGenWaveData(0));
                Debug.Log(levelData.GetBattleGenWaveData(1));
                Debug.Log(levelData.GetBattleGenWaveData(2));
                Debug.Log(levelData.GetBattleGenWaveData(3));

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