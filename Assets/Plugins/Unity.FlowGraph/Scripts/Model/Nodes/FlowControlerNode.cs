using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace FlowGraph.Model
{

    [Category("Flow Controllers")]
    public abstract class FlowControlerNode : FlowNode
    {





        //public IEnumerable<string> EnumerateFlowInputNames()
        //{
        //    if (flowInputs == null)
        //        yield break;
        //    foreach (var input in flowInputs)
        //        yield return input.Name;
        //}

        //public IEnumerable<string> EnumerateFlowOutputNames()
        //{
        //    if (flowOutputs == null)
        //        yield break;
        //    foreach (var output in flowOutputs)
        //        yield return output.Name;
        //}


        public override void ExecuteContent(Flow flow)
        {

        }




    }

}