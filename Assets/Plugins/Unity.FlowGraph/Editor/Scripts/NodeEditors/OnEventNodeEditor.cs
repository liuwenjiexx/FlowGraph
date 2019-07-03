using FlowGraph.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FlowGraph.Editor
{

    [CustomFlowNodeEditor(typeof(OnEventNode))]
    public class OnEventNodeEditor : FlowNodeEditor
    {

        public override void OnNodeNameGUI(Rect rect, GUIStyle labelStyle)
        {
            var eventNode = target as OnEventNode;

            string eventName = eventNode.EventName;
            GUIContent content;
            content = new GUIContent("@" + eventName, "OnEvent");

            if (string.IsNullOrEmpty(eventName))
            {
                content.tooltip = "Event Name null";
                GUI.color = Color.red;
            }

            labelStyle.LabelFit(rect, content);
            GUI.color = Color.white;
        }


    }


}