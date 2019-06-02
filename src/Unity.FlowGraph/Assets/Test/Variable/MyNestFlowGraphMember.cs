using FlowGraph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNestFlowGraphMember : MonoBehaviour {

    [SerializeField]
    public MyClass myClass;


    [SerializeField]
    public MyClass[] array;

    [System.Serializable] 
    public class MyClass
    {
        public FlowGraphField fied;
    }

    private void Start()
    {
        var ctx= myClass.fied.Execute(gameObject, null);
     //   ctx.GetVariable()
    }


}
