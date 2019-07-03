using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;
using System.Collections.ObjectModel;

namespace FlowGraph.Model
{

    [Serializable]
    public class FlowGraphData : ISerializationCallbackReceiver
    {
        [SerializeField]
        private string name;
        [SerializeField]
        private List<FlowNodeData> nodes;
        private ReadOnlyCollection<FlowNodeData> nodesReadOnly;
        [SerializeField]
        private List<VariableInfo> variables;
        private ReadOnlyCollection<VariableInfo> variablesReadOnly;
        public List<FlowNode> activeNodes;
        [SerializeField]
        private List<FlowGraphAsset> assets = new List<FlowGraphAsset>();

        internal static string LogTag = "FlowGraph";
        internal static ILogger Logger = new UnityEngine.LogExtension.LoggerExtension();

        private static int _n;
        public static int N { get { return _n++; } }



        public FlowGraphData()
        {
            nodes = new List<FlowNodeData>();
            nodesReadOnly = nodes.AsReadOnly();
            variables = new List<VariableInfo>();
            variablesReadOnly = variables.AsReadOnly();
            activeNodes = new List<FlowNode>();

        }





        public ReadOnlyCollection<FlowNodeData> Nodes { get { return nodesReadOnly; } }

        public ReadOnlyCollection<VariableInfo> Variables { get { return variablesReadOnly; } }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public bool IsEmpty
        {
            get { return nodes.Count == 0 && variables.Count == 0; }
        }

        [SerializeField]
        public FlowGraphDataFlags flags;


        public bool HasDeserializeError
        {
            get
            {
                foreach (var n in nodes)
                {
                    if (n.HasDeserializeError)
                        return true;
                }
                return false;
            }
        }


        //private bool IsEditor
        //{
        //    get
        //    {
        //        switch (Application.platform)
        //        {
        //            case RuntimePlatform.WindowsEditor:
        //            case RuntimePlatform.OSXEditor:
        //            case RuntimePlatform.LinuxEditor:
        //                return true;
        //        }
        //        return false;
        //    }
        //}

        private FlowNodeData Exists(FlowNode node)
        {
            foreach (var n in nodes)
            {
                if (n.Node == node)
                    return n;
            }
            return null;
        }

        public FlowNodeData AddNode(FlowNode node)
        {
            return AddNode(node, Vector2.zero);
        }
        public FlowNodeData AddNode(FlowNode node, Vector2 position)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            FlowNodeData n = Exists(node);
            if (n == null)
            {
                n = new FlowNodeData(node);
                n.OnBeforeSerialize();
            }

            n.Position = position;
            nodes.Add(n);
            return n;
        }

        public FlowNodeData AddNode(FlowAsset node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            var nodeData = new FlowNodeData(node);
            nodes.Add(nodeData);
            return nodeData;
        }

        public void RemoveNode(FlowNodeData node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (!nodes.Contains(node))
                return;
            if (node.Node != null)
            {
                foreach (var flowOutput in node.Node.FlowOutputs)
                {
                    if (flowOutput.LinkInput != null)
                    {
                        BreakFlowPort(flowOutput);
                    }
                }

                foreach (var input in node.Node.ValueInputs)
                {
                    if (input.LinkOutput != null)
                    {
                        BreakValuePort(input);
                    }
                }

                foreach (var n in nodes)
                {
                    if (n.Node == null || n == node)
                        continue;

                    foreach (var output in n.Node.FlowOutputs)
                    {
                        if (output.LinkInput != null && output.LinkInput.Node == node.Node)
                        {
                            BreakFlowPort(output);
                        }
                    }

                    foreach (var input in n.Node.ValueInputs)
                    {
                        if (input.LinkOutput != null && input.LinkOutput.Node == node.Node)
                        {
                            BreakValuePort(input);
                        }
                    }
                }
            }
            nodes.Remove(node);
        }

        public FlowNodeData ToFlowNodeData(FlowNode node)
        {
            foreach (var n in nodes)
            {
                if (n.Node == node)
                    return n;
            }
            return null;
        }




        public void LinkFlowPort(FlowNode outputNode, string outputName, FlowNode inputNode, string inputName)
        {
            if (outputNode == inputNode)
                return;
            var output = outputNode.GetFlowOutput(outputName);
            var input = inputNode.GetFlowInput(inputName);
            if (output == null || input == null)
                return;

            output.LinkInput = input;
        }

        public void LinkValuePort(FlowNode outputNode, string outputName, FlowNode inputNode, string inputName)
        {
            if (outputNode == inputNode)
                return;
            var output = outputNode.GetValueOutput(outputName);
            var input = inputNode.GetValueInput(inputName);
            if (output == null || input == null)
                return;
            input.LinkOutput = output;
        }

        public void BreakFlowPort(FlowOutput output)
        {
            if (output.LinkInput != null)
            {
                output.LinkInput = null;

            }
        }

        public void BreakValuePort(ValuePort input)
        {
            if (input.LinkOutput != null)
            {
                input.LinkOutput = null;
            }
        }

        public VariableInfo AddVariable<T>(string name)
        {
           return AddVariable(name, typeof(T), default(T));
        }

        public VariableInfo AddVariable(string name, Type type, object defaultValue)
        {

            VariableInfo variable = new VariableInfo(name, type, defaultValue);
            variable.Mode = VariableMode.Local;
            for (int i = 0; i < variables.Count; i++)
            {
                if (variables[i].Name == name)
                {
                    variables.RemoveAt(i);
                    break;
                }
            }
            variables.Add(variable);
            return variable;
        }

        public bool HasVariable(string name)
        {
            return GetVariable(name) != null;
        }

        public VariableInfo GetVariable(string name)
        {
            for (int i = 0; i < variables.Count; i++)
            {
                if (variables[i].Name == name)
                {
                    return variables[i];
                }
            }
            return null;
        }

        public void AddActive(FlowNode node)
        {

        }

        public void ChangeActive(FlowNode oldNode, FlowNode newNode)
        {

        }

        public void OnBeforeSerialize()
        {
            if (IsDeserialize && !isInitial)
            {
                Initial();
                if (!isInitial)
                    return;
            }
            if (HasDeserializeError)
                return;

            //Debug.Log(GetType().Name + " OnBeforeSerialize trace:" + N);
            //assets.Clear();
            //foreach (var nodeData in nodes)
            //{
            //    if (nodeData.Node != null)
            //    {
            //        var assetNode = nodeData.Node as FlowGraphNode;
            //        if (assetNode != null && assetNode.Asset)
            //        {
            //            assets.Add(assetNode.Asset);

            //        }
            //    }
            //}

            List<LinkedPort> list = new List<LinkedPort>();

            foreach (var nodeData in nodes)
            {
                if (nodeData.Node == null)
                    continue;
                list.Clear();
                foreach (var flowOutput in nodeData.Node.FlowOutputs)
                {
                    if (flowOutput.LinkInput != null)
                    {
                        var input = ToFlowNodeWrap(flowOutput.LinkInput.Node);
                        if (input == null)
                            continue;
                        list.Add(new LinkedPort() { port = flowOutput.Name, targetNodeId = input.ID, targetPort = flowOutput.LinkInput.Name });
                    }

                }
                nodeData.FlowOutputs = list.ToArray();
                list.Clear();
                foreach (var valueInput in nodeData.Node.ValueInputs)
                {
                    if (valueInput.LinkOutput != null)
                    {
                        var output = ToFlowNodeWrap(valueInput.LinkOutput.Node);
                        if (output == null)
                            continue;
                        list.Add(new LinkedPort() { port = valueInput.Name, targetNodeId = output.ID, targetPort = valueInput.LinkOutput.Name });
                    }

                }
                nodeData.ValueInputs = list.ToArray();
            }



        }
        [NonSerialized]
        private bool isInitial;
        public bool IsInitial
        {
            get { return isInitial; }
        }
        [NonSerialized]
        public bool IsDeserialize;
        public void OnAfterDeserialize()
        {
            //if (Logger.logEnabled)
            //    Logger.Log(LogTag, GetType().Name + ".OnAfterDeserialize trace:" + N);
            IsDeserialize = true;
            isInitial = false;
            //foreach (var node in nodes)
            //{
            //    node.Graph = this;
            //    if (!node.HasDeserializeError)
            //    {
            //        if (node.IsNodeAsset)
            //        {
            //            if (node.Node == null && node.NodeAsset)
            //            {
            //                node.CreateFlowNode();
            //            }
            //        }
            //    }
            //}

            //Update();

            //Debug.Log(GetType().Name + ".OnAfterDeserialize end trace:" + N);
        }
        public void Initial()
        {
            if (isInitial)
                return;
            if (!IsDeserialize)
                return;
            //if (Logger.logEnabled)
            //    Logger.Log(LogTag, GetType().Name + " Initial");

            isInitial = true;
            IsDeserialize = false;

            Dictionary<string, FlowNode> flowNodes = new Dictionary<string, FlowNode>();
            foreach (var node in nodes)
            {
                node.Initial(this);
                if (node.Node != null)
                {
                    flowNodes[node.ID] = node.Node;
                }
            }
            Update(flowNodes);

        }

  

        public void Update(Dictionary<string, FlowNode> flowNodes)
        {

            foreach (var nodeData in nodes)
            {
                FlowNode node;
                flowNodes.TryGetValue(nodeData.ID, out node);
                if (node == null)
                    continue;
                foreach (var targetPort in nodeData.FlowOutputs)
                {
                    var output = GetFlowOutput(node, targetPort);
                    var input = GetTargetFlowInput(flowNodes, targetPort);
                    if (output != null && input != null)
                    {
                        output.LinkInput = input;
                    }
                    else
                    {
                        //Debug.LogError("link flow error:" + flowOutputStr.port + ",output null:" + (output == null) + ",input null:" + (input == null));
                    }

                }

                foreach (var targetPort in nodeData.ValueInputs)
                {
                    var input = GetValueInput(node, targetPort);
                    var output = GetTargetValueOutput(flowNodes, targetPort);
                    if (input != null && output != null)
                    {
                        input.LinkOutput = output;
                    }
                    else
                    {
                        //Debug.LogError("link value error:" + valueInputStr + ",output null:" + (output == null) + ",input null:" + (input == null));
                    }
                }

            }

        }

        private static FlowOutput GetFlowOutput(FlowNode node, LinkedPort str)
        {
            if (node == null)
                return null;
            var output = node.GetFlowOutput(str.port);
            return output;
        }
        private static FlowInput GetTargetFlowInput(Dictionary<string, FlowNode> flowNodes, LinkedPort str)
        {
            FlowNode node;
            flowNodes.TryGetValue(str.targetNodeId, out node);
            if (node == null)
                return null;
            return node.GetFlowInput(str.targetPort);
        }
        private static ValuePort GetTargetValueOutput(Dictionary<string, FlowNode> flowNodes, LinkedPort str)
        {
            FlowNode node;
            flowNodes.TryGetValue(str.targetNodeId, out node);
            if (node == null)
                return null;
            var output = node.GetValueOutput(str.targetPort);
            return output;
        }
        private static ValuePort GetValueInput(FlowNode node, LinkedPort str)
        {
            if (node == null)
                return null;
            return node.GetValueInput(str.port);
        }

        public FlowNodeData FindNode(string nodeId)
        {
            foreach (var node in nodes)
            {
                if (node.ID == nodeId)
                    return node;
            }
            return null;
        }

        //public bool FindOutput(string inputNodeId, string input, out string outputNodeId, out string output)
        //{
        //    var inputNode = FindNode(inputNodeId);
        //    if (inputNode == null)
        //        return false;
        //    inputNode.ValueInputs

        //    return false;
        //}

        private FlowNode GetFlowNode(string id)
        {
            foreach (var node in nodes)
            {
                if (node.ID == id)
                    return node.Node;
            }
            return null;
        }

        private FlowNodeData ToFlowNodeWrap(FlowNode node)
        {
            if (node != null)
            {
                foreach (var wrap in nodes)
                {
                    if (wrap.Node == node)
                        return wrap;
                }
                return null;
            }
            Debug.LogError("node to data null," + node);
            return null;
        }


        class NodeState
        {
            public FlowNode start;
            public FlowNode current;
        }

        public ExecutionContext CreateExecutionContext(IContext parent, GameObject go, MonoBehaviour mono)
        {
            var context = new ExecutionContext(parent, go, mono);

            if (Logger.logEnabled)
                Logger.Log(LogTag, GetType().Name + " CreateExecutionContext " + context.GameObject);
            Initial();

            Dictionary<string, FlowNode> flowNodes = new Dictionary<string, FlowNode>();

            foreach (var nodeData in nodes)
            {
                var node = nodeData.Node;
                if (node == null)
                    continue;
                if (!node.IsReusable)
                    node = nodeData.CreateFlowNode();
                if (node == null)
                    continue;
                flowNodes[nodeData.ID] = node;
            }

            Update(flowNodes);

            context.nodes = flowNodes.Values.ToArray();

            foreach (var variableInfo in Variables)
            {
                if ((variableInfo.Mode & VariableMode.In) == VariableMode.In)
                    continue;
                context.AddVariable(variableInfo.Name, variableInfo.Type);
                context.SetVariable(variableInfo.Name, variableInfo.DefaultValue);
            }

            return context;
        }


        public static void InitializedNewGraph(FlowGraphData graph)
        {
            if ((graph.flags & FlowGraphDataFlags.InitializedNew) != FlowGraphDataFlags.InitializedNew)
            {
                graph.AddNode(new GraphStartEvent(), new Vector2(50, 100));
                graph.AddNode(new GraphStopEvent(), new Vector2(50, 300));
                graph.flags |= FlowGraphDataFlags.InitializedNew;
               
            }
        }


    }




    [Serializable]
    public class LinkedPort
    {
        [SerializeField]
        public string port;
        [SerializeField]
        public string targetNodeId;
        [SerializeField]
        public string targetPort;
    }






}