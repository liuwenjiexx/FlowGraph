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
        private FlowGraphData graph;
        private ContextData contextData;
        private GameObject gameObject;

        public ExecutionContext(FlowGraphField field, IContext parent, GameObject go)
            : this(field.GetFlowGraphData(), parent, go)
        {
            //if (field.Inputs != null)
            //{
            //    foreach (var input in field.Inputs)
            //    {
            //        if (!graph.HasVariable(input.Name))
            //            continue;

            //        var varInfo = graph.GetVariable(input.Name);
            //        if (varInfo.IsIn)
            //        {
            //            contextData.SetVariable(input.Name, input.DefaultValue);
            //        }
            //    }
            //}
        }

        public ExecutionContext(FlowGraphData graphData, IContext parent, GameObject go)
        {
            this.gameObject = go;
            this.graph = graphData;
            this.parent = parent;

            contextData = new ContextData(parent);

            foreach (var variableInfo in graph.Variables)
            {
                if ((variableInfo.Mode & VariableMode.In) == VariableMode.In)
                    continue;
                contextData.AddVariable(variableInfo.Name, variableInfo.Type);
                contextData.SetVariable(variableInfo.Name, variableInfo.DefaultValue);
            }
        }


        public FlowGraphData Graph
        {
            get { return graph; }
        }

        public GameObject GameObject
        {
            get { return gameObject; }
        }
        public IEnumerable<FlowNode> Nodes
        {
            get { return graph.Nodes.Select(o => o.Node).Where(o => o != null); }
        }






        public void StartCoroutine(IEnumerator routine)
        {
            FlowGraphBehaviour.Instance.StartCoroutine(routine);
        }

        public void StartCoroutine(IEnumerator routine, Action callback)
        {
            FlowGraphBehaviour.Instance.StartCoroutine(_StartCoroutine(routine, callback));
        }
        private IEnumerator _StartCoroutine(IEnumerator routine, Action callback)
        {
            yield return FlowGraphBehaviour.Instance.StartCoroutine(routine);
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
                yield return (T)node;
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
            foreach (var node in FindNodes<EnterableEvent>(eventName))
            {
                if (node.EventName != eventName)
                    continue;
                node.OnEnter(this);
            }
        }

        public void SendExitEvent(string eventName)
        {
            foreach (var node in FindNodes<EnterableEvent>(eventName))
            {
                if (node.EventName != eventName)
                    continue;
                node.OnExit(this);
            }
        }

        public void SendEnterEvent(string eventName, object arg1)
        {
            foreach (var node in FindNodes<EnterableEventArg1>(eventName))
            {
                if (node.EventName != eventName)
                    continue;
                node.OnEnter(this, arg1);
            }
        }

        public void SendExitEvent(string eventName, object arg1)
        {
            foreach (var node in FindNodes<EnterableEventArg1>(eventName))
            {
                if (node.EventName != eventName)
                    continue;
                node.OnExit(this, arg1);
            }
        }

        #endregion


        public object GetInjectValue(Inject inject)
        {
            return GetInjectValue(inject.Type, inject.Name, inject.ValueType);
        }

        public object GetInjectValue(Type type, string name, InjectValueType valueType)
        {
            if (!string.IsNullOrEmpty(name))
            {
                return GetVariable(name);
            }

            if (valueType != InjectValueType.None)
            {
                switch (valueType)
                {
                    case InjectValueType.GameObject:
                        return gameObject;
                    case InjectValueType.Transform:
                        return gameObject.transform;
                    case InjectValueType.DeltaTime:
                        return Time.deltaTime;
                    case InjectValueType.UnscaledDeltaTime:
                        return Time.unscaledDeltaTime;
                    case InjectValueType.FixedTime:
                        return Time.fixedTime;
                }
            }

            if (type != null)
            {
                if (type == typeof(GameObject))
                    return gameObject;
                else if (type == typeof(ExecutionContext))
                    return this;
                else if (typeof(Component).IsAssignableFrom(type))
                    return gameObject.GetComponent(type);
                else
                    throw new ArgumentException("not support inject type:" + type.Name);
            }
            throw new ArgumentException(string.Format("get inject value fail. type:{0},name:{1}, value type:{2}", type, name, valueType));
        }

        public bool HasVariable(string name)
        {
            return contextData.HasVariable(name);
        }

        public Type GetVariableType(string name)
        {
            return contextData.GetVariableType(name);
        }

        public void SetVariable(string name, object value)
        {
            contextData.SetVariable(name, value);
        }

        public object GetVariable(string name)
        {
            return contextData.GetVariable(name);
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