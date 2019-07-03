using FlowGraph.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;


namespace FlowGraph.Editor
{
    [CustomEditor(typeof(FlowAsset), true)]
    public class FlowAssetEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            FlowAsset asset = (FlowAsset)target;

            var node = asset.CreateFlowNode();
            if (node == null)
            {
                GUILayout.Label("node null");
                return;
            }

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                GUILayout.Space(6);
                var variables = node.ValueInputs.Select(o => new VariableInfo(o.Name, o.ValueType, null) { Mode = VariableMode.In })
                    .Concat(node.ValueOutputs.Select(o => new VariableInfo(o.Name, o.ValueType, null) { Mode = VariableMode.Out })).ToArray();


                float inputsHeight = FlowGraphFieldPropertyDrawer.GetVariablesHeight(variables);

                Rect rect = GUILayoutUtility.GetRect(0, Screen.width, inputsHeight, inputsHeight);
                rect.x = 0;
                asset.Inputs = FlowGraphFieldPropertyDrawer.DrawVariables(rect, variables, asset.GetAllOvrrideInputs().ToDictionary(o => o.Name), asset.Inputs);
                if (check.changed)
                    EditorUtility.SetDirty(target);
            }
        }
    }


}