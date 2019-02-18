using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FlowGraph
{
    public interface IContext
    {
        bool HasVariable(string name);
        Type GetVariableType(string name);
        void SetVariable(string name, object value);
        object GetVariable(string name);
    }
}
