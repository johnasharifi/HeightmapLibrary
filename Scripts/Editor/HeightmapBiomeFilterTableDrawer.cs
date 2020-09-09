#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(HeightmapBiomeFilter))]
public class HeightmapBiomeFilterDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.indentLevel = 0;
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Biome mapping"));

        Undo.RecordObject(property.serializedObject.targetObject, "name");
        
        EditorGUI.BeginChangeCheck();
        
        Rect origClassRect = new Rect(position.x + position.width * 0.0f, position.y, position.width * 0.1f, position.height);
        Rect targClassRect = new Rect(position.x + position.width * 0.1f, position.y, position.width * 0.1f, position.height);
        Rect failClassRect = new Rect(position.x + position.width * 0.2f, position.y, position.width * 0.1f, position.height);
        Rect predicateClassRect = new Rect(position.x + position.width * 0.3f, position.y, position.width * 0.2f, position.height);
        Rect predicateFloatRect0 = new Rect(position.x + position.width * 0.5f, position.y, position.width * 0.1f, position.height);
        Rect predicateFloatRect1 = new Rect(position.x + position.width * 0.6f, position.y, position.width * 0.2f, position.height);
        Rect predicateFloatRect2 = new Rect(position.x + position.width * 0.8f, position.y, position.width * 0.2f, position.height);

        SerializedProperty originClassProperty = property.FindPropertyRelative("originClass");
        SerializedProperty targetClassProperty = property.FindPropertyRelative("targetClass");
        SerializedProperty failClassProperty = property.FindPropertyRelative("failClass");
        SerializedProperty predciateTypeProperty = property.FindPropertyRelative("predicateType");
        SerializedProperty predicatePerlinScaleProperty = property.FindPropertyRelative("predicatePerlinScale");
        SerializedProperty predicateThresholdAProperty = property.FindPropertyRelative("predicateThresholdA");
        SerializedProperty predicateThresholdBProperty = property.FindPropertyRelative("predicateThresholdB");

        originClassProperty.intValue = EditorGUI.DelayedIntField(origClassRect, originClassProperty.intValue);
        targetClassProperty.intValue = EditorGUI.DelayedIntField(targClassRect, targetClassProperty.intValue);
        failClassProperty.intValue = EditorGUI.DelayedIntField(failClassRect, failClassProperty.intValue);
        predciateTypeProperty.intValue = EditorGUI.Popup(predicateClassRect, predciateTypeProperty.intValue, HeightmapBiomeFilter.predicateTypes);
        predicatePerlinScaleProperty.floatValue = EditorGUI.DelayedFloatField(predicateFloatRect0, predicatePerlinScaleProperty.floatValue);
        if (!HeightmapBiomeFilter.IsBlendedExterior(predciateTypeProperty.intValue))
        {
            predicateThresholdAProperty.floatValue = EditorGUI.DelayedFloatField(predicateFloatRect1, predicateThresholdAProperty.floatValue);
            predicateThresholdBProperty.floatValue = EditorGUI.DelayedFloatField(predicateFloatRect2, predicateThresholdBProperty.floatValue);
        }
        
        EditorGUI.EndProperty();

        property.serializedObject.ApplyModifiedProperties();
    }
}

#endif
