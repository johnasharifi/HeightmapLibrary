﻿#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(HeightmapSpeedLookupTable))]
public class HeightmapSpeedLookupTableDrawer : PropertyDrawer
{
    const float lineHeight = 20f;
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        HeightmapSpeedLookupTable propertyLut = (HeightmapSpeedLookupTable)fieldInfo.GetValue(property.serializedObject.targetObject);
        float height = (propertyLut.Keys.Count + 2) * lineHeight;
        return height;
    }

    int nextInd = 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        const float textWidthPercent = 0.3f;
        const float textSpeedPercent = 0.6f;
        const float textRemovePercent = 0.1f;
        
        Undo.RecordObject(property.serializedObject.targetObject, "name");

        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Speed lookup table"));

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        HeightmapSpeedLookupTable propertyLut = (HeightmapSpeedLookupTable) fieldInfo.GetValue(property.serializedObject.targetObject);

        Rect rectNextKeyAdd = new Rect(position.x + position.width * textWidthPercent, position.y + (propertyLut.Keys.Count) * lineHeight, position.width * (1.0f - textWidthPercent), lineHeight);
        Rect rectNextKeyInput = new Rect(position.x, position.y + (propertyLut.Keys.Count) * lineHeight, position.width * textWidthPercent, lineHeight);

        if (propertyLut.Keys.Count == 0)
        {
            Rect rectObjectInput = new Rect(position.x, position.y + 1 * lineHeight, position.width, lineHeight);

            TextAsset textAsset = (TextAsset) EditorGUI.ObjectField(rectObjectInput, "Speed lookup table json", null, typeof(TextAsset), false);
            if (textAsset != null)
            {
                propertyLut.Overwrite(textAsset.text);
            }
        }

        for (int i = 0; i < propertyLut.Keys.Count; i++)
        {
            Rect rectText = new Rect(position.x, position.y + i * lineHeight, position.width * textWidthPercent, lineHeight);
            Rect rectSpeed = new Rect(position.x + position.width * textWidthPercent, position.y + i * lineHeight, position.width * textSpeedPercent, lineHeight);
            Rect rectRemoveKey = new Rect(position.x + position.width * (textWidthPercent + textSpeedPercent), position.y + i * lineHeight, position.width * textRemovePercent, lineHeight);
            
            EditorGUI.LabelField(rectText, propertyLut.Keys[i].ToString());

            float val = EditorGUI.DelayedFloatField(rectSpeed, propertyLut[propertyLut.Keys[i]]);
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
            propertyLut[nextInd] = default;
        }
        
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
        
        property.serializedObject.ApplyModifiedProperties();
    }

}

#endif
