using System;
using UnityEngine;

namespace FlowGraph.Model
{

    public class VariableValueAttribute : ValueAttribute
    {

        public VariableValueAttribute(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            this.Name = name;
        }


        public string Name { get; set; }

        public override IDefaultValue GetDefaultValue()
        {
            return new VariableValue(Name);
        }
    }

    public class VariableValue : IDefaultValue
    {
        private string name;

        public VariableValue(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }


        public object GetDefaultValue(ExecutionContext context, Type targetType)
        {
            return context.GetVariable(name);
        }


        public override string ToString()
        {
            string str = null;
            if (!string.IsNullOrEmpty(Name))
                str = "$" + Name;
            if (str == null)
                str = "null";
            return str;
        }

    }



}