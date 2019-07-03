using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FlowGraph.Model
{
    [Name("Find Transform")]
    [Category(UnityActions.FindCategory)]
    public class FindTransformNode : FlowNode
    {
        private ValuePort<Transform> rootIn;
        private ValuePort<string> nameIn;
        private ValuePort<Transform> resultOut;


        protected override void RegisterPorts()
        {
            base.RegisterPorts();
            rootIn = AddValueInput<Transform>("root", DefaultValue.This);
            nameIn = AddValueInput<string>("name");
            resultOut = AddValueOutput<Transform>(Type_Transform);
        }

        public override void ExecuteContent(Flow flow)
        {
            ExecutionContext context = flow.Context;

            Transform result = null;

            string name = nameIn.GetValue(context);
            if (!string.IsNullOrEmpty(name))
            {
                Transform root;
                root = rootIn.GetValue(context);
                if (root)
                {
                    result = FindTransform(root, name);
                }
            }
            resultOut.SetValue(result);
        }

        private static Transform FindTransform(Transform root, string name)
        {
            Transform c;
            for (int i = 0, len = root.childCount; i < len; i++)
            {
                c = root.GetChild(i);
                if (c.name == name)
                    return c;
            }

            for (int i = 0, len = root.childCount; i < len; i++)
            {
                c = FindTransform(root.GetChild(i), name);
                if (c)
                    return c;
            }

            return null;
        }


    }


}