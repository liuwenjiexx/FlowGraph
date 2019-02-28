using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SerializableTypeCode = FlowGraph.SerializableValue.SerializableTypeCode;

namespace FlowGraph.Editor
{

    [CustomPropertyDrawer(typeof(SerializableValue))]
    public class SerializableValuePropertyDrawer : PropertyDrawer
    {


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            var typeCodeProperty = property.FindPropertyRelative("typeCode");
            var stringValueProperty = property.FindPropertyRelative("stringValue");
            var objectValueProperty = property.FindPropertyRelative("objectValue");
            var curveValueProperty = property.FindPropertyRelative("curveValue");

            var sv = property.GetObjectOfProperty() as SerializableValue;
            ValueField(position, sv.Value, SerializableValue.SerializableTypeCodeToType(sv.TypeCode));
        }


        public static object LayoutValueField(object value, Type type, params GUILayoutOption[] options)
        {
            var rect = GUILayoutUtility.GetRect(0, Screen.width, 0, 16, options);
            return ValueField(rect, value, type);
        }

        public static object ValueField(Rect rect, object value, Type type)
        {
            object newValue = value;
            GUIContent labelContent = GUIContent.none;
            var typeCode = SerializableValue.TypeToSerializableTypeCode(type);
            switch (typeCode)
            {
                case SerializableTypeCode.String:
                    {
                        string str = value as string;
                        str = str ?? "";
                        newValue = EditorGUI.DelayedTextField(rect, str);
                    }
                    break;
                case SerializableTypeCode.Int32:
                    {
                        int n = (int)value;
                        newValue = EditorGUI.DelayedIntField(rect, n);
                    }
                    break;
                case SerializableTypeCode.Single:
                    {
                        float f = (float)value;
                        newValue = EditorGUI.DelayedFloatField(rect, f);
                    }
                    break;
                case SerializableTypeCode.Boolean:
                    {
                        bool b;
                        b = (bool)value;
                        newValue = EditorGUI.Toggle(rect, b);
                    }
                    break;
                case SerializableTypeCode.UnityObject:
                    {
                        UnityEngine.Object obj = value as UnityEngine.Object;
                        newValue = EditorGUI.ObjectField(rect, obj, type, true);
                    }
                    break;
                case SerializableTypeCode.Vector2:
                    {
                        Vector2 v = (Vector2)value;
                        newValue = EditorGUI.Vector2Field(rect, labelContent, v);
                    }
                    break;
                case SerializableTypeCode.Vector3:
                    {
                        Vector3 v = (Vector3)value;
                        newValue = EditorGUI.Vector3Field(rect, labelContent, v);
                    }
                    break;
                case SerializableTypeCode.Vector4:
                    {
                        Vector4 v = (Vector4)value;
                        newValue = EditorGUI.Vector4Field(rect, labelContent, v);
                    }
                    break;
                case SerializableTypeCode.Color:
                    {
                        Color v = (Color)value;
                        newValue = EditorGUI.ColorField(rect, labelContent, v);
                    }
                    break;
                case SerializableTypeCode.Rect:
                    {
                        Rect v = (Rect)value;
                        newValue = EditorGUI.RectField(rect, labelContent, v);
                    }
                    break;
                case SerializableTypeCode.Bounds:
                    {
                        Bounds v = (Bounds)value;
                        newValue = EditorGUI.BoundsField(rect, labelContent, v);
                    }
                    break;
                case SerializableTypeCode.AnimationCurve:
                    {
                        AnimationCurve v = (AnimationCurve)value;
                        newValue = EditorGUI.CurveField(rect, labelContent, v);
                    }
                    break;
            }
            return newValue;
        }
    }

}