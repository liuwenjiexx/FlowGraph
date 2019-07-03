using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{
    [Flags]
    public enum FlowGraphDataFlags
    {
        None = 0,
        InitializedNew = 1,
    }

}