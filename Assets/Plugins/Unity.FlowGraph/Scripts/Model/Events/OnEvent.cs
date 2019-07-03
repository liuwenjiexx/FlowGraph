using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FlowGraph.Model
{
    public abstract class OnEventBase : OnEventNode
    {
        private FlowOutput flowOut;


        protected FlowOutput FlowOut
        {
            get { return flowOut; }
        }

        protected override void RegisterPorts()
        {
            flowOut = AddFlowOutput(FLOW_OUT);
        }
    }

    [Name("OnEvent")]
    public class OnEvent : OnEventBase
    {

        public void TriggerEvent(ExecutionContext context)
        {
            FlowOut.Execute(context);
        }
    }

    [HiddenMenu]
    [Name("OnEvent(Arg1)")]
    public class OnEvent<TArg1> : OnEventBase
    {
        private ValuePort<TArg1> arg1Out;

        protected override void RegisterPorts()
        {
            base.RegisterPorts();
            arg1Out = AddValueOutput<TArg1>(ARGUMENT1);
        }

        public void TriggerEvent(ExecutionContext context, TArg1 arg1)
        {
            arg1Out.SetValue(arg1);
            FlowOut.Execute(context);
        }
    }


    [Name("OnEvent(arg1)")]
    public class OnEventArg1 : OnEvent<object>
    {

    }
}
