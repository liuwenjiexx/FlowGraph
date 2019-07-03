using FlowGraph.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FlowGraph.Editor
{
    [CustomFlowNodeEditor(typeof(InputableValueNode<>))]
    class InputableValueNodeEditor : FlowNodeEditor
    {
        Type ValueType
        {
            get
            {
                Type valueType = target.GetType().FindGenericType(typeof(InputableValueNode<>)).GetGenericArguments()[0];

                return valueType;
            }
        }
        object Value
        {
            get
            {
                object value = target.GetType().GetProperty("Value").GetValue(target, null);

                return value;
            }
            set
            {
                target.GetType().GetProperty("Value").SetValue(target, value, null);
            }
        }
        bool isEditing;
        private double lastClickCount;

        public override void OnNodeNameGUI(Rect rect, GUIStyle labelStyle)
        {
            Type valueType = ValueType;
            object value = Value;
            TypeCode typeCode = Type.GetTypeCode(valueType);
            string nameTooltip = FlowNode.GetDisplayValueTypeName(valueType) + " Value";
            bool doubleClick = false;
            Event evt = Event.current;
            if (evt.type == EventType.MouseUp)
            {
                if (rect.Contains(evt.mousePosition))
                {
                    if (EditorApplication.timeSinceStartup - lastClickCount < 0.2f)
                    {
                        doubleClick = true;
                    }
                    lastClickCount = EditorApplication.timeSinceStartup;
                }
            }
            else if (evt.type == EventType.KeyDown)
            {
                if (evt.keyCode == KeyCode.Escape)
                {
                    if (isEditing)
                    {
                        isEditing = false;
                        evt.Use();
                    }
                }
            }

            if (!selected)
                isEditing = false;
            string valueStr = string.Empty;
            if (value != null)
                valueStr = value.ToString();
            bool changed = false;
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                switch (typeCode)
                {
                    case TypeCode.String:
                        {
                            if (!isEditing)
                            {
                                labelStyle.LabelFit(rect, new GUIContent(valueStr, nameTooltip));
                                if (doubleClick)
                                    isEditing = true;
                            }
                            else
                            {
                                value = EditorGUI.DelayedTextField(rect, valueStr);
                            }
                        }
                        break;
                    case TypeCode.Boolean:
                        {
                            if (!isEditing)
                            {
                                labelStyle.LabelFit(rect, new GUIContent(valueStr, nameTooltip));
                                if (doubleClick)
                                    isEditing = true;
                            }
                            else
                            {
                                value = EditorGUI.ToggleLeft(rect, new GUIContent(valueStr, nameTooltip), (bool)value);
                            }
                        }
                        break;
                    case TypeCode.Int32:
                        {
                            if (!isEditing)
                            {
                                labelStyle.LabelFit(rect, new GUIContent(valueStr, nameTooltip));
                                if (doubleClick)
                                    isEditing = true;
                            }
                            else
                            {
                                value = EditorGUI.DelayedIntField(rect, (int)value);
                            }
                        }
                        break;
                    case TypeCode.Single:
                        {
                            if (!isEditing)
                            {
                                labelStyle.LabelFit(rect, new GUIContent(valueStr, nameTooltip));
                                if (doubleClick)
                                    isEditing = true;
                            }
                            else
                            {
                                value = EditorGUI.DelayedFloatField(rect, (float)value);
                            }
                        }
                        break;
                    default:
                        {

                            if (valueType == typeof(Object) || valueType.IsSubclassOf(typeof(UnityEngine.Object)))
                            {
                                value = EditorGUI.ObjectField(rect, GUIContent.none, (Object)value, valueType, true);
                            }
                            else if (valueType == typeof(Color))
                            {
                                value = EditorGUI.ColorField(rect, GUIContent.none, (Color)value, false, true, false);
                            }
                            else if (valueType == typeof(AnimationCurve))
                            {
                                value = EditorGUI.CurveField(rect, (AnimationCurve)value);
                            }
                            else if (valueType == typeof(Vector2))
                            {
                                value = EditorGUI.Vector2Field(rect, GUIContent.none, (Vector2)value);
                            }
                            else if (valueType == typeof(Vector2Int))
                            {
                                value = EditorGUI.Vector2IntField(rect, GUIContent.none, (Vector2Int)value);
                            }
                            else if (valueType == typeof(Vector3))
                            {
                                value = EditorGUI.Vector3Field(rect, GUIContent.none, (Vector3)value);
                            }
                            else if (valueType == typeof(Vector3Int))
                            {
                                value = EditorGUI.Vector3IntField(rect, GUIContent.none, (Vector3Int)value);
                            }
                            else if (valueType == typeof(Vector4))
                            {
                                value = EditorGUI.Vector4Field(rect, GUIContent.none, (Vector4)value);
                            }
                            else
                            {
                                labelStyle.LabelFit(rect, new GUIContent(valueStr, nameTooltip));
                            }
                        }
                        break;
                }
                if (check.changed)
                {
                    Value = value;
                    GUI.changed = false;
                    isEditing = false;
                    changed = true;
                }
            }
            if (changed)
            {
                GUI.changed = true;
            }
            EditorGUI.LabelField(rect, new GUIContent(string.Empty, nameTooltip));
        }

        public override void OnDetailsGUI()
        {
            Type valueType = ValueType;

            float maxWidth = GUILayoutUtility.GetRect(0, Screen.width, 0, 0).width;

            FlowGraphEditorWindow.detailsNameStyle.LabelFit(new GUIContent("Value"), (int)maxWidth);

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Type", GUILayout.Width(EditorGUIUtility.labelWidth));
                GUILayout.Label(FlowNode.GetDisplayValueTypeName(valueType));
            }

        }

    }
}
