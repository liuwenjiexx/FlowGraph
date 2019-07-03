using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FlowGraph.Model
{
    public class CategoryAttribute : Attribute
    {
        public CategoryAttribute(string category)
        {
            this.Category = category;
        }

        public string Category { get; set; }

        public static string Get(ICustomAttributeProvider obj, string defaultValue)
        {
            var attrs = obj.GetCustomAttributes(typeof(CategoryAttribute), true);
            if (attrs == null || attrs.Length == 0)
                return defaultValue;
            var attr = attrs[0] as CategoryAttribute;
            if (attr != null)
            {
                if (!string.IsNullOrEmpty(attr.Category))
                    return attr.Category;
            }
            return defaultValue;
        }
    }

}
