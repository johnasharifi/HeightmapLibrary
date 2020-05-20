using UnityEngine;
using UnityEditor;

// Heightmap drawn using IMGUI
[CustomPropertyDrawer(typeof(Heightmap))]
public class HeightmapDrawer : PropertyDrawer
{
    const float heightInEditorLines = 5f;
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * heightInEditorLines;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);
        
        Heightmap h = (Heightmap) fieldInfo.GetValue(property.serializedObject.targetObject);
        Texture2D tex = (Texture2D)h;
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        EditorGUI.DrawPreviewTexture(position, tex);

        EditorGUI.EndProperty();
    }
}