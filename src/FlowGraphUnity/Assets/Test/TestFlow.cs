
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowGraph;
using System;


public class TestFlow : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Debug.Log( new DateTime(2018, 7, 27).AddDays(100));
        //Log log1 = new Log();
        //log1.ValueInputs[0].Value = "log1";

        //Log log2 = new Log();
        //log2.ValueInputs[0].Value = "log2";

        //log1.FlowOutputs[0].LinkInput = log2.FlowInputs[0];

        ////  FlowGraph g = new FlowGraph();
        //FlowGraphData g = GetComponent<FlowController>().graph;

        //g.AddNode(log1);
        //g.AddNode(log2);

        ////GetComponent<FlowController>().graph = g;
        //log1.FlowInputs[0].Execute(g);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
