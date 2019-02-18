using UnityEngine;

namespace FlowGraph.Model
{

    [Name("Enterable Event")]
    public class EnterableEventNode : OnEventNode
    {
        private FlowOutput enterFlowOut;
        private FlowOutput exitFlowOut;

        protected FlowOutput EnterFlowOut
        {
            get { return enterFlowOut; }
        }

        protected FlowOutput ExitFlowOut
        {
            get { return exitFlowOut; }
        }


        protected override void RegisterPorts()
        {
            enterFlowOut = AddFlowOutput("Enter");
            exitFlowOut = AddFlowOutput("Exit");
        }

    }



    [Name("Enterable Event")]
    public class EnterableEvent : EnterableEventNode
    {

        public void OnEnter(ExecutionContext context)
        {
            EnterFlowOut.Execute(context);
        }

        public void OnExit(ExecutionContext context)
        {
            ExitFlowOut.Execute(context);
        }
    }

    [Hidden]
    public class EnterableEvent<TArg1> : EnterableEventNode
    {
        private ValuePort<TArg1> arg1Out;

        protected override void RegisterPorts()
        {
            base.RegisterPorts();
            arg1Out = AddValueOutput<TArg1>(ARGUMENT1);
        }

        public void OnEnter(ExecutionContext context, TArg1 arg1)
        {
            arg1Out.SetValue(arg1);
            EnterFlowOut.Execute(context);
        }

        public void OnExit(ExecutionContext context, TArg1 arg1)
        {
            arg1Out.SetValue(arg1);
            ExitFlowOut.Execute(context);
        }

    }

    [Name("Enterable Event(Arg1)")]
    public class EnterableEventArg1 : EnterableEvent<object>
    {

    }


}