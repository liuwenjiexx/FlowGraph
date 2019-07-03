using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FlowGraph.Model
{

    [FlowGraphMembers]
    [Category(FlowNode.FunctionsCategory + "/Int32")]
    internal static class Int32Operator
    {
        [Name("Add")]
        [return: Name(FlowNode.Type_Int32)]
        public static int Add(int a, int b)
        {
            return a + b;
        }
        [Name("Subtract")]
        [return: Name(FlowNode.Type_Int32)]
        public static int Subtract(int a, int b)
        {
            return a - b;
        }
        [Name("Multiply")]
        [return: Name(FlowNode.Type_Int32)]
        public static int Multiply(int a, int b)
        {
            return a * b;
        }
        [Name("Divide")]
        [return: Name(FlowNode.Type_Int32)]
        public static int Divide(int a, int b)
        {
            return a / b;
        }
        [Name("Modulo")]
        [return: Name(FlowNode.Type_Int32)]
        public static int Modulo(int a, int b)
        {
            return a % b;
        }
        [Name("Negate")]
        [return: Name(FlowNode.Type_Int32)]
        public static int Negate(int a)
        {
            return -a;
        }
    }


}