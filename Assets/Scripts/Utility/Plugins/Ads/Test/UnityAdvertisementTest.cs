#if UNITY_EDITOR
namespace UnityEngine.Advertisements
{
    using System.Collections;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;

    public class UnityAdvertisementTest
    {

        [TearDown]
        public void TearDown()
        {
            UnityAdvertisement.Dispose();
        }

        [Test]
        public void UnityAdvertisementTest_Initialize()
        {
            try
            {
                var camera = new GameObject();
                camera.AddComponent<Camera>();

                var ads = UnityAdvertisement.Instance;
                Debug.Log(ads);
                Assert.IsNotNull(ads);
            }
            catch
            {
                Assert.Fail();
            }
        }


        [UnityTest]
        public IEnumerator UnityAdvertisementTest_Show()
        {
            var camera = new GameObject();
            camera.AddComponent<Camera>();

            var ads = UnityAdvertisement.Instance;
            Debug.Log(ads);
            Assert.IsNotNull(ads);

            while (!ads.IsInitialized) yield return null;

            ads.ShowAds();

            yield return new WaitForSeconds(1f);
        }

        [UnityTest]
        public IEnumerator UnityAdvertisementTest_Ads_Complete()
        {
            var camera = new GameObject();
            camera.AddComponent<Camera>();

            var ads = UnityAdvertisement.Instance;

            Debug.Log(ads);
            Assert.IsNotNull(ads);


            bool isRun = true;
            ads.SetOnAdsRewardedListener(result =>
            {
                Debug.Log(result);
                Assert.IsTrue(result);
                isRun = false;
            });

            while (!ads.IsInitialized) yield return null;

            ads.ShowAds();

            yield return null;

            ads.OnUnityAdsShowComplete_Test();

            while (isRun) yield return null;

        }

        [UnityTest]
        public IEnumerator UnityAdvertisementTest_Ads_Skipped()
        {
            var camera = new GameObject();
            camera.AddComponent<Camera>();

            var ads = UnityAdvertisement.Instance;
            Debug.Log(ads);
            Assert.IsNotNull(ads);

            bool isRun = true;
            ads.SetOnAdsRewardedListener(result =>
            {
                Debug.Log(result);
                Assert.IsFalse(result);
                isRun = false;
            });

            while (!ads.IsInitialized) yield return null;

            ads.ShowAds();

            yield return null;

            ads.OnUnityAdsShowSkipped_Test();

            while (isRun) yield return null;
        }
    }
}

#endif