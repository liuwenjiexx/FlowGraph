using FlowGraph.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FlowGraph.Editor
{

    [CustomFlowNodeEditor(typeof(VariableNode<>))]
    class GetVariableNodeEditor : FlowNodeEditor
    {
        string VariableName
        {
            get
            {
                var nameProperty = target.GetType().GetProperty("Name");
                string varName = nameProperty.GetGetMethod().Invoke(target, null) as string;

                return varName;
            }
        }

        Type VariableType
        {
            get
            {
                Type varType = target.GetType().FindGenericType(typeof(VariableNode<>)).GetGenericArguments()[0];

                return varType;
            }
        }

        bool IsGetVariable
        {
            get
            {
                return target.GetType().IsGenericSubclassOf(typeof(GetVariableNode<>));
            }
        }



        public override void OnNodeNameGUI(Rect rect, GUIStyle labelStyle)
        {
            FlowNode node = target as FlowNode;
            string varName = VariableName;
            Type varType = VariableType;
            GUIContent content;
            content = new GUIContent("$" + varName, "Get Variable(" + FlowNode.GetDisplayValueTypeName(varType) + ")");

            var variable = Graph.GetVariable(varName);
            if (variable == null)
            {
                content.tooltip = string.Format("not variable '{0}'", varName);
                GUI.color = Color.red;
            }
            else
            {

                if (variable.Type != varType)
                {
                    content.tooltip = string.Format("variable type no match", variable.Type, varType);
                    GUI.color = Color.red;
                }
            }


            labelStyle.LabelFit(rect, content);
            GUI.color = Color.white;
        }



        public override void OnDetailsGUI()
        {
            Type type = VariableType;
            string name = VariableName;
            bool isGet = IsGetVariable;
            float maxWidth = GUILayoutUtility.GetRect(0, Screen.width, 0, 0).width;

            FlowGraphEditorWindow.detailsNameStyle.LabelFit(new GUIContent(IsGetVariable ? "Get Variable" : "Set Variable"), (int)maxWidth);

            var variable = Graph.GetVariable(name);

            using (new GUILayout.HorizontalScope())
            {
                if (variable == null)
                {
                    GUI.color = Color.red;
                }
                GUILayout.Label("Name", GUILayout.Width(EditorGUIUtility.labelWidth));
                GUI.color = Color.white;
                GUILayout.Label(name);
            }

            using (new GUILayout.HorizontalScope())
            {
                if (variable != null && variable.Type != type)
                {
                    GUI.color = Color.red;
                }
                GUILayout.Label("Type", GUILayout.Width(EditorGUIUtility.labelWidth));
                GUI.color = Color.white;
                GUILayout.Label(FlowNode.GetDisplayValueTypeName(type));
            }

        }

    }
}