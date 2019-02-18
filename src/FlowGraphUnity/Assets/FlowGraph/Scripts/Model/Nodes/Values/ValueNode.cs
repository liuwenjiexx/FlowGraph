using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{


    [Name("Delta Time")]
    [Category("Time")]
    public class DeltaTimeValue : ValueNode<float>
    {
        protected override string OutputName
        {
            get { return Type_Float32; }
        }
        protected override float GetValue(Flow flow)
        {
            return Time.deltaTime;
        }
    }

    [Name("Unscaled Delta Time")]
    [Category("Time")]
    public class UnscaledDeltaTimeValue : ValueNode<float>
    {
        protected override string OutputName
        {
            get { return Type_Float32; }
        }
        protected override float GetValue(Flow flow)
        {
            return Time.unscaledDeltaTime;
        }
    }

  
}