using FlowGraph.Model;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FlowGraph.Editor
{

    [CustomFlowNodeEditor(typeof(MemberNode))]
    class MemberNodeEditor : FlowNodeEditor
    {
        public override GUIContent GetNodeName()
        {

            MemberNode node = target as MemberNode;
            if (node == null)
            {
                return new GUIContent("missing member");
            }
            var member = node.Member;
            if (member == null)
                return new GUIContent("missing member");
            return new GUIContent(member.Name, member.MemberType + " " + member.DeclaringType.FullName);
        }

        public override void OnDetailsGUI()
        {
            MemberNode node = target as MemberNode;
            if (node == null)
            {
                base.OnDetailsGUI();
                return;
            }
            var member = node.Member;
            if (member == null)
            {
                base.OnDetailsGUI();
                return;
            }
            float maxWidth = GUILayoutUtility.GetRect(0, Screen.width, 0, 0).width;

            FlowGraphEditorWindow.detailsNameStyle.LabelFit(new GUIContent(member.Name, member.Name), (int)maxWidth);
            FlowGraphEditorWindow.detailsFullNameStyle.LabelFit(new GUIContent(member.DeclaringType.FullName), (int)maxWidth);
        }
    }
}