using FlowGraph.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPropertyNode : ActionNode
{
    [SerializeField]
    [Range(0, 1)]
    public float rangeValue;

    [SerializeField]
    [CeilToInt]
    public float myValue;

    public override void Execute()
    {
        Debug.Log("rangeValue:" + rangeValue + ", myValue:" + myValue);
    }
}

public class CeilToIntAttribute : PropertyAttribute
{

}