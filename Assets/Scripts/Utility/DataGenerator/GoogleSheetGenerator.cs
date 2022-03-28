#if UNITY_EDITOR
namespace Utility.Generator
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using ScriptableObjectData;
    //using GoogleSheetsToUnity;
    using System.Linq;
    //using Utility.Data;
    //using LitJson;

    public class GoogleSheetGenerator
    {

        //#region ##### CreateAndUpdate #####

        public static void CreateAndUpdateTextAsset(string sheetkey, string worksheet, string dataPath, string bundleName = null)
        {
        //    GSTU_Search gstuSearcher = new GSTU_Search(sheetkey, worksheet);
        //    SpreadsheetManager.Read(gstuSearcher, sheet =>
        //    {
        //        OnCreateAndUpdateTextAssetEvent(sheet, worksheet, dataPath, bundleName, gstuSearcher.titleRow);
        //    });
        //}


        //private static void OnCreateAndUpdateTextAssetEvent(GstuSpreadSheet sheet, string worksheet, string dataPath, string bundleName, int startRow)
        //{
        //    Dictionary<string, List<Dictionary<string, Dictionary<string, string>>>> _dic = new Dictionary<string, List<Dictionary<string, Dictionary<string, string>>>>(); 

        //    string[] arr = new string[1];

        //    string tmpkey = "";

        //    //Structure
        //    //{"data":
        //    //    {"key" : Dictionary
        //    //		[{"values" : List, Dictionary
        //    //            {"Language_000 : 0}, Dictionary
        //    //            { Language_001 : 0},						
        //    //	  		  {"Language_000 : 0}, 
        //    //            { Language_001 : 0}
        //    //      }
        //    //		],
        //    //		[
        //    //      ...
        //    //      ]
        //    //    }
        //    //{

        //    ////ù���� ��� �� Ű ����
        //    ///Key_2 Kor Eng
        //    ////�ι�° �ٺ��� ������

        //    var keys = sheet.rows.PrimaryValues.ToArray();

        //    for (int c = 0; c < keys.Length; c++)
        //    {
        //        var row = keys[c];
        //        var key = row[0].value;

        //        if (!string.IsNullOrEmpty(key))
        //        {
        //            //ù�����̸� ��� ����
        //            if (c == 0)
        //            {
        //                //Ű������ �����ϰ� ����
        //                arr = new string[row.Count - 1];
        //                for(int i = 0; i < row.Count - 1; i++)
        //                {
        //                    arr[i] = row[i + 1].value;
        //                }
        //            }
        //            //�ι�°���� �����Ͷ���
        //            else
        //            {
        //                //key�� ������
        //                if(tmpkey == key)
        //                {
        //                    _dic[key].Add(new Dictionary<string, Dictionary<string, string>>());

        //                    _dic[key][_dic[key].Count - 1].Add("values", new Dictionary<string, string>());

        //                    for (int i = 0; i < row.Count - 1; i++)
        //                    {
        //                        _dic[key][_dic[key].Count - 1]["values"].Add(arr[i], row[i + 1].value);
        //                    }
        //                }
        //                //key�� �ٸ���
        //                else
        //                {
        //                    if (!_dic.ContainsKey(key)) 
        //                    {
        //                        _dic.Add(key, new List<Dictionary<string, Dictionary<string, string>>>());
        //                    }

        //                    _dic[key].Add(new Dictionary<string, Dictionary<string, string>>());

        //                    _dic[key][_dic[key].Count - 1].Add("values", new Dictionary<string, string>());

        //                    for (int i = 0; i < row.Count - 1; i++)
        //                    {
        //                        Debug.Log(row[i + 1].value);
        //                        _dic[key][_dic[key].Count - 1]["values"].Add(arr[i], row[i + 1].value);
        //                    }
        //                    tmpkey = key;
        //                }
        //            }
        //        }                
        //    }

        //    Debug.Log(JsonMapper.ToJson(_dic));

        //    TextAsset textAsset = new TextAsset(JsonMapper.ToJson(_dic));

        //    System.IO.File.WriteAllText($"{dataPath}/{worksheet}.txt", textAsset.text);

        //    AssetDatabase.Refresh();

        //    //�ٷ� ������� ����
        //    UnityEditor.AssetImporter importer = UnityEditor.AssetImporter.GetAtPath($"{dataPath}/{worksheet}.txt");
        //    importer.SetAssetBundleNameAndVariant(bundleName, "");
        //    AssetDatabase.SaveAssets();

            Debug.Log("Create And Update TextAsset End");
        }



        ///// <summary>
        ///// ���� �� ����
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="dataPath"></param>
        ///// <param name="bundleName"></param>
        public static void CreateAndUpdateAllData<T>(string sheetkey, string worksheet, string dataPath, string bundleName = null) where T : ScriptableObjectData
        {
        //    GSTU_Search gstuSearcher = new GSTU_Search(sheetkey, worksheet, "A2", 2);
        //    SpreadsheetManager.Read(gstuSearcher, sheet =>
        //    { 
        //        OnCreateAndUpdateEvent<T>(sheet, dataPath, bundleName, gstuSearcher.titleRow); 
        //    });
        //}

        //private static void OnCreateAndUpdateEvent<T>(GstuSpreadSheet sheet, string dataPath, string bundleName, int startRow) where T : ScriptableObjectData
        //{
        //    T tmpData = null;
        //    int index = 0;

        //    var keys = sheet.rows.PrimaryValues.ToArray();

        //    for (int c = 0; c < keys.Length; c++, index++)
        //    {
        //        var row = keys[c];
        //        var key = row[0].value;
        //        //var row = sheet.rows[key];

        //        Debug.Log(row[0].value);

        //        //���� ����Ʈ - Ű�� ����
        //        if (tmpData != null && tmpData.Key == key)
        //        {
        //            tmpData.AddData(row.Select(cell => cell.value).ToArray());
        //            EditorUtility.SetDirty(tmpData);
        //        }
        //        else
        //        {
        //            tmpData = null;
        //            //try
        //            //{
        //            var data = AssetDatabase.LoadAssetAtPath<T>($"{dataPath}/{typeof(T).Name}_{key}.asset");

        //            if (data == null)
        //            {
        //                data = ScriptableObject.CreateInstance<T>();
        //                data.SetSortIndex(index);
        //                data.SetData(row.Select(cell => cell.value).ToArray());
        //                AssetDatabase.CreateAsset(data, $"{dataPath}/{typeof(T).Name}_{key}.asset");
        //                data.SetAssetBundle(bundleName);
        //                EditorUtility.SetDirty(data);
        //                AssetDatabase.SaveAssets();
        //            }
        //            else
        //            {
        //                data.SetSortIndex(index);
        //                data.SetData(row.Select(cell => cell.value).ToArray());
        //                data.SetAssetBundle(bundleName);
        //                EditorUtility.SetDirty(data);
        //            }

        //            tmpData = data;
        //        }
        //        //}
        //        //catch (System.Exception e)
        //        //{
        //        //    Debug.LogWarning(e.Message);
        //        //    AssetDatabase.DeleteAsset($"{dataPath}/{typeof(T).Name}_{key}.asset");
        //        //}
                
                
        //    }
            Debug.Log("Create And Update End");
        }

        //#endregion



        //#region ##### Upload #####


        ///// <summary>
        ///// ���ε�
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="dataPath"></param>
        //public static void UploadAllData<T>(string sheetkey, string worksheet, string dataPath, UnityEngine.Events.UnityAction endCallback = null) where T : ScriptableObjectData
        //{
        //    Debug.LogWarning("������� ����");
        //    //���������� ������Ʈ�ϵ��� ���� �ʿ�
        //    //�ش� �������� �ε����� ����� �ʿ� ����
        //    //Upload<T>(sheetkey, worksheet, dataPath, endCallback);
        //}

        //private static void Upload<T>(string sheetkey, string worksheet, string dataPath, UnityEngine.Events.UnityAction endCallback = null) where T : ScriptableObjectData
        //{
        //    var list = new List<T>();
        //    var files = System.IO.Directory.GetFiles(dataPath);
        //    for (int i = 0; i < files.Length; i++)
        //    {
        //        var data = AssetDatabase.LoadAssetAtPath<T>(files[i]);
        //        if (data != null && !data.IsTest) list.Add(data);
        //    }
        //    list.Sort((d1, d2) => d1.SortIndex - d2.SortIndex);

        //    var saveList = new List<List<string>>();

        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        if (list[i].HasDataArray())
        //        {
        //            var arr = list[i].GetDataArray();

        //            for(int j = 0; j < arr.Length; j++)
        //            {
        //                saveList.Add(new List<string>(arr[j]));
        //            }
        //        }
        //        else
        //        {
        //            saveList.Add(new List<string>(list[i].GetData()));
        //        }
        //    }
        //    GSTU_Search gstuSearcher = new GSTU_Search(sheetkey, worksheet, "A2");
        //    SpreadsheetManager.Write(gstuSearcher, new ValueRange(saveList), endCallback);
        //}

        //#endregion
    }
}
#endif

