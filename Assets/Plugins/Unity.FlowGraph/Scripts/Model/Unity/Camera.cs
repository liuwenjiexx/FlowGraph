using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FlowGraph.Model
{

    [Name("Main Camera")]
    [Category("Static Values")]
    public class MainCameraValue : ValueNode<Camera>
    {
        protected override Camera GetValue(Flow flow)
        {
            return Camera.main;
        }
    }

}