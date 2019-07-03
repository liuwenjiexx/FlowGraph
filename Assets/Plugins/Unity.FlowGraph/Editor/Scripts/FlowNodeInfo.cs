using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FlowGraph.Editor
{
    public class FlowNodeInfo
    {
        public string Category;
        public string Description;
        public string Name;
        public string CategoryName;
        public string DisplayFullName;
        public Type Type;
        public MethodInfo method;
        public FlowNodeType NodeType;
        public List<MemberInfo> dataMembers;
        public Type ValueType;
        public bool ignoreMenu;

        public bool isFlowAsset;  
         
    }
}