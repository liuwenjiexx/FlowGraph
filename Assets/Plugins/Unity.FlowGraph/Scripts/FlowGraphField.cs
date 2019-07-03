using FlowGraph.Model;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FlowGraph
{

    [Serializable]
    public class FlowGraphField
    {
        [SerializeField]
        private FlowGraphData data;
        [SerializeField]
        private FlowGraphAsset assetData;
        [SerializeField]
        private DataType dataType = DataType.Data;
        [SerializeField]
        private List<VariableInfo> inputs;

        public enum DataType
        {
            //Null = 0,
            Data = 1,
            Asset = 2,
        }

        public bool IsAssetData
        {
            get { return dataType == DataType.Asset; }
        }

        //public bool IsNull
        //{
        //    get
        //    {
        //        return dataType == DataType.Null;
        //    }
        //}

        public FlowGraphData Data
        {
            get { return data; }
            set
            {
                if (value != data)
                {
                    data = value;
                    assetData = null;
                    //if (data != null)
                    //{
                    dataType = DataType.Data;
                    //}
                    //else
                    //{
                    //    dataType = DataType.Null;
                    //}
                }
            }
        }
        public FlowGraphAsset AssetData
        {
            get { return assetData; }
            set
            {
                if (value != assetData)
                {
                    assetData = value;
                    data = null;
                    //if (assetData)
                    //{
                    dataType = DataType.Asset;
                    //}
                    //else
                    //{
                    //    dataType = DataType.Null;
                    //}
                }
            }
        }
        public List<VariableInfo> Inputs
        {
            get
            {
                return inputs;
            }
            set
            {
                inputs = value;
            }
        }

        public FlowGraphData GetFlowGraphData()
        {
            if (dataType == DataType.Asset)
            {
                if (assetData)
                    return assetData.Graph;
            }
            else if (dataType == DataType.Data)
            {
                return data;
            }
            return null;
        }

        public VariableInfo GetInput(string name)
        {
            if (inputs != null)
            {
                foreach (var variable in inputs)
                {
                    if (name == variable.Name)
                        return variable;
                }
            }
            return null;
        }


        public IEnumerable<VariableInfo> GetAllOvrrideInputs()
        {
            Dictionary<string, VariableInfo> inputs = new Dictionary<string, VariableInfo>();
            if (this.inputs != null)
            {
                foreach (var input in this.inputs)
                {
                    inputs[input.Name] = input;
                }
            }
            if (IsAssetData)
            {
                if (assetData)
                {
                    foreach (var input in assetData.GetAllOvrrideInputs())
                    {
                        if (!inputs.ContainsKey(input.Name))
                            inputs[input.Name] = input;
                    }
                }
            }
            return inputs.Values;
        }


        public ExecutionContext CreateExecutionContext(IContext parent, GameObject go, MonoBehaviour mono)
        {
            ExecutionContext context = null;

            //if (inputs != null && inputs.Count > 0)
            //{

            //    ContextData scope = new ContextData(parent);
            //    foreach (var input in inputs)
            //    {
            //        //var variable = g.GetVariable(input.Name);
            //        //if (variable == null)
            //        //{
            //        //    Debug.LogError("not found variable " + input.Name);
            //        //    continue;
            //        //}

            //        scope.AddVariable(input.Name, input.Type);
            //        scope.SetVariable(input.Name, input.DefaultValue);
            //    }
            //    parent = scope;
            //}

            if (IsAssetData)
            {
                if (assetData)
                {
                    context = assetData.CreateExecutionContext(parent, go, mono);
                }
            }
            else
            {
                if (data != null)
                {
                    context = data.CreateExecutionContext(parent, go, mono);
                }
            }

            if (context != null)
            {
                if (inputs != null && inputs.Count > 0)
                {
                    context.OvrrideInputs(inputs);
                }
            }

            return context;
        }


        public IContext Execute(GameObject go, MonoBehaviour mono)
        {
            var context = go.GetComponentInParent<IContext>();
            return Execute(go, context, mono);
        }

        public IContext Execute(GameObject go, IContext parent, MonoBehaviour mono)
        {
            ExecutionContext context = CreateExecutionContext(parent, go, mono);

            foreach (var node in context.Nodes)
            {
                node.OnGraphStarted(context);
            }

            foreach (var node in context.Nodes)
            {
                node.OnGraphStoped(context);
            }

            return context;
        }

    }

}