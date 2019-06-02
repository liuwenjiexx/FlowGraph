using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{
    [Name("Stopwatch")]
    [Category(EventsCategory)]
    public class Stopwatch : FlowNode
    {
        private ValuePort<float> timeIn;
        private ValuePort<float> deltaIn;
        private ValuePort<float> elapsedOut;
        private bool isRunning;
        protected override void RegisterPorts()
        {
            AddFlowInput(FLOW_IN);
            AddFlowOutput("done");
            AddFlowOutput("tick");
            timeIn = AddValueInput<float>("time");
            deltaIn = AddValueInput<float>("delta", new Inject(InjectValueType.DeltaTime));
            elapsedOut = AddValueOutput<float>("elapsed");

        }

        IEnumerator Execute(Flow flow)
        {
            ExecutionContext context = flow.Context;
            float time;
            float t = 0;
            float delta;
            var tickOut = GetFlowOutput("tick");
            isRunning = true;
            Status = FlowNodeStaus.Waiting;

            while (true)
            {
                DiryAllValueInputNodes(flow);
                ExecuteAllValueInputNode(context, this, flow);
                time = timeIn.GetValue(flow.Context);
                if (t >= time)
                    break;

                delta = (float)deltaIn.GetValue(flow.Context, deltaIn.ValueType);

                if (tickOut.LinkInput != null)
                {
                    elapsedOut.SetValue(t);
                    tickOut.Execute(flow);
                }
                t += delta;
                yield return null;
            }

            isRunning = false;
            if (Status == FlowNodeStaus.Waiting)
            {
                Status = FlowNodeStaus.Complete;
                elapsedOut.SetValue(t);
                var doneOut = GetFlowOutput("done");
                doneOut.Execute(flow);
            }
        }

        public override void Flow(Flow flow)
        {
            flow.Context.StartCoroutine(Execute(flow));
        }

    }
}