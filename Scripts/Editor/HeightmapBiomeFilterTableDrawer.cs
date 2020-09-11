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

        float[] rectWidthPercents = new float[] { 0.0f, 0.1f, 0.2f, 0.3f, 0.5f, 0.6f, 0.8f, 1.0f };
        string[] propertyNames = new string[] { "m_originClass", "m_targetClass", "m_failClass", "m_predicateType", "m_predicatePerlinScale", "m_predicateThresholdA", "m_predicateThresholdB"};
        Rect[] rects = new Rect[rectWidthPercents.Length - 1];
        SerializedProperty predciateTypeProperty = property.FindPropertyRelative(propertyNames[3]);
        bool isPerlinBlend = !HeightmapBiomeFilter.IsBlendedExterior(predciateTypeProperty.intValue);

        for (int i = 0; i < rects.Length; i++)
        {
            // for perlin blend function, skip drawing m_predicateThresholdA and m_predicateThresholdB. Just draw m_predicatePerlinScale
            if (i > 4 && !isPerlinBlend)
                continue;

            float rectStartX = position.x + position.width * rectWidthPercents[i];
            float rectWidth = position.width * (rectWidthPercents[i + 1] - rectWidthPercents[i]);
            rects[i] = new Rect(rectStartX, position.y, rectWidth, position.height);

            SerializedProperty propertyI = property.FindPropertyRelative(propertyNames[i]);

            switch (i)
            {
                default:
                case 0:
                case 1:
                case 2:
                    propertyI.intValue = EditorGUI.DelayedIntField(rects[i], propertyI.intValue);
                    break;
                case 3:
                    propertyI.intValue = EditorGUI.Popup(rects[i], propertyI.intValue, HeightmapBiomeFilter.predicateTypes);
                    break;
                case 4:
                case 5:
                case 6:
                    propertyI.floatValue = EditorGUI.DelayedFloatField(rects[i], propertyI.floatValue);
                    break;
            }
        }

        /*
        SerializedProperty originClassProperty = property.FindPropertyRelative(propertyNames[0]);
        SerializedProperty targetClassProperty = property.FindPropertyRelative(propertyNames[1]);
        SerializedProperty failClassProperty = property.FindPropertyRelative(propertyNames[2]);
        SerializedProperty predciateTypeProperty = property.FindPropertyRelative(propertyNames[3]);
        SerializedProperty predicatePerlinScaleProperty = property.FindPropertyRelative(propertyNames[4]);
        SerializedProperty predicateThresholdAProperty = property.FindPropertyRelative(propertyNames[5]);
        SerializedProperty predicateThresholdBProperty = property.FindPropertyRelative(propertyNames[6]);

        originClassProperty.intValue = EditorGUI.DelayedIntField(rects[0], originClassProperty.intValue);
        targetClassProperty.intValue = EditorGUI.DelayedIntField(rects[1], targetClassProperty.intValue);
        failClassProperty.intValue = EditorGUI.DelayedIntField(rects[2], failClassProperty.intValue);
        predciateTypeProperty.intValue = EditorGUI.Popup(rects[3], predciateTypeProperty.intValue, HeightmapBiomeFilter.predicateTypes);
        predicatePerlinScaleProperty.floatValue = EditorGUI.DelayedFloatField(rects[4], predicatePerlinScaleProperty.floatValue);
        if (!HeightmapBiomeFilter.IsBlendedExterior(predciateTypeProperty.intValue))
        {
            predicateThresholdAProperty.floatValue = EditorGUI.DelayedFloatField(rects[5], predicateThresholdAProperty.floatValue);
            predicateThresholdBProperty.floatValue = EditorGUI.DelayedFloatField(rects[6], predicateThresholdBProperty.floatValue);
        }
        */
        
        EditorGUI.EndProperty();

        property.serializedObject.ApplyModifiedProperties();
    }
}

#endif
