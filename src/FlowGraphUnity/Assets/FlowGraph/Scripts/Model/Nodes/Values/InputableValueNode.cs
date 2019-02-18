using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FlowGraph.Model
{

    [Category("Values")]
    public abstract class InputableValueNode<TValue> : ValueNode<TValue>
    {

        [SerializeField]
        private TValue value;

        public TValue Value
        {
            get { return value; }
            set { this.value = value; }
        }

        protected override TValue GetValue(Flow flow)
        {
            return value;
        }

        public override string GetDisplayName()
        {
            if (value == null)
                return "<null>";
            return value.ToString();
        }
        public override string GetDisplayNameDesc()
        {
            return GetDisplayValueTypeName(typeof(TValue)) + " Value";
        }
    }

    [Name("Int")]
    public class Int32Value : InputableValueNode<int>
    {
        protected override string OutputName
        {
            get { return Type_Int32; }
        }
    }

    [Name("Float")]
    public class Float32Value : InputableValueNode<float>
    {
        protected override string OutputName
        {
            get { return Type_Float32; }
        }
    }

    [Name("Bool")]
    public class BooleanValue : InputableValueNode<bool>
    {
        protected override string OutputName
        {
            get { return Type_Bool; }
        }
    }

    [Name("String")]
    public class StringValue : InputableValueNode<string>
    {
        protected override string OutputName
        {
            get { return Type_String; }
        }
    }

    [Name("Object")]
    public class UnityObjectValue : InputableValueNode<UnityEngine.Object>
    {
        protected override string OutputName
        {
            get { return Type_UnityObject; }
        }
    }

    [Name("Vector2")]
    public class Vector2Value : InputableValueNode<Vector2>
    {
        protected override string OutputName
        {
            get { return Type_Vector2; }
        }
    }
    [Name("Vector2Int")]
    public class Vector2IntValue : InputableValueNode<Vector2Int>
    {
        protected override string OutputName
        {
            get { return Type_Vector2; }
        }
    }
    [Name("Vector3")]
    public class Vector3Value : InputableValueNode<Vector3>
    {
        protected override string OutputName
        {
            get { return Type_Vector3; }
        }
    }
    [Name("Vector4")]
    public class Vector4Value : InputableValueNode<Vector4>
    {
        protected override string OutputName
        {
            get { return Type_Vector4; }
        }
    }
    [Name("Color")]
    public class ColorValue : InputableValueNode<Color>
    {
        protected override string OutputName
        {
            get { return Type_Color; }
        }
    }

    [Name("Color32")]
    public class Color32Value : InputableValueNode<Color32>
    {
        protected override string OutputName
        {
            get { return Type_Color32; }
        }
    }

    [Name("Curve")]
    public class AnimationCurveValue : InputableValueNode<AnimationCurve>
    {
        protected override string OutputName
        {
            get { return Type_AnimationCurve; }
        }
    }

    [Name("GameObject")]
    public class GameObjectValue : InputableValueNode<GameObject>
    {
        protected override string OutputName
        {
            get { return Type_GameObject; }
        }
        public override string GetDisplayName()
        {
            if (!Value)
                return string.Format("missing({0})", typeof(GameObject).Name);

            return string.Format("{0}({1})", Value.name, typeof(GameObject).Name);
        }
    }

    [Name("Component")]
    public class ComponentValue : InputableValueNode<Component>
    {
        protected override string OutputName
        {
            get { return Type_Component; }
        }

        public override string GetDisplayName()
        {
            if (!Value)
                return string.Format("missing({0})", typeof(Component).Name);
            return string.Format("{0}({1})", Value.name, Value.GetType().Name);
        }

    }

    //[Name("Transform")]
    //public class TransformValue : InputableValueNode<Transform>
    //{
    //    protected override string OutputName
    //    {
    //        get { return Type_Transform; }
    //    }
    //    public override string GetDisplayName()
    //    {
    //        if (!Value)
    //            return string.Format("missing({0})", typeof(Transform).Name);
    //        return string.Format("{0}({1})", Value.name, typeof(Transform).Name);
    //    }
    //}

}