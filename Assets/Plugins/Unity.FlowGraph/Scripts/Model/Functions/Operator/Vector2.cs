using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{
    [FlowGraphMembers]
    [Category("Functions/Vector2")]
    internal static class Vector2Operator
    {
        [Name("Add")]
        [return: Name(FlowNode.Type_Vector2)]
        public static Vector2 Add(Vector2 a, Vector2 b)
        {
            return a + b;
        }
        [Name("Subtract")]
        [return: Name(FlowNode.Type_Vector2)]
        public static Vector2 Subtract(Vector2 a, Vector2 b)
        {
            return a - b;
        }

        [Name("Multiply")]
        [return: Name(FlowNode.Type_Vector2)]
        public static Vector2 Multiply(Vector2 a, Vector2 b)
        {
            return a * b;
        }

        [Name("Multiply")]
        [return: Name(FlowNode.Type_Vector2)]
        public static Vector2 Multiply(Vector2 a, float b)
        {
            return a * b;
        }

        [Name("Divide")]
        [return: Name(FlowNode.Type_Vector2)]
        public static Vector2 Divide(Vector2 a, float b)
        {
            return a / b;
        }
        [Name("Negate")]
        [return: Name(FlowNode.Type_Vector2)]
        public static Vector2 Negate(Vector2 a)
        {
            return -a;
        }
        [Name("Lerp")]
        [return: Name(FlowNode.Type_Vector2)]
        public static Vector2 Lerp(Vector2 from, Vector2 to, float t)
        {
            return Vector2.LerpUnclamped(from, to, t);
        }
    }

}
