using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace FlowGraph.Model
{
    [Name("Enable")]
    [Category(UnityComponentCategory)]
    public class EnableComponentNode : FlowNode
    {
        private ValuePort<Component> componentIn;

        protected override void RegisterPorts()
        {
            base.RegisterPorts();
            componentIn = AddValueInput<Component>(Type_Component);
        }

        public override void ExecuteContent(Flow flow)
        {
            ExecutionContext context = flow.Context;
            Component c;
            c = componentIn.GetValue(context);
            if (c)
            {
                if (c is Behaviour)
                {
                    ((Behaviour)c).enabled = true;
                }
                else if (c is Collider)
                {
                    ((Collider)c).enabled = true;
                }
                else if (c is Renderer)
                    ((Renderer)c).enabled = true;
            }

        }

    }



    [Name("Disable")]
    [Category(UnityComponentCategory)]
    public class DisableComponentNode : FlowNode
    {
        private ValuePort<Component> componentIn;

        protected override void RegisterPorts()
        {
            base.RegisterPorts();
            componentIn = AddValueInput<Component>(Type_Component);
        }

        public override void ExecuteContent(Flow flow)
        {
            ExecutionContext context = flow.Context;
            Component c;
            c = componentIn.GetValue(context);
            if (c)
            {
                if (c is Behaviour)
                {
                    ((Behaviour)c).enabled = false;
                }
                else if (c is Collider)
                {
                    ((Collider)c).enabled = false;
                }
                else if (c is Renderer)
                    ((Renderer)c).enabled = false;

            }
        }

    }



    [Name(" Find Componet")]
    [Category(UnityActions.FindCategory)]
    public class FindComponetNode : FlowNode
    {
        private ValuePort<Transform> rootIn;
        private ValuePort<string> nameIn;
        private ValuePort<string> typeNameIn;
        private ValuePort<Component> resultOut;


        protected override void RegisterPorts()
        {
            base.RegisterPorts();
            rootIn = AddValueInput<Transform>("root", DefaultValue.This);
            nameIn = AddValueInput<string>("name");
            typeNameIn = AddValueInput<string>("typeName");
            resultOut = AddValueOutput<Component>(Type_Component);
        }

        public override void ExecuteContent(Flow flow)
        {
            ExecutionContext context = flow.Context;
            Transform root;
            root = rootIn.GetValue(context);
            Component result = null;
            if (root)
            {
                result = FindComponent(root, nameIn.GetValue(context), typeNameIn.GetValue(context));
            }
            resultOut.SetValue(result);
        }

        public static Component FindComponent(Transform root, string name, string typeName)
        {

            Transform t = null;
            if (!string.IsNullOrEmpty(name))
            {
                t = _FindTransform(root, name);
            }
            if (!t)
                t = root;

            Type type = Extensions.GetTypeInAllAssemblies(typeName, false);
            if (type == null)
                throw new Exception("FindComponent not found type, typeName:" + typeName);
            var c = t.GetComponentInChildren(type);
            return c;
        }
        private static Transform _FindTransform(Transform root, string name)
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
                c = _FindTransform(root.GetChild(i), name);
                if (c)
                    return c;
            }

            return null;
        }

    }

}