using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FlowGraph.Model;
using System.Reflection;
using System.Linq;

namespace FlowGraph.Editor
{
    [CustomPropertyDrawer(typeof(FlowGraphField))]
    public class FlowGraphFieldPropertyDrawer : PropertyDrawer
    {
        static FlowGraphField copy;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = base.GetPropertyHeight(property, label);
            float totalHeight = height;
            /* var field = GetGraphField(property);
             if (field != null)
             {
                 var g = field.GetFlowGraphData();
                 if (g != null)
                 {
                     int n = g.Variables.Where(o => o.IsIn).Count();
                     if (n > 0)
                     {
                         totalHeight += (height * (n + 1));
                     }
                 }
             }*/
            return totalHeight;
        }



        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var dataProperty = property.FindPropertyRelative("data");
            var assetDataProperty = property.FindPropertyRelative("assetData");
            var dataTypeProperty = property.FindPropertyRelative("dataType");
            FlowGraphField.DataType dataType = (FlowGraphField.DataType)dataTypeProperty.intValue;
            var assetData = assetDataProperty.objectReferenceValue as FlowGraphAsset;
            float height = base.GetPropertyHeight(property, label);
            Rect rowRect = new Rect(position.x, position.y, position.width, height);

            float space = 16;
            var targetObject = property.serializedObject.targetObject;

            var field = GetGraphField(property);

            FlowGraphData graphData = null;
            if (dataType == FlowGraphField.DataType.Asset)
            {
                if (assetData)
                {
                    FlowGraphEditorWindow.ShowWindow(assetData);
                    graphData = assetData.Data;
                }
            }
            else
            {
                graphData = field.GetFlowGraphData();
            }

            if (graphData != null && graphData.HasDeserializeError)
            {
                GUI.color = Color.red;
                //label.text = "err";
                //Debug.Log("has errr");
            }
            //Debug.Log("null:" + (graphData == null)+","+graphData.IsDeserialize+","+graphData.HasDeserializeError);


            rowRect = EditorGUI.PrefixLabel(rowRect, label);
            GUI.color = Color.white;

            if (GUI.Button(new Rect(position.x, position.y, position.width - rowRect.width, rowRect.height), GUIContent.none, "label"))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Fix"), false, () =>
                {

                    if (assetDataProperty.objectReferenceValue)
                    {
                        dataType = FlowGraphField.DataType.Asset;
                    }
                    else
                    {
                        dataType = FlowGraphField.DataType.Data;
                    }
                    dataTypeProperty.intValue = (int)dataType;

                    property.serializedObject.ApplyModifiedProperties();
                });
                menu.AddItem(new GUIContent("Copy"), false, () =>
                {
                    copy = field;
                });
                if (copy != null)
                {
                    menu.AddItem(new GUIContent("Paste"), false, () =>
                    {
                        if (copy != null)
                        {
                            Undo.RecordObject(property.serializedObject.targetObject, null);
                            if (copy.IsNull)
                            {
                                field.AssetData = null;
                                field.Data = null;
                            }
                            else if (copy.IsAssetData)
                            {
                                field.AssetData = copy.AssetData;
                            }
                            else
                            {
                                field.Data = copy.Data;
                            }
                            property.serializedObject.ApplyModifiedProperties();
                        }
                    });
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Paste"));
                }
                menu.AddItem(new GUIContent("Clear Data"), false, () =>
                {
                    Undo.RecordObject(property.serializedObject.targetObject, null);
                    assetDataProperty.objectReferenceValue = null;
                    dataType = FlowGraphField.DataType.Null;
                    dataTypeProperty.intValue = (int)dataType;
                    property.serializedObject.ApplyModifiedProperties();

                });
                menu.ShowAsContext();
            }

            rowRect = new Rect(rowRect);

            GUI.Label(new Rect(rowRect.x, rowRect.y, 40, rowRect.height), dataType.ToString());

            int btnWidth = 40;
            rowRect.xMax -= btnWidth;
            rowRect.xMin += 40;

            //if (assetData != null && assetData.Data != null && assetData.Data.HasDeserializeError)
            //    GUI.color = Color.red;

            var newValue = EditorGUI.ObjectField(rowRect, assetData, typeof(FlowGraphAsset), true) as FlowGraphAsset;
            //GUI.color = Color.white;
            if (newValue != assetData)
            {
                assetData = newValue;
                if (newValue)
                {
                    dataType = FlowGraphField.DataType.Asset;
                }
                else
                {
                    dataType = FlowGraphField.DataType.Null;
                }
                foreach (var target in property.serializedObject.targetObjects)
                {
                    assetDataProperty.objectReferenceValue = newValue;
                    dataTypeProperty.intValue = (int)dataType;
                }

                property.serializedObject.ApplyModifiedProperties();

            }
            rowRect = new Rect(rowRect);
            rowRect.xMin = rowRect.xMax;
            rowRect.width = btnWidth;

            if (GUI.Button(rowRect, "Edit"))
            {


                //if (field != null)
                //{

                //    FlowGraphEditorWindow.ShowWindow(property.serializedObject.targetObject,  property.propertyPath);
                //}
                if (dataType == FlowGraphField.DataType.Asset)
                {
                    if (assetData)
                    {
                        FlowGraphEditorWindow.ShowWindow(assetData);
                    }
                }
                else
                {
                    graphData = field.GetFlowGraphData();
                    if (graphData == null)
                    {
                        Undo.RecordObject(property.serializedObject.targetObject, null);
                        graphData = FlowGraphEditorWindow.CreateGraph();
                        field.Data = graphData;
                        EditorUtility.SetDirty(property.serializedObject.targetObject);
                    }
                    FlowGraphEditorWindow.ShowWindow(property.serializedObject.targetObject, graphData);
                }
            }
            rowRect = new Rect(rowRect);
            rowRect.x = position.x;
            rowRect.y += rowRect.height;
            rowRect.width = position.width;

            if (field != null)
            {
                var graph = field.GetFlowGraphData();
                if (graph != null)
                {

                    //if (field.Inputs != null)
                    //{
                    //    for (int i = 0; i < field.Inputs.Count; i++)
                    //    {
                    //        var variable = field.Inputs[i];
                    //        if (!graph.HasVariable(variable.Name))
                    //        {
                    //            GUI.changed = true;
                    //            field.Inputs.RemoveAt(i);
                    //            i--;
                    //        }
                    //    }
                    //}

                    /* int inputCount = graph.Variables.Where(o => o.IsIn).Count();
                     if (inputCount > 0)
                     {
                         GUIStyle labelStyle = new GUIStyle("label");
                         labelStyle.fontStyle = FontStyle.Bold;
                         rowRect.x += space;
                         rowRect.width -= space;
                         GUI.Label(rowRect, new GUIContent("Inputs"), labelStyle);
                         rowRect.y = rowRect.yMax;
                         rowRect.x += space;
                         rowRect.width -= space;
                        graph.Variables.Where(o => o.IsIn).Where((variable, index) =>
                         {
                             var inVar = field.Inputs.Where(o => o.Name == variable.Name).FirstOrDefault();

                             if (inVar != null && inVar.Type != variable.Type)
                             {
                                 field.Inputs.Remove(inVar);
                                 inVar = null;
                             }
                             if (inVar == null)
                             {
                                 inVar = new VariableInfo(variable.Name, variable.Type, variable.DefaultValue);
                                 field.Inputs.Add(inVar);
                             }
                             int labelWidth = 60;
                             GUI.Label(new Rect(rowRect.x, rowRect.y, labelWidth, rowRect.height), new GUIContent(variable.Name));


                             object newValue2;
                             newValue2 = FlowGraphEditorWindow.ValueField(new Rect(rowRect.x + labelWidth, rowRect.y, rowRect.width - labelWidth, rowRect.height), inVar.DefaultValue, inVar.Type);
                             if (!object.Equals(newValue, inVar.DefaultValue))
                             {
                                 GUI.changed = true;
                                 inVar.DefaultValue = newValue2;
                             }
                             rowRect.y = rowRect.yMax;
                             return false;
                         }).Count();
                     }*/
                }
            }

        }

        FlowGraphField GetGraphField(SerializedProperty property)
        {/*
            UnityEngine.Object obj = property.serializedObject.targetObject;
            var fieldProp = obj.GetType().GetField(property.propertyPath, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldProp == null)
                Debug.LogError("Field null, " + property.propertyPath + ", " + obj);
            var field = fieldProp.GetValue(obj) as FlowGraphField;
            return field;*/
            var field = property.GetObjectOfProperty() as FlowGraphField;
            return field;
        }
    }
}
