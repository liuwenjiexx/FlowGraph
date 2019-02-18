using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FlowGraph
{

    public class DescriptionAttribute : Attribute
    {
        public DescriptionAttribute(string description)
        {
            this.Description = description;
        }

        public string Description { get; set; }

        public static string Get(ICustomAttributeProvider obj, string defaultValue)
        {
            var attrs = obj.GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (attrs == null || attrs.Length == 0)
                return defaultValue;
            var attr = attrs[0] as DescriptionAttribute;
            if (attr != null)
            {
                if (!string.IsNullOrEmpty(attr.Description))
                    return attr.Description;
            }
            return defaultValue;
        }
    }

}