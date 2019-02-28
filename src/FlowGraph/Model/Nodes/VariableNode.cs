using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{


    [Hidden]
    [Category("Variables")]
    public abstract class VariableNode<TValue> : FlowNode
    {
        [SerializeField]
        private string name;

        public string Name
        {
            get { return name; }
        }

        public VariableNode()
        {
        }

        public VariableNode(string name)
        {
            this.name = name;
        }

    }


    [Category("Variables/Get")]
    public abstract class GetVariableNode<TValue> : VariableNode<TValue>
    {
        public GetVariableNode()
        {
        }
        public GetVariableNode(string name)
            : base(name)
        {
        }

        protected virtual string OutputName
        {
            get { return GetValueTypeName(typeof(TValue)); }
        }


        protected override void RegisterPorts()
        {
            AddValueOutput<TValue>(OutputName);
        }

        public override void ExecuteContent(Flow flow)
        {
            object value = flow.Context.GetVariable(Name);
            ValueOutputs[0].SetValue(value);
        }

        public override string GetDisplayName()
        {
            return "$" + Name;
        }
        public override string GetDisplayNameDesc()
        {
            return "Get Variable(" + GetDisplayValueTypeName(typeof(TValue)) + ")";
        }
    }

    [Category("Variables/Set")]
    public abstract class SetVariableNode<TValue> : VariableNode<TValue>
    {

        public SetVariableNode()
        {
        }

        public SetVariableNode(string name)
            : base(name)
        {
        }

        protected override void RegisterPorts()
        {
            AddFlowInput(FLOW_IN);
            AddFlowOutput(FLOW_OUT);
            AddValueInput<TValue>(GetValueTypeName(typeof(TValue)));

        }
        public override void ExecuteContent(Flow flow)
        {
            object value = ValueInputs[0].GetValue(flow.Context);
            flow.Context.SetVariable(Name, value);
        }

        public override string GetDisplayName()
        {
            return "$" + Name;
        }
        public override string GetDisplayNameDesc()
        {
            return "Set Variable(" + GetDisplayValueTypeName(typeof(TValue)) + ")";
        }
    }



    [Name("Int")]
    public class Int32GetVariable : GetVariableNode<int>
    {
        public Int32GetVariable() { }

        public Int32GetVariable(string name)
            : base(name)
        {
        }

        protected override string OutputName
        {
            get { return Type_Int32; }
        }

    }


    [Hidden]
    [Name("Int")]
    public class Int32SetVariable : SetVariableNode<int>
    {
        public Int32SetVariable() { }

        public Int32SetVariable(string name)
            : base(name)
        {
        }
    }


    [Hidden]
    [Name("Float")]
    public class Float32GetVariable : GetVariableNode<float>
    {
        public Float32GetVariable() { }

        public Float32GetVariable(string name)
            : base(name)
        {
        }
    }


    [Hidden]
    [Name("Float")]
    public class Float32SetVariable : SetVariableNode<float>
    {
        public Float32SetVariable() { }

        public Float32SetVariable(string name)
            : base(name)
        {
        }
    }


    [Hidden]
    [Name("Bool")]
    public class BooleanGetVariable : GetVariableNode<bool>
    {
        public BooleanGetVariable() { }

        public BooleanGetVariable(string name)
            : base(name)
        {
        }
    }

    [Hidden]
    [Name("Bool")]
    public class BooleanSetVariable : SetVariableNode<bool>
    {
        public BooleanSetVariable() { }

        public BooleanSetVariable(string name)
            : base(name)
        {
        }
    }


    [Hidden]
    [Name("String")]
    public class StringGetVariable : GetVariableNode<string>
    {
        public StringGetVariable() { }

        public StringGetVariable(string name)
            : base(name)
        {
        }
    }
    [Hidden]
    public class StringSetVariable : SetVariableNode<string>
    {
        public StringSetVariable() { }

        public StringSetVariable(string name)
            : base(name)
        {
        }
    }


    [Name("object")]
    public class ObjectGetVariable : GetVariableNode<object>
    {
        public ObjectGetVariable() { }

        public ObjectGetVariable(string name)
            : base(name)
        {
        }
    }

    [Hidden]
    [Name("object")]
    public class ObjectSetVariable : SetVariableNode<object>
    {
        public ObjectSetVariable() { }

        public ObjectSetVariable(string name)
            : base(name)
        {
        }
    }

    [Hidden]
    [Name("Object")]
    public class UnityObjectGetVariable : GetVariableNode<UnityEngine.Object>
    {
        public UnityObjectGetVariable() { }

        public UnityObjectGetVariable(string name)
            : base(name)
        {
        }
    }

    [Hidden]
    [Name("Object")]
    public class UnityObjectSetVariable : SetVariableNode<UnityEngine.Object>
    {
        public UnityObjectSetVariable() { }

        public UnityObjectSetVariable(string name)
            : base(name)
        {
        }
    }



    [Hidden]
    [Name("Vector2")]
    public class Vector2GetVariable : GetVariableNode<Vector2>
    {
        public Vector2GetVariable() { }

        public Vector2GetVariable(string name)
            : base(name)
        {
        }
    }

    [Hidden]
    [Name("Vector2")]
    public class Vector2SetVariable : SetVariableNode<Vector2>
    {
        public Vector2SetVariable() { }

        public Vector2SetVariable(string name)
            : base(name)
        {
        }
    }

    [Hidden]
    [Name("Vector2Int")]
    public class Vector2IntGetVariable : GetVariableNode<Vector2Int>
    {
        public Vector2IntGetVariable() { }

        public Vector2IntGetVariable(string name)
            : base(name)
        {
        }
    }
    [Hidden]
    [Name("Vector2Int")]
    public class Vector2SetIntVariable : SetVariableNode<Vector2Int>
    {
        public Vector2SetIntVariable() { }

        public Vector2SetIntVariable(string name)
            : base(name)
        {
        }
    }


    [Hidden]
    [Name("Vector3")]
    public class Vector3GetVariable : GetVariableNode<Vector3>
    {
        public Vector3GetVariable() { }

        public Vector3GetVariable(string name)
            : base(name)
        {
        }
    }
    [Hidden]
    [Name("Vector3")]
    public class Vector3SetVariable : SetVariableNode<Vector3>
    {
        public Vector3SetVariable() { }

        public Vector3SetVariable(string name)
            : base(name)
        {
        }
    }

    [Hidden]
    [Name("Vector3Int")]
    public class Vector3IntGetVariable : GetVariableNode<Vector3Int>
    {
        public Vector3IntGetVariable() { }

        public Vector3IntGetVariable(string name)
            : base(name)
        {
        }
    }
    [Hidden]
    [Name("Vector3Int")]
    public class Vector3IntSetVariable : SetVariableNode<Vector3Int>
    {
        public Vector3IntSetVariable() { }

        public Vector3IntSetVariable(string name)
            : base(name)
        {
        }
    }


    [Hidden]
    [Name("Vector4")]
    public class Vector4GetVariable : GetVariableNode<Vector4>
    {
        public Vector4GetVariable() { }

        public Vector4GetVariable(string name)
            : base(name)
        {
        }
    }
    [Hidden]
    [Name("Vector4")]
    public class Vector4SetVariable : SetVariableNode<Vector4>
    {
        public Vector4SetVariable() { }

        public Vector4SetVariable(string name)
            : base(name)
        {
        }
    }


    [Hidden]
    [Name("Color")]
    public class ColorGetVariable : GetVariableNode<Color>
    {
        public ColorGetVariable() { }

        public ColorGetVariable(string name)
            : base(name)
        {
        }
    }
    [Hidden]
    [Name("Color")]
    public class ColorSetVariable : SetVariableNode<Color>
    {
        public ColorSetVariable() { }

        public ColorSetVariable(string name)
            : base(name)
        {
        }
    }


    [Hidden]
    [Name("Color32")]
    public class Color32GetVariable : GetVariableNode<Color32>
    {
        public Color32GetVariable() { }

        public Color32GetVariable(string name)
            : base(name)
        {
        }
    }
    [Hidden]
    [Name("Color32")]
    public class Color32SetVariable : SetVariableNode<Color32>
    {
        public Color32SetVariable() { }

        public Color32SetVariable(string name)
            : base(name)
        {
        }
    }

    [Hidden]
    [Name("Curve")]
    public class AnimationCurveGetVariable : GetVariableNode<AnimationCurve>
    {
        public AnimationCurveGetVariable() { }

        public AnimationCurveGetVariable(string name)
            : base(name)
        {
        }
    }

    [Hidden]
    [Name("Curve")]
    public class AnimationCurveSetVariable : SetVariableNode<AnimationCurve>
    {
        public AnimationCurveSetVariable() { }

        public AnimationCurveSetVariable(string name)
            : base(name)
        {
        }
    }

}