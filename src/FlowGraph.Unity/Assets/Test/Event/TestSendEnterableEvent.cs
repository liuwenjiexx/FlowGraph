using FlowGraph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{

    public class TestSendEnterableEvent : MonoBehaviour
    {


        FlowGraphController controller;

        // Use this for initialization
        void Start()
        {
            controller = GetComponent<FlowGraphController>();
        }


 

        private void OnGUI()
        {
            if (GUILayout.Button("SendEnter MyEnterableEvent1.enter"))
            {
                controller.Context.SendEnterEvent("MyEnterableEvent1");
            }

            if (GUILayout.Button("SendExit MyEnterableEvent1.exit"))
            {
                controller.Context.SendExitEvent("MyEnterableEvent1");
            }

            if (GUILayout.Button("SendEnter MyEnterableEvent1.enter hello"))
            {
                controller.Context.SendEnterEvent("MyEnterableEvent2", "hello");
            }

            if (GUILayout.Button("SendExit MyEnterableEvent1.exit world"))
            {
                controller.Context.SendExitEvent("MyEnterableEvent2", "world");
            }
        }
    }

}