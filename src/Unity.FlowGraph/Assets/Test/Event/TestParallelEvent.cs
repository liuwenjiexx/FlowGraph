using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowGraph;

namespace Tests
{
    public class TestParallelEvent : MonoBehaviour
    {

        FlowGraphController controller;

        // Use this for initialization
        void Start()
        {
            controller = GetComponent<FlowGraphController>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        int n;
        private void OnGUI()
        {
            if (GUILayout.Button("Send Event"))
            {
                n++;
                Debug.Log("Send:" + n);
                controller.Context.SendEvent("MyEvent", n);

                n++;
                Debug.Log("Send:" + n);
                controller.Context.SendEvent("MyEvent", n);
            }
        }
    }
}