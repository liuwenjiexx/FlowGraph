using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{

    [FlowGraph]
    [Category(FlowNode.FunctionsCategory + "/Comparer")]
    internal class Comparer
    {
        [Name("==")]
        [return: Name(FlowNode.Type_Bool)]
        public static bool Equal(object a, object b)
        {
            return object.Equals(a, b);
        }

        [Name("!=")]
        [return: Name(FlowNode.Type_Bool)]
        public static bool NotEqual(object a, object b)
        {
            return !object.Equals(a, b);
        }
        [Name(">")]
        [return: Name(FlowNode.Type_Bool)]
        public static bool GreaterThan(float a, float b)
        {
            return a > b;
        }
        [Name(">=")]
        [return: Name(FlowNode.Type_Bool)]
        public static bool GreaterThanOrEqual(float a, float b)
        {
            return a >= b;
        }
        [Name("<")]
        [return: Name(FlowNode.Type_Bool)]
        public static bool LessThan(float a, float b)
        {
            return a < b;
        }
        [Name("<=")]
        [return: Name(FlowNode.Type_Bool)]
        public static bool LessThanOrEqual(float a, float b)
        {
            return a <= b;
        }
    }
}