using FlowGraph.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph
{

    public class Flow
    {
        private ExecutionContext context;
        private Flow parent;
        private FlowOutput output;
        private Dictionary<ValuePort, object> outputs;


        
        public Flow(Flow parent, ExecutionContext context, FlowOutput output)
        {

            this.context = context;
            this.parent = parent;
            this.output = output;
            if (output != null)
            {
                RemoveParent(output.Node);

                foreach (var port in output.Node.ValueOutputs)
                {
                    if (outputs == null)
                        outputs = new Dictionary<ValuePort, object>();
                    outputs[port] = port.GetValue(context);
                }

            }

        }

        public ExecutionContext Context
        {
            get
            {
                return context;
            }
        }

        public Flow Parent
        {
            get
            {
                return parent;
            }
        }

        public FlowNode Node
        {
            get
            {
                if (output != null)
                    return output.Node;
                return null;
            }
        }

        public FlowOutput Output
        {
            get
            {
                return output;
            }
        }

        public object GetOutput(ValuePort output)
        {
            return outputs[output];
        }



        public bool TryGetOutput(FlowNode node, ValuePort output, out object value)
        {
            var f = FindFlow(node);
            if (f != null)
            {
                if (outputs != null && outputs.TryGetValue(output, out value))
                    return true;
            }
            value = null;
            return false;
        }

        public Flow FindFlow(FlowNode flowNode)
        {

            Flow f = this;
            while (f != null)
            {
                if (f.Node == flowNode)
                    return f;
                f = f.parent;
            }
            return null;
        }

        private void RemoveParent(FlowNode node)
        {
            if (parent == null)
                return;
            if (parent.output != null && parent.output.Node == node)
            {
                parent = null;
            }
            else
            {
                parent.RemoveParent(node);
            }
        }


    }

}