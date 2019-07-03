using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

namespace FlowGraph.Model
{
    public abstract class FieldNode : MemberNode
    {
        private ValuePort thisIn;

        public FieldNode()
        {
        }

        public FieldNode(FieldInfo field)
            : base(field)
        {
        }

        public FieldInfo Field
        {
            get { return Member as FieldInfo; }
        }

        public ValuePort ThisIn
        {
            get { return thisIn; }
        }

        protected bool IsStatic
        {
            get
            {
                FieldInfo field = Field;
                if (field != null)
                    return field.IsStatic;
                return false;
            }
        }

        protected override void RegisterPorts()
        {

            base.RegisterPorts();
            FieldInfo field = Field;
            if (field != null)
            {
                if (!field.IsStatic)
                {
                    thisIn = AddValueInput(THIS, field.DeclaringType, GetDefaultInject(field.DeclaringType));
                }
            }
        }

     
    }

    [HiddenMenu]
    [Name("Get Field")]
    [Category(ActionsCategory)]
    public class GetField : FieldNode
    {
        private ValuePort valueOut;

        public GetField()
        {
        }

        public GetField(FieldInfo field)
            : base(field)
        {
        }

        protected override void RegisterPorts()
        {
            base.RegisterPorts();
            FieldInfo field = Field;
            if (field != null)
            {
                valueOut = AddValueOutput(VALUE, field.FieldType);
            }
        }

        public override void ExecuteContent(Flow flow)
        {
            object value;
            object instance = null;
            FieldInfo field = Field;
            if (!field.IsStatic)
            {
                instance = ThisIn.GetValue(flow.Context);
                if (instance == null)
                    throw new Exception("get field error, instance null. field: "+ field);
            }

            value = field.GetValue(instance);

            valueOut.SetValue(value);
        }
         

    }

    [HiddenMenu]
    [Name("Set Field")]
    [Category(ActionsCategory)]
    public class SetField : FieldNode
    {
        private ValuePort valueIn;

        public SetField()
        {
        }

        public SetField(FieldInfo field)
            : base(field)
        {
        }

        protected override void RegisterPorts()
        {
            base.RegisterPorts();
            FieldInfo field = Field;
            if (field != null)
            {
                valueIn = AddValueInput(GetValueTypeName(field.FieldType), field.FieldType);
            }
        }

        public override void ExecuteContent(Flow flow)
        {
            object value = valueIn.GetValue(flow.Context);
            object instance = null;
            FieldInfo field = Field;
            if (!field.IsStatic)
            {
                instance = ThisIn.GetValue(flow.Context);
                if (instance == null)
                    throw new Exception("set field error, instance null ,field: "+field);
            }

            field.SetValue(instance, value);
        }
    
    }
}