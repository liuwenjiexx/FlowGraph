using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowGraph;


public class TestSendEvent : MonoBehaviour
{

    FlowGraphController controller;

    // Use this for initialization
    void Start()
    {
        controller = GetComponent<FlowGraphController>();
    }

 

    private void OnGUI()
    {
        if (GUILayout.Button("Send MyEvent1"))
        {
            controller.Context.SendEvent("MyEvent1");
        }
        if (GUILayout.Button("Send MyEvent2(Hello)"))
        {
            controller.Context.SendEvent("MyEvent2", "Hello");
        }

      
    }

}
