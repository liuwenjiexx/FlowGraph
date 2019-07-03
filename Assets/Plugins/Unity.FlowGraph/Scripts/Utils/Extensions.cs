using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FlowGraph
{


    public static class Extensions
    {

        /// <summary>
        /// (type, typeof(T<>))
        /// </summary>
        /// <param name="type"></param>
        /// <param name="generic"></param>
        /// <returns></returns>
        public static bool IsGenericSubclassOf(this Type type, Type generic)
        { 
            return FindGenericType(type,generic)!=null;
        }

        /// <summary>
        /// (type, typeof(T<>))
        /// </summary>
        /// <param name="type"></param>
        /// <param name="generic"></param>
        /// <returns></returns>
        public static Type FindGenericType(this Type type, Type generic)
        {
            Type p = type;
            while (p != null)
            {
                if (p == generic)
                    return p;
                if (p.IsGenericType && p.GetGenericTypeDefinition() == generic)
                    return p;
                p = p.BaseType;
            }
            return null;
        }
        public static Type GetTypeInAllAssemblies(this string typeName)
        {
            return GetTypeInAllAssemblies(typeName, false);
        }
        public static Type GetTypeInAllAssemblies(this string typeName, bool ignoreCase)
        {
            Type type;
            type = Type.GetType(typeName, false, ignoreCase);
            if (type == null)
            {
                foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
                {
                    type = ass.GetType(typeName, false, ignoreCase);
                    if (type != null)
                        break;
                }
            }
            return type;
        }

        public static object CreateDefaultValue(this Type type)
        {
            if (type.IsValueType)
            {
                if (type == typeof(string))
                    return null;
                return Activator.CreateInstance(type);
            }
            else
            {
                return null;
            }
        }

        public static T EnsureComponent<T>(this GameObject go)
            where T : Component
        {
            var c = go.GetComponent<T>();
            if (!c)
                c = go.AddComponent<T>();
            return c;
        }


        public static bool IsRef(this ParameterInfo pInfo)
        {
            return !pInfo.IsOut && pInfo.ParameterType.IsByRef;
        }

    

    }


}