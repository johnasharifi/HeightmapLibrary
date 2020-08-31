#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(HeightmapColorLookupTable))]
public class HeightmapColorLookupTableDrawer : PropertyDrawer
{
    const float lineHeight = 20f;
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        HeightmapColorLookupTable propertyLut = (HeightmapColorLookupTable)fieldInfo.GetValue(property.serializedObject.targetObject);
        float height = (propertyLut.Keys.Count + 2) * lineHeight;
        return height;
    }

    int nextInd = 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        const float textWidthPercent = 0.3f;
        const float textColorPercent = 0.6f;
        const float textRemovePercent = 0.1f;

        Undo.RecordObject(property.serializedObject.targetObject, "name");

        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        HeightmapColorLookupTable propertyLut = (HeightmapColorLookupTable) fieldInfo.GetValue(property.serializedObject.targetObject);

        Rect rectNextKeyAdd = new Rect(position.x + position.width * textWidthPercent, position.y + (propertyLut.Keys.Count) * lineHeight, position.width * (1.0f - textWidthPercent), lineHeight);
        Rect rectNextKeyInput = new Rect(position.x, position.y + (propertyLut.Keys.Count) * lineHeight, position.width * textWidthPercent, lineHeight);

        for (int i = 0; i < propertyLut.Keys.Count; i++)
        {
            Rect rectText = new Rect(position.x, position.y + i * lineHeight, position.width * textWidthPercent, lineHeight);
            Rect rectColor = new Rect(position.x + position.width * textWidthPercent, position.y + i * lineHeight, position.width * textColorPercent, lineHeight);
            Rect rectRemoveKey = new Rect(position.x + position.width * (textWidthPercent + textColorPercent), position.y + i * lineHeight, position.width * textRemovePercent, lineHeight);
            
            EditorGUI.LabelField(rectText, propertyLut.Keys[i].ToString());

            Color val = EditorGUI.ColorField(rectColor, propertyLut[propertyLut.Keys[i]]);
            if (val != propertyLut[propertyLut.Keys[i]])
            {
                propertyLut[propertyLut.Keys[i]] = val;
            }
            
            if (!EditorGUI.Toggle(rectRemoveKey, true))
            {
                propertyLut.Remove(propertyLut.Keys[i]);
            }
        }

        EditorGUI.BeginChangeCheck();
        nextInd = EditorGUI.DelayedIntField(rectNextKeyInput, nextInd);
        if (EditorGUI.EndChangeCheck())
        {
            propertyLut[nextInd] = Color.white;
        }
        
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
        
        property.serializedObject.ApplyModifiedProperties();
    }

}

#endif
