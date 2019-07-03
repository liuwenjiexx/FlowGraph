using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using System.Linq;
using System;
using FlowGraph.Model;

namespace FlowGraph
{

    public class FlowGraphController : MonoBehaviour
    {
        [SerializeField]
        private FlowGraphField graph;

        private bool isStarted;
        private bool isRunning;

        private ReadOnlyCollection<FlowNode> nodesReadOnly;

        private ExecutionContext executeContext;
        private IContext context;


        public FlowGraphField GraphField
        {
            get
            {
                if (graph == null)
                    graph = new FlowGraphField();
                return graph;
            }
            set { graph = value; }
        }

        public FlowGraphData Graph
        {
            get
            {
                if (graph == null)
                    return null;
                FlowGraphData g = graph.GetFlowGraphData();
                return g;
            }
        }

        public ExecutionContext Context
        {
            get { return executeContext; }
        }


        // Use this for initialization
        void Start()
        {
            isStarted = true;
            context = GetComponentInParent<IContext>();
            if (Graph != null)
            {
                Run();
            }
        }

        void Initial()
        {

        }

        private void OnTransformParentChanged()
        {
            context = GetComponentInParent<IContext>();
        }

        private void Run()
        {
            if (isRunning)
                return;

            if (graph == null)
            {
                return;
            }

            executeContext = graph.CreateExecutionContext(context, gameObject, this);

            isRunning = true;
            foreach (var node in executeContext.Nodes)
            {
                node.OnGraphStarted(executeContext);
            }

        }


        private void OnEnable()
        {
            if (isStarted && !isRunning)
            {
                Run();
            }
        }

        private void OnDisable()
        {
            if (isRunning)
            {
                isRunning = false;

                foreach (var node in executeContext.Nodes)
                {
                    node.OnGraphStoped(executeContext);
                }
            }
        }



        bool IsValueNode(FlowNode node)
        {
            if (node.GetType().IsGenericSubclassOf(typeof(InputableValueNode<>)))
                return true;
            return false;
        }
        bool IsVariableNode(FlowNode node)
        {
            if (node.GetType().IsGenericSubclassOf(typeof(VariableNode<>)))
                return true;
            return false;
        }


    }

}