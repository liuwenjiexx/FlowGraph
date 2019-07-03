using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{

    internal class FlowGraphNode : FlowNode
    {
        [SerializeField]
        private FlowGraphAsset asset;

        public FlowGraphNode()
        {
        }

        public FlowGraphNode(FlowGraphAsset asset)
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
        public override bool IsReusable
        {
            get
            {
                return false;
            }
        }

        protected override void RegisterPorts()
        {

            AddFlowInput(FLOW_IN);
            AddFlowOutput(FLOW_OUT);
            if (asset)
            {
                var g = asset.Graph;
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

        public override void OnGraphStarted(ExecutionContext context)
        {

        }

        public override void OnGraphStoped(ExecutionContext context)
        {

        }

        public override void ExecuteContent(Flow flow)
        {

            if (asset)
            {
                var data = asset.Graph;
                if (data != null)
                {
                    ExecutionContext execCtx = asset.CreateExecutionContext(flow.Context, flow.Context.GameObject, flow.Context.MonoBehaviour);


                    foreach (var input in ValueInputs)
                    {
                        if (input.LinkOutput == null)
                            continue;
                        if (!execCtx.HasVariableLocal(input.Name))
                            execCtx.AddVariable(input.Name, input.ValueType);
                        execCtx.SetVariable(input.Name, input.GetValue(flow.Context));
                    }

                    foreach (var node in execCtx.Nodes)
                        node.OnGraphStarted(execCtx);
                    foreach (var node in execCtx.Nodes)
                        node.OnGraphStoped(execCtx);
                    foreach (var output in ValueOutputs)
                    {
                        if (execCtx.HasVariableLocal(output.Name))
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
                if (asset.Graph == null)
                {
                    HasDeserializeError = true;
                    return;
                }
            }
            ResetPorts();
        }

    }

}