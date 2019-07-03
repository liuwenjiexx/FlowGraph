using FlowGraph.Model;
using UnityEditor;

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


            FlowGraphData graph = controller.Graph;

            //if (graph != null)
            //{
            //    if (graph.HasDeserializeError)
            //    {
            //        GUI.color = Color.red;
            //    }
            //}

            EditorGUILayout.PropertyField(graphProperty);

            serializedObject.ApplyModifiedProperties();
            //GUI.color = Color.white;
            if (graph == null)
                return;


        }



    }
}