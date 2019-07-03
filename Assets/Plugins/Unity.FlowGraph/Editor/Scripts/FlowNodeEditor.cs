using FlowGraph.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FlowGraph.Editor
{
    public class FlowNodeEditor
    {

        public object target;
        private FlowGraphData graph;
        public bool selected;
        public UnityEngine.Object targetObject;
        public FlowNodeData nodeData;

        public FlowGraphData Graph
        {
            get
            {
                return graph;
            }

            set
            {
                graph = value;
            }
        }

        public virtual void OnNodeNameGUI(Rect rect, GUIStyle labelStyle)
        {
            labelStyle.LabelFit(rect, GetNodeName());
        }

        public virtual GUIContent GetNodeName()
        {
            FlowNode node = target as FlowNode;
            string name;

            var nodeInfo = FlowGraphEditorWindow.GetNodeInfo(node);
            if (nodeInfo != null)
            {
                name = nodeInfo.Name;
            }
            else
            {
                name = node.GetType().Name;
            }
            return new GUIContent(name, node.GetType().FullName);
        }

        public virtual void OnDetailsGUI()
        {
            bool changed = false;
            FlowNode node = target as FlowNode;
            FlowNodeInfo nodeInfo = FlowGraphEditorWindow.GetNodeInfo(node);

            if (node == null || nodeInfo == null|| nodeData.HasDeserializeError)
            {
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel("nodeType");

                    //string newValue = ((GUIStyle)"label").LabelEditable(new GUIContent(nodeData.TypeName ?? "", nodeData.TypeName ?? ""));
                    string newType;
                    newType = EditorGUILayout.DelayedTextField(nodeData.TypeName ?? string.Empty);
                    if (nodeData.TypeName != newType)
                    {
                        //Undo.RecordObject(Target, null);
                        nodeData.TypeName = newType;
                        //SetTargetDirty();
                        GUI.changed = true;
                        TryDeserialize();
                    }

                }


                var props = nodeData.Properties;
                if (props != null)
                {
                    foreach (var p in props)
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            EditorGUILayout.PrefixLabel(new GUIContent(p.field, p.field));

                            if (p.value.Value != null && p.value.TypeCode == SerializableValue.SerializableTypeCode.String)
                            {
                                string strValue = EditorGUILayout.DelayedTextField((string)p.value.Value);
                                if (!object.Equals(strValue, p.value.Value))
                                {
                                    p.value.Value = strValue;
                                    GUI.changed = true;
                                    TryDeserialize();
                                }

                            }
                            else
                            {
                                if (p.value.Value == null)
                                {
                                    GUILayout.Label("null");
                                }
                                else
                                {
                                    GUILayout.Label(new GUIContent(p.value.Value.ToString(), p.value.Value.ToString()));
                                }
                            }

                        }
                    }
                }

                return;
            }


            if (node != null)
            {


                float maxWidth = GUILayoutUtility.GetRect(0, Screen.width, 0, 0).width;


                FlowGraphEditorWindow.detailsNameStyle.LabelFit(new GUIContent(nodeInfo.Name, nodeInfo.Name), (int)maxWidth);

                FlowGraphEditorWindow.detailsFullNameStyle.LabelFit(new GUIContent(nodeInfo.DisplayFullName, nodeInfo.DisplayFullName), (int)maxWidth);
                //if (nodeInfo.NodeType == NodeType.Value)
                if (nodeInfo.dataMembers != null && nodeInfo.dataMembers.Count > 0)
                {
                    Action<MemberInfo> drawMember = (mInfo) =>
                    {
                        FieldInfo field = mInfo as FieldInfo;
                        if (field != null)
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                //GUILayout.Label(field.Name, GUILayout.Width(detailsFieldLabelWidth));
                                EditorGUILayout.PrefixLabel(field.Name);
                                object value = field.GetValue(node), newValue;

                                newValue = SerializableValuePropertyDrawer.LayoutValueField(value, field.FieldType);
                                if (!object.Equals(newValue, value))
                                {
                                    Undo.RecordObject(targetObject, null);
                                    field.SetValue(node, newValue);
                                    changed = true;
                                    //SetTargetDirty();
                                    node.OnAfterDeserialize();

                                }
                            }
                            return;
                        }
                        PropertyInfo pInfo = mInfo as PropertyInfo;
                        if (pInfo != null)
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                EditorGUILayout.PrefixLabel(pInfo.Name);//, GUILayout.Width( detailsFieldLabelWidth)
                                object value = pInfo.GetValue(node, null), newValue;

                                newValue = SerializableValuePropertyDrawer.LayoutValueField(value, pInfo.PropertyType);
                                if (!object.Equals(newValue, value))
                                {
                                    Undo.RecordObject(targetObject, null);
                                    pInfo.SetValue(node, newValue, null);
                                    //SetTargetDirty();
                                    changed = true;
                                }
                            }
                        }
                    };

                    if (nodeInfo.dataMembers != null)
                    {
                        foreach (var m in nodeInfo.dataMembers)
                        {
                            if (m.IsDefined(typeof(HideInInspector), false))
                                continue;
                            drawMember(m);
                        }
                    }

                }
            }

            if (changed)
            {
                GUI.changed = true;
            }

        }
        private void TryDeserialize()
        {
            if (nodeData != null)
            {
                nodeData.OnAfterDeserialize();
            }
            Graph.OnAfterDeserialize();
        }

    }
}
