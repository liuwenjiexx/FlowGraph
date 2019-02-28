using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowGraph;
using System;

[DefaultExecutionOrder(-1)]
public class TestContext : MonoBehaviour
{
    public InputItem[] items;

    [Serializable]
    public class InputItem
    {
        public string name;
        public string value;
    }


    void Start()
    {
        var ctx = gameObject.EnsureComponent<FlowContext>();
        foreach (var item in items)
        {
            ctx.AddVariable(item.name, typeof(string));
            ctx.SetVariable(item.name, item.value);
        }

      

    }



}
