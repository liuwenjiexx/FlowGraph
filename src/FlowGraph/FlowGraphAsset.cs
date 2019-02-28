using FlowGraph.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FlowGraph
{
    [Serializable]
    public class FlowGraphAsset : ScriptableObject
    {
        [SerializeField]
        private FlowGraphData graph;

      

        public FlowGraphData Data
        {
            get { return graph; }
            set { graph = value; }
        }



    }
}