using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{


    public class FlowInput
    {
        private string name;
        private FlowNode node;

        public FlowInput(FlowNode node, string name)
        {
            this.node = node;
            this.name = name;
        }
     


        public string Name
        {
            get { return name; }
        }

        public FlowNode Node
        {
            get { return node; }
        }

        public void Execute(Flow flow)
        {

            Node.DiryAllValueInputNodes(flow);
            Node.ExecuteAllValueInputNode(flow.Context, Node, flow);
            if (Node.Status == FlowNodeStaus.Complete)
            {
                Node.Flow(flow);
            }
        }

        public override string ToString()
        {
            return string.Format("FlowInput: {0}", Name);
        }

    }


}