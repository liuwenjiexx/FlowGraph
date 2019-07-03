using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{
    [FlowGraphMembers]
    [Category(FlowNode.FunctionsCategory + "/Vector3")]
    internal static class Vector3Operator
    {


        [Name("Add")]
        [return: Name(FlowNode.Type_Vector3)]
        public static Vector3 Add(Vector3 a, Vector3 b)
        {
            return a + b;
        }
        [Name("Subtract")]
        [return: Name(FlowNode.Type_Vector3)]
        public static Vector3 Subtract(Vector3 a, Vector3 b)
        {
            return a - b;
        }

        [Name("Multiply")]
        [return: Name(FlowNode.Type_Vector3)]
        public static Vector3 Multiply(Vector3 a, Vector3 b)
        {
            return Vector3.Scale(a, b);
        }


        [Name("Multiply")]
        [return: Name(FlowNode.Type_Vector3)]
        public static Vector3 Multiply(Vector3 a, float b)
        {
            return a * b;
        }

        [Name("Divide")]
        [return: Name(FlowNode.Type_Vector3)]
        public static Vector3 Divide(Vector3 a, float b)
        {
            return a / b;
        }

        [Name("Negate")]
        [return: Name(FlowNode.Type_Vector3)]
        public static Vector3 Negate(Vector3 a)
        {
            return -a;
        }

        [Name("New")]
        [return: Name(FlowNode.Type_Vector3)]
        public static Vector3 New(float x, float y, float z)
        {
            return new Vector3(x, y, z);
        }


        [Name("Extract")]
        public static void Extract(Vector3 vector3, out float x, out float y, out float z)
        {
            x = vector3.x;
            y = vector3.y;
            z = vector3.z;
        }

        [Name("Lerp")]
        [return: Name(FlowNode.Type_Vector3)]
        public static Vector3 Lerp(Vector3 from, Vector3 to, float t)
        {
            return Vector3.LerpUnclamped(from, to, t);
        }
    }

}
