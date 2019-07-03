using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FlowGraph.Model
{

    public abstract class CoroutineNodeBase : FlowNode
    {

    }

    public abstract class CoroutineNode : CoroutineNodeBase
    {
        protected abstract IEnumerator Execute();
        public override void ExecuteContent(Flow flow)
        {
            flow.Context.StartCoroutine(Execute(), () =>
            {
                Status = FlowNodeStaus.Complete;
                Flow(flow);
            });
        }


    }


    public abstract class CoroutineNode<TInput> : CoroutineNodeBase
    {
        protected abstract IEnumerator Execute(TInput arg1);

        public override void ExecuteContent(Flow flow)
        {

        }
    }

}