using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{

    public class FlowOutput
    {
        private FlowInput linkInput;

        public FlowOutput(FlowNode node, string name)
        {
            this.Node = node;
            this.Name = name;
        }

        public string Name { get; private set; }

        public FlowNode Node { get; private set; }

        public FlowInput LinkInput
        {
            get
            {
                return linkInput;
            }

            set
            {
                if (value != linkInput)
                {
                    linkInput = value;
                }
            }
        }


        public void Execute(ExecutionContext context)
        {
            if (LinkInput == null)
                return;
            using (new UnityEngine.Profiling.ProfilerSample("FlowGraph.Flow." + Node.GetType().Name + "." + Name))
            {
                Flow flow = new Flow(null, context, this);
                LinkInput.Execute(flow);
            }
        }

        public void Execute(Flow flow)
        {
            if (LinkInput == null)
                return;
            LinkInput.Execute(new Flow(flow, flow.Context, this));
        }


        public static void Bind(FlowOutput output, FlowInput input)
        {
            if (output != null)
            {
                if (output.LinkInput != null)
                {
                    //output.Input.Output = null;
                    output.LinkInput = null;
                }
            }
            //if (input != null)
            //{
            //    if (input.Output != null)
            //    {
            //        input.Output.Input = null;
            //        input.Output = null;
            //    }
            //}
            if (output != null && input != null)
            {
                output.LinkInput = input;
                //input.Output = output;
            }
        }

        public override string ToString()
        {
            return string.Format("FlowOutput: {0}", Name);
        }
    }

    public class FlowBinding
    {
        public FlowOutput Output { get; private set; }
        public FlowInput Input { get; private set; }

        public FlowBinding(FlowOutput output, FlowInput input)
        {
            this.Output = output;
            this.Input = input;
        }

    }

}