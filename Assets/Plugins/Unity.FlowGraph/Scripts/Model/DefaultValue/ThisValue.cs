using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{

    public class ThisValueAttribute : ValueAttribute
    {
        public override IDefaultValue GetDefaultValue()
        {
            return DefaultValue.This;
        }
    }
}