#if UNITY_EDITOR
namespace Utility.UI.Editor
{
    using UnityEditor;
    using UnityEditor.UI;

    [CustomEditor(typeof(UILanguageButton))]
    [CanEditMultipleObjects]
    public class UILanguageButtonEditor : ButtonEditor
    {
        private SerializedProperty _iconProp;
        private SerializedProperty _textProp;

        protected override void OnEnable()
        {
            base.OnEnable();
            _iconProp = serializedObject.FindProperty("_icon");
            _textProp = serializedObject.FindProperty("_text");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            serializedObject.Update();
            EditorGUILayout.PropertyField(_iconProp);
            EditorGUILayout.PropertyField(_textProp);
            serializedObject.ApplyModifiedProperties();
        }
    }

}
#endif