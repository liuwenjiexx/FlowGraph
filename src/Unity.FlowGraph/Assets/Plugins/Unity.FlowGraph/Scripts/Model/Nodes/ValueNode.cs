using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{

    public abstract class ValueNode<TValue> : FlowNode
    {

        protected virtual string OutputName
        {
            get { return GetValueTypeName(typeof(TValue)); }
        }

        protected override void RegisterPorts()
        {
            AddValueOutput<TValue>(OutputName);
        }

        public override void ExecuteContent(Flow flow)
        {
            ValueOutputs[0].SetValue(GetValue(flow));
        }

        protected abstract TValue GetValue(Flow flow);

    }
}