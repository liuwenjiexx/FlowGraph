using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{

    [Name("Destroy Object")]
    [Category(UnityObjectCategory)]
    public class DestroyObjectNode : FlowNode
    {
        private ValuePort<Object> objectIn;
        private ValuePort<float> delayIn;

        protected override void RegisterPorts()
        {
            base.RegisterPorts();
            objectIn = AddValueInput<Object>(Type_UnityObject);
            delayIn = AddValueInput<float>("delay");
        }

        public override void ExecuteContent(Flow flow)
        {
            ExecutionContext context = flow.Context;
            Object obj;
            obj = objectIn.GetValue(context);
            if (obj)
            {
                Object.Destroy(obj, delayIn.GetValue(context));
            }
        }
    }


}