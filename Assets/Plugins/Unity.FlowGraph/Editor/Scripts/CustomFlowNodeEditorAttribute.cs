using FlowGraph;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace FlowGraph.Editor
{
    public class CustomFlowNodeEditorAttribute : Attribute
    {

        private static Dictionary<Type, Type> cached;

        public CustomFlowNodeEditorAttribute(Type nodeType)
        {
            this.NodeType = nodeType;
        }

        public Type NodeType { get; private set; }


        public static Type GetEditorType(Type nodeType)
        {
            if (cached == null)
            {
                cached = new Dictionary<Type, Type>();

                CustomFlowNodeEditorAttribute attr;
                foreach (Type type in AppDomain.CurrentDomain
                   .GetAssemblies()
                   .Referenced(new Assembly[] { typeof(CustomFlowNodeEditorAttribute).Assembly })
                    .SelectMany(o => o.GetTypes()))
                {
                    if (!type.IsDefined(typeof(CustomFlowNodeEditorAttribute), false))
                        continue;
                    attr = type.GetCustomAttributes(typeof(CustomFlowNodeEditorAttribute), false).FirstOrDefault() as CustomFlowNodeEditorAttribute;

                    if (attr.NodeType == null)
                        continue;
                    cached[attr.NodeType] = type;
                }
            }

            Type editorType;
            if (!cached.TryGetValue(nodeType, out editorType))
            {
                Type p = nodeType;
                while (p != null)
                {
                    if (cached.TryGetValue(p, out editorType))
                        break;
                    if (p.IsGenericType && cached.TryGetValue(p.GetGenericTypeDefinition(), out editorType))
                        break;
                    p = p.BaseType;
                }

                //if (editorType != null)
                //{
                //    Type child = nodeType;
                //    while (child != null)
                //    {
                //        if (!cached.ContainsKey(child))
                //        {
                //            cached[child] = editorType;
                //        }
                //        if (child.IsGenericType)
                //        {
                //            if (!cached.ContainsKey(child.GetGenericTypeDefinition()))
                //            {
                //                cached[child.GetGenericTypeDefinition()] = editorType;
                //            }
                //        }
                //        if (child == p)
                //            break;
                //        child = child.BaseType;
                //    }
                //}

            }
            if (editorType == null)
                editorType = typeof(FlowNodeEditor);
            return editorType;
        }

    }
}