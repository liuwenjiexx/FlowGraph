using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{

    [FlowGraphMembers]
    [Category(FlowNode.FunctionsCategory + "/String")]
    internal static class StringOperator
    {

        [Name("String Join")]
        [return: Name(FlowNode.Type_String)]
        public static string Join(string a, string b, string separator)
        {
            if (string.IsNullOrEmpty(separator))
                return a + b;
            return a + separator + b;
        }

        [Name("String Join")]
        [return: Name(FlowNode.Type_String)]
        public static string Join(object a, object b, string separator)
        {
            return Join(a == null ? string.Empty : a.ToString(), b == null ? string.Empty : b.ToString(), separator);
        }
    }

}
