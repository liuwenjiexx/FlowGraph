//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace FlowGraph.Model
//{
  
//    [Category(FunctionsCategory + "/Lerps")]
//    public abstract class LerpNode<TValue> : FunctionNode<TValue, TValue, TValue, float>
//    {
//    }

//    [Hidden]
//    public class Vector2Lerp : LerpNode<Vector2>
//    {
//        public override Vector2 Execute(Vector2 from, Vector2 to, float t)
//        {
//            return Vector2.LerpUnclamped(from, to, t);
//        }
//    }

//    [Hidden]
//    public class Vector3Lerp : LerpNode<Vector3>
//    {
//        public override Vector3 Execute(Vector3 from, Vector3 to, float t)
//        {
//            return Vector3.LerpUnclamped(from, to, t);
//        }
//    }

//    [Hidden]
//    public class QuaternionLerp : LerpNode<Quaternion>
//    {
//        public override Quaternion Execute(Quaternion from, Quaternion to, float t)
//        {
//            return Quaternion.LerpUnclamped(from, to, t);
//        }
//    }

//    [Hidden]
//    public class ColorLerp : LerpNode<Color>
//    {
//        public override Color Execute(Color from, Color to, float t)
//        {
//            return Color.Lerp(from, to, t);
//        }
//    }


//}
