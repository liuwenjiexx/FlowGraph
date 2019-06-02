using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{

    [Name("Start")]
    [Category(EventsCategory + "/Graph")]
    public class GraphStartEvent : FlowNode
    {
        private FlowOutput startFlowOut;

        protected override void RegisterPorts()
        {
            startFlowOut = AddFlowOutput("Start");
        }

        public override void OnGraphStarted(ExecutionContext context)
        {
            base.OnGraphStarted(context);
            startFlowOut.Execute(context);
        }


    }

    [Name("Stop")]
    [Category(EventsCategory + "/Graph")]
    public class GraphStopEvent : FlowNode
    {
        private FlowOutput stopFlowOut;

        protected override void RegisterPorts()
        {
            stopFlowOut = AddFlowOutput("Stop");
        }


        public override void OnGraphStoped(ExecutionContext context)
        {
            base.OnGraphStoped(context);
            stopFlowOut.Execute(context);
        }

    }

}