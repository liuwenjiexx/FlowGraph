using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{ 
  

    [FlowGraphMembers]
    [Category(FlowNode.FunctionsCategory + "/Quaternion")]
    internal static class QuaternionOperator
    {

        [Name("Lerp")]
        public static Quaternion Lerp(Quaternion from, Quaternion to, float t)
        {
            return Quaternion.LerpUnclamped(from, to, t);
        }
    }


}