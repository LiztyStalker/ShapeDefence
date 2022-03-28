#if UNITY_EDITOR
namespace UtilityManager
{
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    public class AssetBundleGenerator
    {

        private readonly static string PATH_BUNDLE = "Assets/StreamingAssets";

        [MenuItem("AssetBundle/Build AssetBundles for Android")]
        static void BuildAllAssetBundlesAndroid()
        {
            string assetBundleDirectory = PATH_BUNDLE;
            if (!Directory.Exists(assetBundleDirectory))
            {
                Directory.CreateDirectory(assetBundleDirectory);
            }
            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.Android);
            AssetDatabase.Refresh();
        }

        [MenuItem("AssetBundle/Build AssetBundles for Window")]
        static void BuildAllAssetBundlesWindow()
        {
            string assetBundleDirectory = PATH_BUNDLE;
            if (!Directory.Exists(assetBundleDirectory))
            {
                Directory.CreateDirectory(assetBundleDirectory);
            }
            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
            AssetDatabase.Refresh();
        }
    }
}
#endif