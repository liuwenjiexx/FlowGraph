using FlowGraph.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlowGraph
{

    public class ExecutionContext : IContext
    {

        private IContext parent;
        private ContextData contextData;
        private GameObject gameObject;
        private MonoBehaviour mono;
        internal FlowNode[] nodes;
        FlowGraphData graph;

        internal ExecutionContext(IContext parent, GameObject go, MonoBehaviour mono)
        {
            this.gameObject = go;
            this.parent = parent;
            this.mono = mono ?? FlowGraphBehaviour.Instance;
            contextData = new ContextData(parent);
        }

        public GameObject GameObject
        {
            get { return gameObject; }
        }
        public IEnumerable<FlowNode> Nodes
        {
            get { return nodes; }
        }

        public MonoBehaviour MonoBehaviour
        {
            get { return mono; }
        }



        public void StartCoroutine(IEnumerator routine)
        {
            mono.StartCoroutine(routine);
        }

        public void StartCoroutine(IEnumerator routine, Action callback)
        {
            mono.StartCoroutine(_StartCoroutine(routine, callback));
        }
        private IEnumerator _StartCoroutine(IEnumerator routine, Action callback)
        {
            yield return mono.StartCoroutine(routine);
            if (callback != null)
                callback();
        }


        private IEnumerable<T> FindNodes<T>(string eventName)
           where T : FlowNode
        {
            foreach (var node in Nodes)
            {
                var evtNode = node as T;
                if (evtNode == null)
                    continue;
                yield return evtNode;
            }
        }


        #region Send Event

        public void SendEvent(string eventName)
        {
            foreach (var node in FindNodes<OnEvent>(eventName))
            {
                if (node.EventName != eventName)
                    continue;
                node.TriggerEvent(this);
            }
        }

        public void SendEvent(string eventName, object arg1)
        {
            foreach (var node in FindNodes<OnEventArg1>(eventName))
            {
                if (node.EventName != eventName)
                    continue;
                node.TriggerEvent(this, arg1);
            }
        }

        #endregion

        #region Send Enterable Event


        public void SendEnterEvent(string eventName)
        {
            foreach (var node in FindNodes<StateEvent>(eventName))
            {
                if (node.EventName != eventName)
                    continue;
                node.OnEnter(this);
            }
        }

        public void SendExitEvent(string eventName)
        {
            foreach (var node in FindNodes<StateEvent>(eventName))
            {
                if (node.EventName != eventName)
                    continue;
                node.OnExit(this);
            }
        }

        public void SendEnterEvent(string eventName, object arg1)
        {
            foreach (var node in FindNodes<StateEventArg1>(eventName))
            {
                if (node.EventName != eventName)
                    continue;
                node.OnEnter(this, arg1);
            }
        }

        public void SendExitEvent(string eventName, object arg1)
        {
            foreach (var node in FindNodes<StateEventArg1>(eventName))
            {
                if (node.EventName != eventName)
                    continue;
                node.OnExit(this, arg1);
            }
        }

        #endregion


        //public object GetInjectValue(Inject inject)
        //{
            
        //    return GetInjectValue(inject.Type, inject.Name, inject.ValueType);
        //}

        //public object GetInjectValue(Type type, string name, InjectValueType valueType)
        //{
        //    if (!string.IsNullOrEmpty(name))
        //    {
        //        return GetVariable(name);
        //    }

        //    if (valueType != InjectValueType.None)
        //    {
        //        switch (valueType)
        //        {
        //            case InjectValueType.GameObject:
        //                return gameObject;
        //            case InjectValueType.Transform:
        //                return gameObject.transform;
        //            case InjectValueType.DeltaTime:
        //                return Time.deltaTime;
        //            case InjectValueType.UnscaledDeltaTime:
        //                return Time.unscaledDeltaTime;
        //            case InjectValueType.FixedTime:
        //                return Time.fixedTime;
        //        }
        //    }

        //    if (type != null)
        //    {
        //        if (type == typeof(GameObject))
        //            return gameObject;
        //        else if (type == typeof(ExecutionContext))
        //            return this;
        //        else if (typeof(Component).IsAssignableFrom(type))
        //            return gameObject.GetComponent(type);
        //        else
        //            throw new ArgumentException("not support inject type:" + type.Name);
        //    }
        //    throw new ArgumentException(string.Format("get inject value fail. type:{0},name:{1}, value type:{2}", type, name, valueType));
        //}

        public bool HasVariable(string name)
        {
            return contextData.HasVariable(name);
        }
        public bool HasVariableLocal(string name)
        {
            return contextData.HasVariableLocal(name);
        }
        public Type GetVariableType(string name)
        {
            return contextData.GetVariableType(name);
        }
        public void AddVariable(string name, Type type)
        {
            contextData.AddVariable(name, type);
        }
        public void SetVariable(string name, object value)
        {
            contextData.SetVariable(name, value);
        }

        public object GetVariable(string name)
        {
            return contextData.GetVariable(name);
        }

        public void OvrrideInputs(IList<VariableInfo> inputs)
        {
            if (inputs != null && inputs.Count > 0)
            {
                int count = inputs.Count;
                for (int i = 0; i < count; i++)
                {
                    var input = inputs[i];
                    if (!HasVariableLocal(input.Name))
                    {
                        AddVariable(input.Name, input.Type);
                    }
                    SetVariable(input.Name, input.DefaultValue);
                }
            }
        }

        class FlowGraphBehaviour : MonoBehaviour
        {
            private static FlowGraphBehaviour instance;

            public static FlowGraphBehaviour Instance
            {
                get
                {
                    if (instance == null)
                    {
                        GameObject go = new GameObject(typeof(FlowGraphBehaviour).Name);
                        instance = go.AddComponent<FlowGraphBehaviour>();
                        GameObject.DontDestroyOnLoad(go);
                    }
                    return instance;
                }
            }

        }
    }




}