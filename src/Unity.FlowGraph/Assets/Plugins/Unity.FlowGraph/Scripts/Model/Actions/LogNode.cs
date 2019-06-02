using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{

    public class LogNode : ActionNode<object>
    {
        public override void Execute(object value)
        {
            Debug.Log(value);
        }

    }
}