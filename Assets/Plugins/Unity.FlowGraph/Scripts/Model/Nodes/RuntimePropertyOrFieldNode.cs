using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

namespace FlowGraph.Model
{
    [Name("Get Property")]
    [Category(ActionsCategory)]
    public class RuntimeGetPropertyOrFieldNode : FlowNode
    {
        [SerializeField]
        private string property;
        private ValuePort<object> thisIn;
        private ValuePort<object> valueOut;
        private MemberInfo member;

        protected override void RegisterPorts()
        {
            base.RegisterPorts();
            thisIn = AddValueInput<object>(THIS);
            valueOut = AddValueOutput<object>(VALUE);

        }

        public override void ExecuteContent(Flow flow)
        {

            object instance = thisIn.GetValue(flow.Context);


            if (member == null)
            {
                if (instance == null)
                {
                    Debug.LogError("Set Instance null");
                    return;
                }

                Type instanceType = instance.GetType();
                member = instanceType.GetField(property);
                if (member == null)
                {
                    member = instanceType.GetProperty(property);
                }
                if (member == null)
                {
                    Debug.LogError("Member null " + property);
                }
            }

            if (member == null)
                return;

            object value;
            if (member.MemberType == MemberTypes.Property)
            {
                PropertyInfo pInfo = (PropertyInfo)member;
                value = pInfo.GetSetMethod().Invoke(instance, null);
            }
            else
            {
                FieldInfo fInfo = (FieldInfo)member;
                value = fInfo.GetValue(instance);
            }

            valueOut.SetValue(value);

        } 
    }



    [Name("Set Property")]
    [Category(ActionsCategory)]
    public class RuntimeSetPropertyOrFieldNode : FlowNode
    {
        [SerializeField]
        private string property;
        private ValuePort<object> thisIn;
        private ValuePort<object> valueIn;
        private MemberInfo member;

        protected override void RegisterPorts()
        {
            base.RegisterPorts();
            thisIn = AddValueInput<object>(THIS);
            valueIn = AddValueInput<object>(VALUE);
        }

        public override void ExecuteContent(Flow flow)
        {

            object instance = thisIn.GetValue(flow.Context);
            object value = valueIn.GetValue(flow.Context);

            if (member == null)
            {
                if (instance == null)
                {
                    Debug.LogError("Set Instance null");
                    return;
                }

                Type instanceType = instance.GetType();
                member = instanceType.GetField(property);
                if (member == null)
                {
                    member = instanceType.GetProperty(property);
                }
                if (member == null)
                {
                    Debug.LogError("Member null " + property);
                }
            }

            if (member == null)
                return;

            if (member.MemberType == MemberTypes.Property)
            {
                PropertyInfo pInfo = (PropertyInfo)member;
                pInfo.GetSetMethod().Invoke(instance, new object[] { value });
            }
            else
            {
                FieldInfo fInfo = (FieldInfo)member;
                fInfo.SetValue(instance, value);
            }
        }


    }
}