#if UNITY_EDITOR
namespace Storage
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public class DataStorageEditorWindow : EditorWindow
    {
        private Vector2 _scrollPos;

        [MenuItem("Window/Show DataStorage")]
        public static void Init()
        {
            var storageWin = (DataStorageEditorWindow)GetWindow(typeof(DataStorageEditorWindow));
            storageWin.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Data Storage", EditorStyles.boldLabel);

            if (GUILayout.Button("Dispose And Refresh"))
            {
                DataStorage.Dispose();
            }
            GUILayout.Space(20f);

            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            GUI.enabled = false;

            //ShowLayout(DataStorage.Instance.GetAllDataArrayOrZero<UnitData>());
            //ShowLayout(DataStorage.Instance.GetAllDataArrayOrZero<Spine.Unity.SkeletonDataAsset>());
            //ShowLayout(DataStorage.Instance.GetAllDataArrayOrZero<CommanderData>());
            //ShowLayout(DataStorage.Instance.GetAllDataArrayOrZero<BattleFieldData>());
            //ShowLayout(DataStorage.Instance.GetAllDataArrayOrZero<BulletData>());
            //ShowLayout(DataStorage.Instance.GetAllDataArrayOrZero<EffectData>());
            //ShowLayout(DataStorage.Instance.GetAllDataArrayOrZero<SkillData>());
            //ShowLayout(DataStorage.Instance.GetAllDataArrayOrZero<StatusData>());
            //ShowLayout(DataStorage.Instance.GetAllDataArrayOrZero<TribeData>());
            ShowLayout(DataStorage.Instance.GetAllDataArrayOrZero<Sprite>());
            ShowLayout(DataStorage.Instance.GetAllDataArrayOrZero<GameObject>());
            ShowLayout(DataStorage.Instance.GetAllDataArrayOrZero<AudioClip>());
            ShowLayout(DataStorage.Instance.GetAllDataArrayOrZero<TextAsset>());

            GUI.enabled = true;

            GUILayout.EndScrollView();



        }


        private void ShowLayout<T>(T[] arr) where T : Object
        {
            GUILayout.Label(typeof(T).ToString(), EditorStyles.boldLabel);
            if (arr != null)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < arr.Length; i++)
                {
                    EditorGUILayout.ObjectField(arr[i], typeof(T), true);
                }
                EditorGUI.indentLevel--;
            }
            GUILayout.Space(20f);
        }

    }
}
#endif