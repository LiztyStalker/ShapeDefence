#if UNITY_EDITOR
namespace Utility.Statistics.Editor
{
    using UnityEngine;
    using UnityEditor;

    [CustomPropertyDrawer(typeof(StatisticsListToPopupAttribute))]
    public class StatisticsListToPopupDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var nowType = System.Type.GetType(property.stringValue);
            int selectedIndex = 0;
            if (nowType != null)
            {
                selectedIndex = StatisticsUtility.Current.FindIndex(nowType);
            }
            var arr = StatisticsUtility.Current.GetValues();
            var types = StatisticsUtility.Current.GetTypes();
            selectedIndex = EditorGUI.Popup(position, "Á¶°Ç", selectedIndex, arr);
            property.stringValue = types[selectedIndex].FullName;

        }
    }
}
#endif