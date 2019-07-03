//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using System.Linq;
//using FlowGraph.Model;
//using UnityEditor.Callbacks;

//namespace FlowGraph.Editor
//{

//    [CustomEditor(typeof(FlowGraphAsset))]
//    public class FlowGraphAssetEditor : UnityEditor.Editor
//    {

//        public override void OnInspectorGUI()
//        {
//            FlowGraphAsset asset = (FlowGraphAsset)target;

//            GUILayout.Space(6);

//            GUILayout.Label("Inputs");

//            foreach (var var1 in asset.Graph.Variables.Where(o => (o.Mode & VariableMode.In) == VariableMode.In).OrderBy(o => o.Name))
//            {
//                using (new GUILayout.HorizontalScope())
//                {
//                    GUILayout.Space(16);
//                    string typeName;
//                    typeName = FlowNode.GetValueTypeName(var1.Type);

//                    GUILayout.Label(var1.Name, GUILayout.ExpandWidth(true));
//                    GUILayout.Label(typeName, GUILayout.ExpandWidth(false));
//                    //EditorGUILayout.PrefixLabel(typeName);
//                    GUILayout.Label(FlowGraphEditorWindow.GetInOutChar(var1.Mode), GUILayout.ExpandWidth(false));
//                }
//            }

//            GUILayout.Space(6);
//            GUILayout.Label("Outputs");
//            foreach (var var1 in asset.Graph.Variables.Where(o => (o.Mode & VariableMode.Out) == VariableMode.Out).OrderBy(o => o.Name))
//            {
//                using (new GUILayout.HorizontalScope())
//                {

//                    GUILayout.Space(16);
//                    string typeName;
//                    typeName = FlowNode.GetValueTypeName(var1.Type);

//                    GUILayout.Label(var1.Name, GUILayout.ExpandWidth(true));
//                    GUILayout.Label(typeName, GUILayout.ExpandWidth(false));
//                    //EditorGUILayout.PrefixLabel(typeName);
//                    GUILayout.Label(FlowGraphEditorWindow.GetInOutChar(var1.Mode), GUILayout.ExpandWidth(false));
//                }
//            }

//        }


//    }
//}