//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace FlowGraph.Model
//{

//    public class TimeInterpolatorNode : FunctionNode<float, float, float, AnimationCurve, float>
//    {
//        public override float Execute(float from, float to, AnimationCurve curve, float time)
//        {
//            float t = Mathf.InverseLerp(from, to, time);
//            return curve.Evaluate(t);
//        }
//    }
//}