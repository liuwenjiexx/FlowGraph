using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{

    public abstract class ValuePort
    {
        private string name;
        private FlowNode node;
        private object value;
        private ValuePort linkOutput;
        private Type valueType;
        private Inject inject;

        public ValuePort(FlowNode node, string name, Type type)
            : this(node, name, type, null)
        {
        }

        public ValuePort(FlowNode node, string name, Type type, Inject inject)
        {
            this.node = node;
            this.name = name;
            this.valueType = type;
            this.inject = inject;
        }

        public string Name
        {
            get { return name; }
        }

        public Type ValueType
        {
            get { return valueType; }
        }

        public Inject Inject
        {
            get { return inject; }
        }


        public ValuePort LinkOutput
        {
            get { return linkOutput; }
            set { linkOutput = value; }
        }

        public FlowNode Node
        {
            get { return node; }
        }

        public ValuePortFlags Flags { get; set; }

        public object GetValue(ExecutionContext executionContext)
        {
            return GetValue(executionContext, valueType);
        }
        public object GetValue(ExecutionContext executionContext, Type targetType)
        {
            object value;
            if (linkOutput != null)
            {
                value = linkOutput.GetValue(executionContext, targetType);
            }
            else
            {
                if (inject != null)
                {
                    value = executionContext.GetInjectValue(inject);
                }
                else
                {
                    value = this.value;
                }
            }
            if (targetType == null)
                targetType = valueType;
            value = FlowNode.ChangeType(value, targetType);
            return value;
        }

        public void SetValue(object value)
        {
            this.value = FlowNode.ChangeType(value, valueType);
        }

        public override string ToString()
        {
            return string.Format("ValuePort: {0}, {1}, {2}", Name, ValueType.Name, this.value);
        }
    }


    public enum ValuePortFlags
    {
        None,
        Hidden = 0x1,
        This = 0x2,
    }


    public class ValuePort<T> : ValuePort
    {
        public ValuePort(FlowNode node, string name)
              : base(node, name, typeof(T))
        {
        }

        public ValuePort(FlowNode node, string name, Inject inject)
           : base(node, name, typeof(T), inject)
        {
        }

        public new T GetValue(ExecutionContext context)
        {
            return (T)base.GetValue(context);
        }

        //public void SetValue(T value)
        //{
        //    base.SetValue(value);
        //}
    }


}


