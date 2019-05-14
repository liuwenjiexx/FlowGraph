using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FlowGraph.Model;
using System.Linq;



namespace FlowGraph.Editor
{
    [CustomEditor(typeof(FlowGraphController))]
    public class FlowGraphControllerEditor : UnityEditor.Editor
    {
        SerializedProperty graphProperty;

        private void OnEnable()
        {

            graphProperty = serializedObject.FindProperty("graph");
        }

        public override void OnInspectorGUI()
        {
            FlowGraphController controller = target as FlowGraphController;


            FlowGraphData graph = null;
            if (controller.Graph != null)
                graph = controller.Graph.GetFlowGraphData();

            //if (graph != null)
            //{
            //    if (graph.HasDeserializeError)
            //    {
            //        GUI.color = Color.red;
            //    }
            //}

            EditorGUILayout.PropertyField(graphProperty);

            //GUI.color = Color.white;
            if (graph == null)
                return;


            serializedObject.ApplyModifiedProperties();
        }



    }
}