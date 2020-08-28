#if UNITY_EDITOR

using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;

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
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
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

        for (int i = 0; i < propertyLut.Keys.Count; i++)
        {
            const float textWidthPercent = 0.3f;
            const float textColorPercent = 0.6f;
            const float textRemovePercent = 0.1f;
            Rect rectText = new Rect(position.x, position.y + i * lineHeight, position.width * textWidthPercent, lineHeight);
            Rect rectColor = new Rect(position.x + position.width * textWidthPercent, position.y + i * lineHeight, position.width * textColorPercent, lineHeight);
            Rect rectRemoveKey = new Rect(position.x + position.width * (textWidthPercent + textColorPercent), position.y + i * lineHeight, position.width * textRemovePercent, lineHeight);

            int key = EditorGUI.DelayedIntField(rectText, propertyLut.Keys[i]);
            Color val = EditorGUI.ColorField(rectColor, propertyLut[propertyLut.Keys[i]]);
            
            if (!EditorGUI.Toggle(rectRemoveKey, true))
            {
                propertyLut.Remove(propertyLut.Keys[i]);
            }
            else
            {
                propertyLut[key] = val;
            }
        }
        
        if (EditorGUI.Foldout(new Rect(position.x, position.y + (propertyLut.Keys.Count) * lineHeight, position.width, lineHeight), false, ""))
        {
            int lastInd = 0;
            if (propertyLut.Keys != null && propertyLut.Keys.Count > 0)
            {
                lastInd = propertyLut.Keys[propertyLut.Keys.Count - 1] + 1;
            }
            
            propertyLut[lastInd] = Color.white;
        }
        
        // fieldInfo.SetValue(property.serializedObject.targetObject, propertyLut);
        // Set indent back to what it was

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
        
        property.serializedObject.ApplyModifiedProperties();
    }

}

#endif

// don't use GUILayout; use UIElements
// https://forum.unity.com/threads/getting-control-0s-position-in-a-group-with-only-0-controls-when-doing-repaint.405774/

// https://docs.unity3d.com/ScriptReference/SerializedProperty.html
// https://docs.unity3d.com/ScriptReference/GUILayout.Button.html

// getting object of a serializedProperty
// https://answers.unity.com/questions/425012/get-the-instance-the-serializedproperty-belongs-to.html

// vars not saved on editor closed
// https://forum.unity.com/threads/variables-are-not-saved-with-the-scene-when-using-custom-editor.374984/

