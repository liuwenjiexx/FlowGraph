using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{

    [FlowGraph]
    [Category(FlowNode.FunctionsCategory + "/Logic")]
    internal static class Logic
    {
        private static bool IsTrue(object obj)
        {
            if (obj == null)
                return false;
            if (obj is bool)
                return (bool)obj;
            var objType = obj.GetType();
            if (objType.IsPrimitive)
            {
                switch (Type.GetTypeCode(objType))
                {
                    case TypeCode.Int32:
                        return (int)obj != 0;
                    case TypeCode.Int64:
                        return (long)obj != 0;
                    case TypeCode.Single:
                        return (float)obj != 0.0f;
                    case TypeCode.Double:
                        return (double)obj != 0.0d;
                }
            }
            else
            {
                if (objType == typeof(string))
                    return ((string)obj).Length != 0;
            }
            return true;
        }

        [Name("And")]
        [return: Name(FlowNode.Type_Bool)]
        public static bool And(object a, object b)
        {
            if (!IsTrue(a))
                return false;
            return IsTrue(b);
        }

        [Name("Or")]
        [return: Name(FlowNode.Type_Bool)]
        public static bool Or(object a, object b)
        {
            if (IsTrue(a))
                return true;
            return IsTrue(b);
        }

        [Name("Not")]
        [return: Name(FlowNode.Type_Bool)]
        public static bool Not(object value)
        {
            return !IsTrue(value);
        }

        [Name("And Value")]
        [Category("Functions/Logic/Value")]
        [return: Name(FlowNode.Type_Object)]
        public static object AndValue(object a, object b)
        {
            if (!IsTrue(a))
                return a;
            return b;
        }

        [Name("Or Value")]
        [Category("Functions/Logic/Value")]
        [return: Name(FlowNode.Type_Object)]
        public static object OrValue(object a, object b)
        {
            if (IsTrue(a))
                return a;
            return b;
        }
    }

}