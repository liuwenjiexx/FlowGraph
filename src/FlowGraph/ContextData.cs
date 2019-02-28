using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowGraph.Model;


namespace FlowGraph
{

    internal class ContextData : IContext
    {
        private Dictionary<string, Variable> values;
        private IContext parent;


        public ContextData()
        {
            values = new Dictionary<string, Variable>();
        }


        public ContextData(IContext parent)
        {
            this.Parent = parent;
            values = new Dictionary<string, Variable>();
        }



        public object this[string name]
        {
            get { return GetVariable(name); }
            set { SetVariable(name, value); }
        }

        public IContext Parent
        {
            get { return parent; }
            set { parent = value; }
        }


        Exception GetNotFoundVariableException(string name)
        {
            return new Exception(string.Format("Not Found Variable: {0}", name));
        }

        private Variable GetLocalVariableInfo(string name)
        {
            Variable variable;
            if (values.TryGetValue(name, out variable))
                return variable;
            return null;
        }

        public bool HasVariable(string name)
        {
            if (values.ContainsKey(name))
                return true;
            if (Parent != null && Parent.HasVariable(name))
                return true;
            return false;
        }
        public bool HasVariableLocal(string name)
        {
            if (values.ContainsKey(name))
                return true;
            return false;
        }

        public void AddVariable(string name, Type type)
        {
            var variable = new VariableInfo(name, type, type.CreateDefaultValue());
            values[name] = new Variable() { value = variable.DefaultValue, variableInfo = variable };
        }


        public Type GetVariableType(string name)
        {
            var variable = GetLocalVariableInfo(name);
            if (variable != null)
                return variable.variableInfo.Type;
            if (Parent != null && Parent.HasVariable(name))
                return Parent.GetVariableType(name);
            throw GetNotFoundVariableException(name);
        }


        public object GetVariable(string name)
        {
            var variable = GetLocalVariableInfo(name);
            if (variable != null)
                return variable.value;
            if (Parent != null)
                return Parent.GetVariable(name);
            throw GetNotFoundVariableException(name);
        }

        public void SetVariable(string name, object value)
        {
            var variable = GetLocalVariableInfo(name);
            if (variable != null)
            {
                if (value != null)
                {
                    if (!variable.variableInfo.Type.IsAssignableFrom(value.GetType()))
                    {
                        throw new Exception(string.Format("SetVariable Value Type Error. Variable Name:{0}, Type:{1}, Value Type:{2}", name, variable.variableInfo.Type.Name, value.GetType().Name));
                    }
                }
                else
                {
                    if (variable.variableInfo.Type.IsValueType)
                    {
                        value = variable.variableInfo.Type.CreateDefaultValue();
                    }
                }
                variable.value = value;
                return;
            }
            if (Parent != null && Parent.HasVariable(name))
            {
                Parent.SetVariable(name, value);
                return;
            }
            throw GetNotFoundVariableException(name);
        }


        private class Variable
        {
            public object value;
            public VariableInfo variableInfo;
        }

    }


}