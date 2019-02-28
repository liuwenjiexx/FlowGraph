using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using FlowGraph.Model;

namespace FlowGraph.Editor
{

    [CustomEditor(typeof(FlowGraphAsset))]
    public class FlowGraphAssetEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            FlowGraphAsset asset = (FlowGraphAsset)target;


            if (GUILayout.Button("Open"))
            {
                FlowGraphEditorWindow.ShowWindow(asset);
            }

            GUILayout.Space(6);

            GUILayout.Label("Vars");
            foreach (var var1 in asset.Data.Variables.OrderBy(o =>
            {
                switch (o.Mode)
                {
                    case VariableMode.In:
                        return 1;
                    case VariableMode.InOut:
                        return 2;
                    case VariableMode.Out:
                        return 3;
                }
                return 0;
            }))
            {
                using (new GUILayout.HorizontalScope())
                {
                    string typeName;
                    typeName = FlowNode.GetValueTypeName(var1.Type);
                    if ((var1.Mode & VariableMode.In) == VariableMode.In)
                        typeName += "(In)";
                    GUILayout.Label(typeName, GUILayout.ExpandWidth(false));
                    GUILayout.Label(var1.Name, GUILayout.ExpandWidth(true));
                    GUILayout.Label(FlowGraphEditorWindow.GetInOutChar(var1.Mode), GUILayout.ExpandWidth(false));
                }
            }


        }

    }
}