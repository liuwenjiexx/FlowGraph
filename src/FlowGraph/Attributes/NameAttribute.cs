using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace FlowGraph
{
    [AttributeUsage(AttributeTargets.Class| AttributeTargets.Method | AttributeTargets.ReturnValue)]
    public class NameAttribute : Attribute
    {

        public NameAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }

        public static string Get(ICustomAttributeProvider obj, string defaultValue)
        {
            var nameAttrs = obj.GetCustomAttributes(typeof(NameAttribute), false);
            if (nameAttrs == null || nameAttrs.Length == 0)
                return defaultValue;
            var nameAttr = nameAttrs[0] as NameAttribute;
            if (nameAttr != null)
            {
                if (!string.IsNullOrEmpty(nameAttr.Name))
                    return nameAttr.Name;
            }
            return defaultValue;
        }
    }




}