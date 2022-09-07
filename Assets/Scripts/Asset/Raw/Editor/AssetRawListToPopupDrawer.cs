#if UNITY_EDITOR
namespace SDefence.Asset.Raw
{
    using UnityEngine;
    using UnityEditor;

    [CustomPropertyDrawer(typeof(AssetRawListToPopupAttribute))]
    public class AssetRawListToPopupDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var nowType = System.Type.GetType(property.stringValue);
            int selectedIndex = 0;
            if (nowType != null)
            {
                selectedIndex = AssetUtility.Current.FindIndex(nowType);
            }
            var arr = AssetUtility.Current.GetValues();
            var types = AssetUtility.Current.GetTypes();
            selectedIndex = EditorGUI.Popup(position, "TypeAsset", selectedIndex, arr);
            property.stringValue = types[selectedIndex].FullName;

        }
    }
}
#endif