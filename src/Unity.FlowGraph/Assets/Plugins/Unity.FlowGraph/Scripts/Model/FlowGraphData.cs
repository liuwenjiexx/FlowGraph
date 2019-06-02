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
            }

            n.Position = position;
            nodes.Add(n);
            return n;
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

        public void AddVariable<T>(string name)
        {
            AddVariable(name, typeof(T), default(T));
        }

        public void AddVariable(string name, Type type, object defaultValue)
        {

            VariableInfo variable = new VariableInfo(name, type, defaultValue);
            for (int i = 0; i < variables.Count; i++)
            {
                if (variables[i].Name == name)
                {
                    variables.RemoveAt(i);
                    break;
                }
            }
            variables.Add(variable);
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
            if (HasDeserializeError)
                return;


            assets.Clear();
            foreach (var nodeData in nodes)
            {
                if (nodeData.Node != null)
                {
                    var assetNode = nodeData.Node as FlowGraphAssetNode;
                    if (assetNode != null && assetNode.Asset)
                    {
                        assets.Add(assetNode.Asset);

                    }
                }
            }

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
        public bool IsDeserialize;
        public void OnAfterDeserialize()
        {
            //Debug.Log(GetType().Name + ".OnAfterDeserialize");
            IsDeserialize = true;
            foreach (var node in nodes)
            {
                foreach (var targetPort in node.FlowOutputs)
                {
                    var output = GetFlowOutput(node.Node, targetPort);
                    var input = GetTargetFlowInput(targetPort);
                    if (output != null && input != null)
                    {
                        output.LinkInput = input;
                    }
                    else
                    {
                        //Debug.LogError("link flow error:" + flowOutputStr.port + ",output null:" + (output == null) + ",input null:" + (input == null));
                    }

                }

                foreach (var targetPort in node.ValueInputs)
                {
                    var input = GetValueInput(node.Node, targetPort);
                    var output = GetTargetValueOutput(targetPort);
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
        private FlowOutput GetFlowOutput(FlowNode node, LinkedPort str)
        {
            if (node == null)
                return null;
            var output = node.GetFlowOutput(str.port);
            return output;
        }
        private FlowInput GetTargetFlowInput(LinkedPort str)
        {
            var node = GetFlowNode(str.targetNodeId);
            if (node == null)
                return null;
            return node.GetFlowInput(str.targetPort);
        }
        private ValuePort GetTargetValueOutput(LinkedPort str)
        {
            var node = GetFlowNode(str.targetNodeId);
            if (node == null)
                return null;
            var output = node.GetValueOutput(str.targetPort);
            return output;
        }
        private ValuePort GetValueInput(FlowNode node, LinkedPort str)
        {
            if (node == null)
                return null;
            return node.GetValueInput(str.port);
        }

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