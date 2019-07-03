using UnityEngine;

namespace FlowGraph.Model
{
    [HiddenMenu]
    public class StateEventNode : OnEventNode
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



    [Name("State Event")]
    [Category(EventsCategory)]
    public class StateEvent : StateEventNode
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

    [HiddenMenu]
    public class StateEvent<TArg1> : StateEventNode
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

    [Name("State Event(Arg1)")]
    [Category(EventsCategory)]
    public class StateEventArg1 : StateEvent<object>
    {

    }


}