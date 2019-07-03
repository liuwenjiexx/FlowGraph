using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FlowGraph.Model;
using System.Reflection;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace FlowGraph.Editor
{
    [CustomPropertyDrawer(typeof(FlowGraphField))]
    public class FlowGraphFieldPropertyDrawer : PropertyDrawer
    {
        static FlowGraphField copy;
        static int variableLabelWidth = 60;
        static float variableCheckWidth = 20;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = base.GetPropertyHeight(property, label);
            float totalHeight = height;
            var field = GetGraphField(property);
            if (field != null)
            {
                var g = field.GetFlowGraphData();
                if (g != null)
                {
                    totalHeight += GetVariablesHeight(g.Variables);
                }
            }
            return totalHeight;
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var dataProperty = property.FindPropertyRelative("data");
            var assetDataProperty = property.FindPropertyRelative("assetData");
            var dataTypeProperty = property.FindPropertyRelative("dataType");
            FlowGraphField.DataType dataType = (FlowGraphField.DataType)dataTypeProperty.intValue;
            var assetData = assetDataProperty.objectReferenceValue as FlowGraphAsset;
            float totalHeight = GetPropertyHeight(property, label);
            Rect rowRect = new Rect(position.x, position.y, position.width, base.GetPropertyHeight(property, label));

            var targetObject = property.serializedObject.targetObject;

            var field = GetGraphField(property);

            FlowGraphData graphData = null;
            if (dataType == FlowGraphField.DataType.Asset)
            {
                if (assetData)
                {
                    //FlowGraphEditorWindow.ShowWindow(assetData);
                    graphData = assetData.Graph;
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

                //menu.AddItem(new GUIContent("Data"), dataType == FlowGraphField.DataType.Data, () =>
                // {
                //     dataType = FlowGraphField.DataType.Data;
                //     assetDataProperty.objectReferenceValue = null;
                //     dataTypeProperty.intValue = (int)dataType;
                //     property.serializedObject.ApplyModifiedProperties();
                // });
                //menu.AddItem(new GUIContent("Asset"), dataType == FlowGraphField.DataType.Asset, () =>
                //{
                //    if (graphData != null)
                //    {
                //        if (!EditorUtility.DisplayDialog("confirm clear data", "FlowGraph data not null", "ok", "cancel"))
                //        {
                //            return;
                //        }
                //    }
                //    dataType = FlowGraphField.DataType.Asset;
                //    dataTypeProperty.intValue = (int)dataType;
                //    property.serializedObject.ApplyModifiedProperties();
                //});

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
                            /*   if (copy.IsNull)
                               {
                                   field.AssetData = null;
                                   field.Data = null;
                               }
                               else*/
                            if (copy.IsAssetData)
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
                //menu.AddItem(new GUIContent("Clear Data"), false, () =>
                //{
                //    if (EditorUtility.DisplayDialog("Clear data", "Clear FlowGraph data", "ok", "cancel"))
                //    {
                //        Undo.RecordObject(property.serializedObject.targetObject, null);
                //        assetDataProperty.objectReferenceValue = null;
                //        dataType = FlowGraphField.DataType.Null;
                //        dataTypeProperty.intValue = (int)dataType;
                //        property.serializedObject.ApplyModifiedProperties();
                //    }

                //});
                menu.ShowAsContext();
            }

            rowRect = new Rect(rowRect);
            int btnWidth = 40;
            //rowRect.width =rowRect.width -btnWidth;

            FlowGraphField.DataType newDataType = (FlowGraphField.DataType)EditorGUI.EnumPopup(new Rect(rowRect.x, rowRect.y, 60, rowRect.height), GUIContent.none, dataType);

            if (newDataType != dataType)
            {
                bool cancel = false;
                if (newDataType == FlowGraphField.DataType.Data)
                {
                    assetDataProperty.objectReferenceValue = null;
                }
                else if (newDataType == FlowGraphField.DataType.Asset)
                {
                    if (graphData != null && !graphData.IsEmpty)
                    {
                        cancel = !EditorUtility.DisplayDialog("confirm clear data", "FlowGraph data not null", "ok", "cancel");
                    }
                }

                if (!cancel)
                {
                    dataType = newDataType;
                    dataTypeProperty.intValue = (int)dataType;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            rowRect.xMax -= btnWidth;
            rowRect.xMin += 60;

            //if (assetData != null && assetData.Data != null && assetData.Data.HasDeserializeError)
            //    GUI.color = Color.red;
            if (dataType == FlowGraphField.DataType.Asset)
            {
                var newValue = EditorGUI.ObjectField(rowRect, assetData, typeof(FlowGraphAsset), true) as FlowGraphAsset;
                //GUI.color = Color.white;
                if (newValue != assetData)
                {
                    assetData = newValue;
                    if (newValue)
                    {
                        dataType = FlowGraphField.DataType.Asset;
                    }
                    //else
                    //{
                    //    dataType = FlowGraphField.DataType.Data;
                    //}
                    foreach (var target in property.serializedObject.targetObjects)
                    {
                        assetDataProperty.objectReferenceValue = newValue;
                        dataTypeProperty.intValue = (int)dataType;
                    }

                    property.serializedObject.ApplyModifiedProperties();


                }
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
                    if (graphData == null || (((graphData.flags & FlowGraphDataFlags.InitializedNew) != FlowGraphDataFlags.InitializedNew) && graphData.IsEmpty))
                    {
                        Undo.RecordObject(property.serializedObject.targetObject, null);
                        if (graphData == null)
                        {
                            graphData = new FlowGraphData();
                            field.Data = graphData;
                        }
                        FlowGraphData.InitializedNewGraph(graphData);


                        EditorUtility.SetDirty(property.serializedObject.targetObject);
                    }

                    FlowGraphEditorWindow.ShowWindow(property.serializedObject.targetObject, graphData, property.propertyPath);
                }
            }
            rowRect.y += rowRect.height;

            using (new GUIDepthScope())
            {

                if (field != null)
                {
                    var graph = field.GetFlowGraphData();
                    if (graph != null)
                    {
                        using (var check = new EditorGUI.ChangeCheckScope())
                        {

                            field.Inputs = DrawVariables(new Rect(0, rowRect.y, position.width, totalHeight - rowRect.y), graph.Variables, field.GetAllOvrrideInputs().ToDictionary(o => o.Name), field.Inputs);
                            if (check.changed)
                            {

                                if (AssetDatabase.IsNativeAsset(property.serializedObject.targetObject))
                                {
                                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                                }
                                else
                                {

                                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                                }
                            }
                        }
                    }
                }
            }
        }

        public static float GetVariablesHeight(IEnumerable<VariableInfo> variables)
        {
            float height = 0;
            int n = variables.Where(o => o.IsIn).Count();
            // if (n > 0)
            {
                height += (EditorGUIUtility.singleLineHeight * (n + 1));
            }
            n = variables.Where(o => o.IsOut).Count();
            //   if (n > 0)
            {
                height += (EditorGUIUtility.singleLineHeight * (n + 1));
            }
            return height;
        }


        public static List<VariableInfo> DrawVariables(Rect position, IEnumerable<VariableInfo> variables, Dictionary<string, VariableInfo> ovrrideInputs, List<VariableInfo> inputs)
        {
            if (inputs != null)
            {
                for (int i = 0; i < inputs.Count; i++)
                {
                    var input = inputs[i];
                    var variable = variables.Where(o => o.Name == input.Name).FirstOrDefault();
                    if (variable == null || variable.Type != input.Type)
                    {
                        inputs.RemoveAt(i);
                        i--;
                        GUI.changed = true;
                    }
                }
            }

            Rect rowRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            GUIStyle labelStyle = new GUIStyle("label");
            //    labelStyle.fontStyle = FontStyle.Bold;

            int inputCount = variables.Where(o => o.IsIn).Count();
            // if (inputCount > 0)
            {
                rowRect.x = position.x;
                rowRect.width = position.width;
                rowRect.xMin += GUI.depth * GUIDepthScope.Pixels;

                //rowRect = EditorGUI.PrefixLabel(rowRect, new GUIContent("Input"), labelStyle);
                GUI.Label(GUIExtensions.Width(ref rowRect, EditorGUIUtility.labelWidth), new GUIContent("Input"), labelStyle);
                rowRect.y += rowRect.height;
                using (new GUIDepthScope())
                {

                    variables.Where(o => o.IsIn).OrderBy(o => o.Name).Where((variable, index) =>
                    {
                        VariableInfo inVar = null;

                        if (inputs != null)
                            inVar = inputs.Where(o => o.Name == variable.Name).FirstOrDefault();

                        if (inVar != null && inVar.Type != variable.Type)
                        {
                            inputs.Remove(inVar);
                            inVar = null;
                            GUI.changed = true;
                        }


                        rowRect.x = position.x;
                        rowRect.width = position.width;
                        rowRect.xMin += GUI.depth * GUIDepthScope.Pixels;

                        if (inVar == null)
                        {
                            GUI.color = Color.gray;
                        }
                        else
                        {
                            GUI.color = Color.white;
                        }
                        GUI.Label(GUIExtensions.Width(ref rowRect, variableLabelWidth), new GUIContent(variable.Name));

                        GUI.color = Color.white;

                        var tmpRect = GUIExtensions.Width(ref rowRect, rowRect.width - variableCheckWidth);
                        if (inVar != null)
                        {
                            object newValue2;

                            newValue2 = SerializableValuePropertyDrawer.ValueField(tmpRect, inVar.DefaultValue, inVar.Type);
                            if (!object.Equals(newValue2, inVar.DefaultValue))
                            {
                                inVar.DefaultValue = newValue2;
                                GUI.changed = true;
                            }
                        }
                        else
                        {
                            GUI.enabled = false;

                            object value = variable.DefaultValue;
                            if (ovrrideInputs.ContainsKey(variable.Name))
                                value = ovrrideInputs[variable.Name].DefaultValue;
                            SerializableValuePropertyDrawer.ValueField(tmpRect, value, variable.Type);
                            GUI.enabled = true;
                        }


                        rowRect.xMin += 5f;

                        if (EditorGUI.Toggle(rowRect, inVar != null))
                        {
                            if (inVar == null)
                            {
                                inVar = new VariableInfo(variable.Name, variable.Type, variable.DefaultValue);
                                if (inputs == null)
                                    inputs = new List<VariableInfo>();
                                inputs.Add(inVar);
                                GUI.changed = true;
                            }
                        }
                        else
                        {
                            if (inVar != null)
                            {
                                inputs.Remove(inVar);
                                GUI.changed = true;
                            }
                        }

                        rowRect.y = rowRect.yMax;
                        return false;
                    }).Count();
                }
            }

            int outputCount = variables.Where(o => o.IsOut).Count();
            //     if (outputCount > 0)
            {
                rowRect.x = position.x;
                rowRect.width = position.width;
                rowRect.xMin += GUI.depth * GUIDepthScope.Pixels;
                //rowRect = EditorGUI.PrefixLabel(rowRect, new GUIContent("Output"), labelStyle);

                GUI.Label(GUIExtensions.Width(ref rowRect, EditorGUIUtility.labelWidth), new GUIContent("Output"), labelStyle);
                rowRect.y += rowRect.height;

                using (new GUIDepthScope())
                {
                    variables.Where(o => o.IsOut).OrderBy(o => o.Name).Where((variable, index) =>
                    {

                        rowRect.x = position.x;
                        rowRect.width = position.width;
                        rowRect.xMin += GUI.depth * GUIDepthScope.Pixels;

                        GUI.Label(GUIExtensions.Width(ref rowRect, variableLabelWidth), new GUIContent(variable.Name));
                        GUI.Label(rowRect, new GUIContent(FlowNode.GetDisplayValueTypeName(variable.Type), variable.Type.FullName));
                        rowRect.y += rowRect.height;
                        return false;
                    }).Count();
                }
            }
            return inputs;
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
