#if UNITY_EDITOR
namespace SDefence.UI.Editor
{
    using UnityEditor;
    using UnityEditor.UI;

    [CustomEditor(typeof(UIAssetButton))]
    [CanEditMultipleObjects]
    public class UIAssetButtonEditor : ButtonEditor
    {
        private SerializedProperty _textProp;

        protected override void OnEnable()
        {
            base.OnEnable();
            _textProp = serializedObject.FindProperty("_text");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            serializedObject.Update();
            EditorGUILayout.PropertyField(_textProp);
            serializedObject.ApplyModifiedProperties();
        }
    }

}
#endif