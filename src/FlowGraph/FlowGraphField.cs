using FlowGraph.Model;
using System;
using System.Collections.Generic;
using UnityEngine;

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
        private DataType dataType = DataType.Null;
        [SerializeField]
        private List<VariableInfo> inputs;

        public enum DataType
        {
            Null = 0,
            Data = 1,
            Asset = 2,
        }

        public bool IsAssetData
        {
            get { return dataType == DataType.Asset; }
        }

        public bool IsNull
        {
            get
            {
                return dataType == DataType.Null;
            }
        }

        public FlowGraphData Data
        {
            get { return data; }
            set
            {
                if (value != data)
                {
                    data = value;
                    assetData = null;
                    if (data != null)
                    {
                        dataType = DataType.Data;
                    }
                    else
                    {
                        dataType = DataType.Null;
                    }
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
                    if (assetData)
                    {
                        dataType = DataType.Asset;
                    }
                    else
                    {
                        dataType = DataType.Null;
                    }
                }
            }
        }
        //public List<VariableInfo> Inputs
        //{
        //    get
        //    {
        //        if (inputs == null)
        //            inputs = new List<VariableInfo>();
        //        return inputs;
        //    }
        //}

        public FlowGraphData GetFlowGraphData()
        {
            if (dataType == DataType.Asset)
            {
                if (assetData)
                    return assetData.Data;
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

        public IContext Execute(GameObject go)
        {
            var context = go.GetComponentInParent<IContext>();
            return Execute(go, context);
        }

        public IContext Execute(GameObject go, IContext parent)
        {
            ExecutionContext context = new ExecutionContext(this, parent, go);

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