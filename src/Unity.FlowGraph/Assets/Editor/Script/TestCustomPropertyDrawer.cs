using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(CeilToIntAttribute))]
class TestCustomPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position = EditorGUI.PrefixLabel(position, label);
        property.floatValue = Mathf.CeilToInt(EditorGUI.Slider(position, property.floatValue, 0, 10));

    }

}


