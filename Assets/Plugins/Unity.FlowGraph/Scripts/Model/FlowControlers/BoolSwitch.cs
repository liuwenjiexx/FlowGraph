using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{
    [Name("Switch")]
    [Category("Flow Controllers")]
    public class BoolSwitch : FlowControlerNode
    {
        private ValuePort<bool> condition;
        private FlowOutput trueOut;
        private FlowOutput falseOut;

        protected override void RegisterPorts()
        {
            condition = AddValueInput<bool>("Condition");
            trueOut = AddFlowOutput("True");
            falseOut = AddFlowOutput("False");

            AddFlowInput(FLOW_IN);
        }

        public override void Flow(Flow flow)
        {

            if (condition.GetValue(flow.Context))
            {
                trueOut.Execute(flow);
            }
            else
            {
                falseOut.Execute(flow);
            }
        }

    }

}