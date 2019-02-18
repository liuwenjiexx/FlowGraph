using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowGraph.Model;
using FlowGraph;


namespace Tests
{
    public class TestGraphField : MonoBehaviour
    {

        public FlowGraphField graph;

        // Use this for initialization
        void Start()
        {
            IContext context = graph.Execute(gameObject);

            Debug.Log("Return:" + context.GetVariable("return"));
        }

    }

}