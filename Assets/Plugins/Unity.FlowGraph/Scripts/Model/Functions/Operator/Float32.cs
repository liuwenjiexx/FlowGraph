using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FlowGraph.Model
{

    [FlowGraphMembers]
    [Category(FlowNode.FunctionsCategory + "/Float32")]
    internal static class Float32Operator
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

        [Name("Clamp01")]
        [return: Name(FlowNode.Type_Float32)]
        public static float Clamp01(float value)
        {
            if (value < 0f)
                return 0;
            if (value > 1f)
                return 1f;
            return value;
        }

        [Name("Clamp")]
        [return: Name(FlowNode.Type_Float32)]
        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }


        [Name("Lerp")]
        [return: Name(FlowNode.VALUE)]
        public static float Lerp(float from, float to, float t)
        {
            return (to - from) * t;
        }

        [Name("InverseLerp")]
        [return: Name("t")]
        public static float InverseLerp(float from, float to, float value)
        {
            //if (value < a)
            //    return 0f;
            //if (value > b)
            //    return 1f;
            //float delta = Mathf.Abs(b - a);
            //if (delta <= Mathf.Epsilon)
            //    return 1f;
            //return Mathf.Abs(value - a) / delta;
            return Mathf.InverseLerp(from, to, value);
        }


        [Name("Interpolator")]
        [return: Name("t")]
        public static float Interpolator(float value, [DefaultValue(0f)] float from, [DefaultValue(1f)] float to, AnimationCurve curve)
        {
            float t = Mathf.InverseLerp(from, to, value);
            return curve.Evaluate(t);
        }
        [Name("Curve")]
        [return: Name("t")]
        public static float Curve(float t, AnimationCurve curve)
        {
            return curve.Evaluate(t);
        }
    }


}
