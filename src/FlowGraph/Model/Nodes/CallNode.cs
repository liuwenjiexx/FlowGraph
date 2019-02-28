using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace FlowGraph.Model
{

    [Category(ActionsCategory)]
    public class CallNode : FlowNode
    {

        [SerializeField]
        private string methodName;
        private ValuePort<object> thisIn;

        protected override void RegisterPorts()
        {
            base.RegisterPorts();
            thisIn = AddValueInput<object>(THIS);
        }

        public override void ExecuteContent(Flow flow)
        {

            object instance = thisIn.GetValue(flow.Context);
            MethodInfo method;

            if (instance == null)
            {
                Debug.LogError("CallNode instance null");
                return;
            }

            Type instanceType = instance.GetType();
            method = instanceType.GetMethod(methodName);

            if (method == null)
            {
                Debug.LogError("not found method  " + methodName);
            }

            if (method == null)
                return;


            method.Invoke(instance, null);
        }

    }
}