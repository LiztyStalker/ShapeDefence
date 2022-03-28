namespace Storage
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Networking;
    using Utility.IO;


    /// <summary>
    /// UnityWebRequest.Result�� 1�� 1����
    /// </summary>

    public class DataLoader : MonoBehaviour
    {

        private readonly string PATH_ASSET_BUNDLE = "Address";


        public static DataLoader Create()
        {
            var obj = new GameObject();
            obj.name = "AssetLoader";
            return obj.AddComponent<DataLoader>();
        }

        public void Dispose()
        {
            DestroyImmediate(gameObject);
        }

        public void Load(System.Action<float> loadCallback, System.Action<TYPE_IO_RESULT> endCallback)
        {
            StartCoroutine(LoadCoroutine(loadCallback, endCallback));
        }

        private IEnumerator LoadCoroutine(System.Action<float> loadCallback, System.Action<TYPE_IO_RESULT> endCallback)
        {

            loadCallback?.Invoke(0.5f);
            yield return new WaitForSeconds(0.5f);

            DataStorage.Instance.ToString();
            endCallback?.Invoke(TYPE_IO_RESULT.Success);

            yield break;

            //���Ŀ� ���� ���� �ڵ� ����

            UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(PATH_ASSET_BUNDLE);
            UnityWebRequestAsyncOperation op = www.SendWebRequest();

            Debug.Assert(www.result == UnityWebRequest.Result.Success, $"bundle Load ���� {www.error}");

            while (true)
            {
                if (op.progress < 1f)
                {
                    loadCallback?.Invoke(op.progress);
                }
                else
                {
                    break;
                }
                yield return null;
            }

            switch (www.result)
            {
                case UnityWebRequest.Result.Success:
                    AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
                    Debug.Log("bundle Load �Ϸ�");
                    yield break;
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                case UnityWebRequest.Result.InProgress:
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError($"bundle Load ���� {www.error}");
                    break;
                default:
                    break;
            }
            yield return null;
            endCallback?.Invoke((TYPE_IO_RESULT)www.result);
        }

#if UNITY_EDITOR || UNITY_INCLUDE_TESTS

        public void LoadTest(System.Action<float> loadCallback, System.Action<TYPE_IO_RESULT> endCallback)
        {
            Debug.LogError("LoadTest ����");
            StartCoroutine(LoadTestCoroutine(loadCallback, endCallback));
        }


        private IEnumerator LoadTestCoroutine(System.Action<float> loadCallback, System.Action<TYPE_IO_RESULT> endCallback)
        {

            //TestCode
            var nowTime = 0f;

            while (true)
            {
                nowTime += Time.deltaTime;
                if (nowTime < 1f)
                {
                    loadCallback?.Invoke(nowTime);
                }
                else
                {
                    break;
                }
                yield return null;
            }
            endCallback?.Invoke(TYPE_IO_RESULT.Success);
        }

#endif


    }
}