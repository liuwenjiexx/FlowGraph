using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{

    [Category("This")]
    public abstract class ThisValueNode<TValue> : ValueNode<TValue>
    {

    }

    [Name("This GameObject")]
    public class ThisGameObjectValue : ThisValueNode<GameObject>
    {
        protected override string OutputName
        {
            get { return Type_GameObject; }
        }

        protected override GameObject GetValue(Flow flow)
        {
            return flow.Context.GameObject;
        }
    }


    [Name("This Transform")]
    [Category("This")]
    public class ThisTransformValue : ThisValueNode<Transform>
    {
        protected override string OutputName
        {
            get { return Type_Transform; }
        }
        protected override Transform GetValue(Flow flow)
        {
            return flow.Context.GameObject.transform;
        }
    }


    [Name("This Collider")]
    [Category("This")]
    public class ThisColliderValue : ThisValueNode<Collider>
    {
        protected override string OutputName
        {
            get { return Type_Collider; }
        }
        protected override Collider GetValue(Flow flow)
        {
            return flow.Context.GameObject.GetComponent<Collider>();
        }
    }

}