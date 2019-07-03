using FlowGraph.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;


namespace FlowGraph.Model
{
    [Serializable]
    public class FlowGraphAsset : FlowAsset
    {
        [SerializeField]
        private FlowGraphData graph;



        public FlowGraphData Graph
        {
            get { return graph; }
            set { graph = value; }
        }

        public override FlowNode CreateFlowNode()
        {
            var g = Graph;
            if (g != null)
            {
                if (!g.IsInitial)
                    g.Initial();
            }

            return new FlowGraphNode(this);
        }

        public ExecutionContext CreateExecutionContext(IContext parent, GameObject go, MonoBehaviour mono)
        {
            ExecutionContext context = null;
            if (graph != null)
            {
                context = graph.CreateExecutionContext(parent, go, mono);
            }

            if (context != null)
            {
                var inputs = Inputs;
                context.OvrrideInputs(inputs);
            }
            return context;
        }

    }
}