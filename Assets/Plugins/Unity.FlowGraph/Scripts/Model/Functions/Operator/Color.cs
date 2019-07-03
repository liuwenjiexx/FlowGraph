using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FlowGraph.Model
{
    [FlowGraphMembers]
    [Category(FlowNode.FunctionsCategory + "/Color")]
    internal static class ColorOperator
    {
        [Name("Lerp")]
        [return: Name(FlowNode.Type_Color)]
        public static Color ColorLerp(Color from, Color to, float t)
        {
            return Color.LerpUnclamped(from, to, t);
        }

    }
}
