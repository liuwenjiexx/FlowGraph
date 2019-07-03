using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph
{

    public class FlowContext : MonoBehaviour, IContext
    {

        private ContextData context;


        private void Awake()
        {
            context = new ContextData();

        }


        private void Start()
        {
            var parent = GetComponentInParent<IContext>();
            context.Parent = parent;
        }


        private void OnTransformParentChanged()
        {
            var parent = GetComponentInParent<IContext>();
            context.Parent = parent;
        }


        public void AddVariable(string name, Type type)
        {
            context.AddVariable(name, type);
        }

        public object GetVariable(string name)
        {
            return context.GetVariable(name);
        }

        public Type GetVariableType(string name)
        {
            return context.GetVariableType(name);
        }

        public bool HasVariable(string name)
        {
            return context.HasVariable(name);
        }
        public bool HasVariableLocal(string name)
        {
            return context.HasVariableLocal(name);
        }

        public void SetVariable(string name, object value)
        {
            context.SetVariable(name, value);
        }

    }

}