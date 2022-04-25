#if UNITY_EDITOR

namespace Utility.Statistics.Test
{
    using NUnit.Framework;
    using UnityEngine;

    public class StatisticsTest
    {

        [Test]
        public void StatisticsTest_Initialize()
        {
            var pack = StatisticsPackage.Create();

            Debug.Log(pack);
            Assert.IsNotNull(pack);
        }

        [Test]
        public void StatisticsTest_CleanUp()
        {
            var pack = StatisticsPackage.Create();

            Debug.Log(pack);
            Assert.IsNotNull(pack);

            pack.CleanUp();
            Debug.Log(pack.IsEmpty());
            Assert.IsTrue(pack.IsEmpty());
        }

        [Test]
        public void StatisticsTest_SetStatisticsData()
        {
            var pack = StatisticsPackage.Create();

            Debug.Log(pack);
            Assert.IsNotNull(pack);

            pack.SetStatisticsData<TestStatisticsData>(1);

            Debug.Log(pack.GetStatisticsValue<TestStatisticsData>().ToString());
            Assert.AreEqual(pack.GetStatisticsValue<TestStatisticsData>().ToString(), "1");
        }

        [Test]
        public void StatisticsTest_AddStatisticsData()
        {
            var pack = StatisticsPackage.Create();

            Debug.Log(pack);
            Assert.IsNotNull(pack);

            pack.AddStatisticsData<TestStatisticsData>(1);
            pack.AddStatisticsData<TestStatisticsData>(1);

            Debug.Log(pack.GetStatisticsValue<TestStatisticsData>().ToString());
            Assert.AreEqual(pack.GetStatisticsValue<TestStatisticsData>().ToString(), "2");
        }

        [Test]
        public void StatisticsTest_GetStatisticsValue()
        {
            var pack = StatisticsPackage.Create();

            Debug.Log(pack);
            Assert.IsNotNull(pack);

            Debug.Log(pack.GetStatisticsValue<TestStatisticsData>().ToString());
            Assert.AreEqual(pack.GetStatisticsValue<TestStatisticsData>().ToString(), "0");

            pack.SetStatisticsData<TestStatisticsData>(1);

            Debug.Log(pack.GetStatisticsValue<TestStatisticsData>().ToString());
            Assert.AreEqual(pack.GetStatisticsValue<TestStatisticsData>().ToString(), "1");
        }

        [Test]
        public void StatisticsTest_RemoveStatisticsValue()
        {
            var pack = StatisticsPackage.Create();

            Debug.Log(pack);
            Assert.IsNotNull(pack);

            pack.SetStatisticsData<TestStatisticsData>(1);

            Debug.Log(pack.GetStatisticsValue<TestStatisticsData>().ToString());
            Assert.AreEqual(pack.GetStatisticsValue<TestStatisticsData>().ToString(), "1");

            pack.RemoveStatisticsData<TestStatisticsData>();

            Debug.Log(pack.GetStatisticsValue<TestStatisticsData>().ToString());
            Assert.AreEqual(pack.GetStatisticsValue<TestStatisticsData>().ToString(), "0");
        }

        [Test]
        public void StatisticsTest_Save()
        {
            var pack = StatisticsPackage.Create();

            Debug.Log(pack);
            Assert.IsNotNull(pack);

            pack.SetStatisticsData<TestStatisticsData>(1);

            Debug.Log(pack.GetStatisticsValue<TestStatisticsData>().ToString());
            Assert.AreEqual(pack.GetStatisticsValue<TestStatisticsData>().ToString(), "1");

            var savable = pack.GetSavableData();

            Debug.Log(savable.Children.Count);
            Assert.AreEqual(savable.Children.Count, 1);
        }

        [Test]
        public void StatisticsTest_Save_Load()
        {
            var pack = StatisticsPackage.Create();

            Debug.Log(pack);
            Assert.IsNotNull(pack);

            pack.SetStatisticsData<TestStatisticsData>(1);

            Debug.Log(pack.GetStatisticsValue<TestStatisticsData>().ToString());
            Assert.AreEqual(pack.GetStatisticsValue<TestStatisticsData>().ToString(), "1");

            var savable = pack.GetSavableData();

            Debug.Log(savable.Children.Count);
            Assert.AreEqual(savable.Children.Count, 1);


            var loadpack = StatisticsPackage.Create();
            
            Debug.Log(loadpack);
            Assert.IsNotNull(loadpack);

            loadpack.SetSavableData(savable);

            Debug.Log(loadpack.GetStatisticsValue<TestStatisticsData>().ToString());
            Assert.AreEqual(loadpack.GetStatisticsValue<TestStatisticsData>().ToString(), "1");
        }
    }

}
#endif