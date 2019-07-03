using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FlowGraph.Model
{

    [Category("Actions")]
    public abstract class ActionNodeBase : FlowNode
    {

        public ActionNodeBase()
        {
            RegisterNodeMethodPorts("Execute");
        }
    }

    public abstract class ActionNode : ActionNodeBase
    {

        public abstract void Execute();

        public override void ExecuteContent(Flow flow)
        {
            Execute();
        }


    }

    public abstract class ActionNode<TInput> : ActionNodeBase
    {
        
        public abstract void Execute(TInput arg1);



        public override void ExecuteContent(Flow flow)
        {
            var arg1 = GetValueInput<TInput>(0).GetValue(flow.Context);
            Execute(arg1);
        }
    }

}