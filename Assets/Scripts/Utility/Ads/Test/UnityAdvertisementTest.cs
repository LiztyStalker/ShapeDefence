#if UNITY_EDITOR
namespace UnityEngine.Advertisements
{
    using System.Collections;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;

    public class UnityAdvertisementTest
    {
        [Test]
        public void UnityAdvertisementTest_Initialize()
        {
            var ads = UnityAdvertisement.Instance;
            Debug.Log(ads);
            Assert.IsNotNull(ads);
        }

        [Test]
        public void UnityAdvertisementTest_Initialize_Fail()
        {
            var ads = UnityAdvertisement.Instance;
            Debug.Log(ads);
            Assert.IsNotNull(ads);
        }

        [Test]
        public void UnityAdvertisementTest_Show()
        {
            var ads = UnityAdvertisement.Instance;
            Debug.Log(ads);
            Assert.IsNotNull(ads);

            ads.ShowAds();
        }

        [Test]
        public void UnityAdvertisementTest_Ads_Complete()
        {
            var ads = UnityAdvertisement.Instance;
            Debug.Log(ads);
            Assert.IsNotNull(ads);

            ads.SetOnAdsRewardedListener(result =>
            {
                Debug.Log(result);
            });

            ads.ShowAds();
        }
        [Test]
        public void UnityAdvertisementTest_Ads_Cancel()
        {
            var ads = UnityAdvertisement.Instance;
            Debug.Log(ads);
            Assert.IsNotNull(ads);

            ads.SetOnAdsRewardedListener(result =>
            {
                Debug.Log(result);
            });

            ads.ShowAds();
        }
    }
}

#endif