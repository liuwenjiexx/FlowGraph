using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{

    [FlowGraph]
    [Category(FlowNode.FunctionsCategory + "/Operator/Float")]
    internal static class FloatOperator
    {


        [Name("Add")]
        [return: Name(FlowNode.Type_Float32)]
        public static float Add(float a, float b)
        {
            return a + b;
        }
        [Name("Subtract")]
        [return: Name(FlowNode.Type_Float32)]
        public static float Subtract(float a, float b)
        {
            return a - b;
        }
        [Name("Multiply")]
        [return: Name(FlowNode.Type_Float32)]
        public static float Multiply(float a, float b)
        {
            return a * b;
        }
        [Name("Divide")]
        [return: Name(FlowNode.Type_Float32)]
        public static float Divide(float a, float b)
        {
            return a / b;
        }
        [Name("Modulo")]
        [return: Name(FlowNode.Type_Float32)]
        public static float Modulo(float a, float b)
        {
            return a % b;
        }
        [Name("Negate")]
        [return: Name(FlowNode.Type_Float32)]
        public static float Negate(float a)
        {
            return -a;
        }
    }

    [FlowGraph]
    [Category(FlowNode.FunctionsCategory + "/Operator/Integer")]
    internal static class IntegerOperator
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

    [FlowGraph]
    [Category("Functions/Operator/Vector2")]
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
    }
    [FlowGraph]
    [Category(FlowNode.FunctionsCategory + "/Operator/Vector3")]
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
    }

    [FlowGraph]
    [Category(FlowNode.FunctionsCategory + "/Operator/String")]
    internal static class StringOperator
    {

        [Name("String Join")]
        [return: Name(FlowNode.Type_String)]
        public static string Join(string a, string b, string separator)
        {
            if (string.IsNullOrEmpty(separator))
                return a + b;
            return a + separator + b;
        }

        [Name("String Join")]
        [return: Name(FlowNode.Type_String)]
        public static string Join(object a, object b, string separator)
        {
            return Join(a == null ? string.Empty : a.ToString(), b == null ? string.Empty : b.ToString(), separator);
        }
    }
}