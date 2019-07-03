using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

namespace FlowGraph.Model
{
    public class PropertyNode : MemberNode
    {
        private ValuePort thisIn;

        public PropertyNode()
        {
        }

        public PropertyNode(PropertyInfo property)
            : base(property)
        {
        }

        public PropertyInfo Property
        {
            get { return Member as PropertyInfo; }
        }

        public ValuePort ThisIn
        {
            get { return thisIn; }
        }

        protected bool IsStatic
        {
            get
            {
                PropertyInfo property = Property;
                if (property != null)
                {
                    var getter = property.GetGetMethod();
                    return getter.IsStatic;
                }
                return false;
            }
        }
         
        protected override void RegisterPorts()
        {
            base.RegisterPorts();
            PropertyInfo property = Property;
            if (property != null)
            {
                if (!IsStatic)
                {
                    thisIn = AddValueInput(THIS, property.DeclaringType, GetDefaultInject(property.DeclaringType));
                }
            }
        }

    }

    [HiddenMenu]
    [Name("Get Property")]
    [Category(ActionsCategory)]
    public class GetProperty : PropertyNode
    {
        private ValuePort valueOut;

        public GetProperty()
        {
        }

        public GetProperty(PropertyInfo property)
            : base(property)
        {
        }
         
        protected override void RegisterPorts()
        {
            base.RegisterPorts();
            PropertyInfo property = Property;
            if (property != null)
            {
                var getter = property.GetGetMethod();
                valueOut = AddValueOutput(GetValueTypeName(property.PropertyType), property.PropertyType);
            }
        }

        public override void ExecuteContent(Flow flow)
        {
            object value;
            object instance = null;
            PropertyInfo property = Property;
            var getter = property.GetGetMethod();
            if (!IsStatic)
            {
                instance = ThisIn.GetValue(flow.Context);
                if (instance == null)
                    throw new Exception("instance null");
            }

            value = getter.Invoke(instance, null);

            valueOut.SetValue(value);
        }


    }

    [HiddenMenu]
    [Name("Set Property")]
    [Category(ActionsCategory)]
    public class SetProperty : PropertyNode
    {

        private ValuePort valueIn;

        public SetProperty()
        {
        }

        public SetProperty(PropertyInfo property)
            : base(property)
        {
        } 

        protected override void RegisterPorts()
        {
            base.RegisterPorts();
            PropertyInfo property = Property;
            if (property != null)
            {
                valueIn = AddValueInput(GetValueTypeName(property.PropertyType), property.PropertyType);
            }
        }

        public override void ExecuteContent(Flow flow)
        {
            object value = valueIn.GetValue(flow.Context);
            object instance = null;
            PropertyInfo property = Property;
            var setter = property.GetSetMethod();
            if (!setter.IsStatic)
            {
                instance = ThisIn.GetValue(flow.Context);
                if (instance == null)
                    throw new Exception("instance null");
            }

            setter.Invoke(instance, new object[] { value });
        }

       
    }
}