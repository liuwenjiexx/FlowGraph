using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{
    [Name("FlowGraph Asset")]
    public class FlowGraphAssetNode : FlowNode
    {
        [SerializeField]
        private FlowGraphAsset asset;

        public FlowGraphAssetNode()
        {
        }

        public FlowGraphAssetNode(FlowGraphAsset asset)
        {
            this.asset = asset;
            ResetPorts();
        }

        public FlowGraphAsset Asset
        {
            get
            {
                return asset;
            }
        }

        protected override void RegisterPorts()
        {

            AddFlowInput(FLOW_IN);
            AddFlowOutput(FLOW_OUT);
            if (asset)
            {
                var g = asset.Data;
                if (g != null)
                {
                    foreach (var variable in g.Variables)
                    {
                        if (variable.IsIn)
                        {
                            AddValueInput(variable.Name, variable.Type);
                        }
                        else if (variable.IsOut)
                        {
                            AddValueOutput(variable.Name, variable.Type);
                        }
                    }
                }

            }
        }



        public override void ExecuteContent(Flow flow)
        {

            if (asset)
            {
                var data = asset.Data;
                if (data != null)
                {
                    var ctx = new ContextData(flow.Context);
                    foreach (var input in ValueInputs)
                    {
                        ctx.AddVariable(input.Name, input.ValueType);
                        //if (data.HasVariable(input.Name))
                        {
                            ctx.SetVariable(input.Name, input.GetValue(flow.Context));
                        }
                    }
                    ExecutionContext execCtx = new ExecutionContext(data, ctx, flow.Context.GameObject);

                    foreach (var node in execCtx.Nodes)
                        node.OnGraphStarted(execCtx);
                    foreach (var node in execCtx.Nodes)
                        node.OnGraphStoped(execCtx);
                    foreach (var output in ValueOutputs)
                    {
                        if (data.HasVariable(output.Name))
                        {
                            output.SetValue(execCtx.GetVariable(output.Name));
                        }
                    }
                }
            }

        }


        public override void OnAfterDeserialize()
        {
            HasDeserializeError = false;
             
            if (asset)
            {
                if (asset.Data == null)
                {
                    HasDeserializeError = true;
                    return;
                }
            }
            ResetPorts();
        }

    }

}