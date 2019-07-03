using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FlowGraph.Model
{
    [Name("Active")]
    [Category(UnityGameObjectCategory)]
    public class ActiveGameObjectNode : FlowNode
    {
        private ValuePort<GameObject> gameObjectIn;

        protected override void RegisterPorts()
        {
            base.RegisterPorts();
            gameObjectIn = AddValueInput<GameObject>(Type_GameObject);
        }

        public override void ExecuteContent(Flow flow)
        {
            ExecutionContext context = flow.Context;
            GameObject go;
            go = gameObjectIn.GetValue(context);
            if (go)
            {
                go.SetActive(true);
            }
        }

    }


    [Name("Inactive")]
    [Category(UnityGameObjectCategory)]
    public class InactiveGameObjectNode : FlowNode
    {
        private ValuePort<GameObject> gameObjectIn;

        protected override void RegisterPorts()
        {
            base.RegisterPorts();
            gameObjectIn = AddValueInput<GameObject>(Type_GameObject);
        }

        public override void ExecuteContent(Flow flow)
        {
            ExecutionContext context = flow.Context;
            GameObject go;
            go = gameObjectIn.GetValue(context);
            if (go)
            {
                go.SetActive(false);
            }
        }
    }



    [Name("Destroy")]
    [Category(UnityGameObjectCategory)]
    public class DestroyGameObjectNode : FlowNode
    {
        private ValuePort<GameObject> gameObjectIn;
        private ValuePort<float> delayIn;

        protected override void RegisterPorts()
        {
            base.RegisterPorts();
            gameObjectIn = AddValueInput<GameObject>(Type_GameObject);
            delayIn = AddValueInput<float>("delay");
        }

        public override void ExecuteContent(Flow flow)
        {
            ExecutionContext context = flow.Context;
            GameObject go;
            go = gameObjectIn.GetValue(context);
            if (go)
            {
                Object.Destroy(go, delayIn.GetValue(context));
            }
        }
    }


    [Name("Find GameObject")]
    [Category(UnityActions.FindCategory)]
    public class FindGameObjectNode : FlowNode
    {
        private ValuePort<Transform> rootIn;
        private ValuePort<string> nameIn;
        private ValuePort<GameObject> resultOut;


        protected override void RegisterPorts()
        {
            base.RegisterPorts();
            rootIn = AddValueInput<Transform>("root", DefaultValue.This);
            nameIn = AddValueInput<string>("name");
            resultOut = AddValueOutput<GameObject>(Type_GameObject);
        }

        public override void ExecuteContent(Flow flow)
        {
            ExecutionContext context = flow.Context;
            GameObject result = null;
            string name = nameIn.GetValue(context);
            if (!string.IsNullOrEmpty(name))
            {
                var root = rootIn.GetValue(context);
                if (root)
                {
                    var t = FindTransform(root, name);
                    if (t)
                        result = t.gameObject;
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
