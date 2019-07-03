using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FlowGraph.Model
{

    [Category("Functions")]
    public abstract class FunctionNodeBase : FlowNode
    {


        protected override void RegisterPorts()
        {
            base.RegisterPorts();
            RegisterNodeMethodPorts("Execute");
        }
    }

    public abstract class FunctionNode<TOutput> : FunctionNodeBase
    {

        public abstract TOutput Execute();

        public override void ExecuteContent(Flow flow)
        {
            var result = Execute();
            GetValueOutput<TOutput>(0).SetValue(result);
        }
    }

    public abstract class FunctionNode<TOutput, TInput> : FunctionNodeBase
    {
        public abstract TOutput Execute(TInput arg);

        public override void ExecuteContent(Flow flow)
        {
            var arg1 = GetValueInput<TInput>(0).GetValue(flow.Context);
            var result = Execute(arg1);
            GetValueOutput<TOutput>(0).SetValue(result);
        }
    }

    public abstract class FunctionNode<TOutput, TInput1, TInput2> : FunctionNodeBase
    {
        public abstract TOutput Execute(TInput1 arg1, TInput2 arg2);

        public override void ExecuteContent(Flow flow)
        {
            var arg1 = GetValueInput<TInput1>(0).GetValue(flow.Context);
            var arg2 = GetValueInput<TInput2>(1).GetValue(flow.Context);
            var result = Execute(arg1, arg2);
            GetValueOutput<TOutput>(0).SetValue(result);
        }
    }

    public abstract class FunctionNode<TOutput, TInput1, TInput2, TInput3> : FunctionNodeBase
    {
        public abstract TOutput Execute(TInput1 arg1, TInput2 arg2, TInput3 arg3);

        public override void ExecuteContent(Flow flow)
        {
            var arg1 = GetValueInput<TInput1>(0).GetValue(flow.Context);
            var arg2 = GetValueInput<TInput2>(1).GetValue(flow.Context);
            var arg3 = GetValueInput<TInput3>(2).GetValue(flow.Context);
            var result = Execute(arg1, arg2, arg3);
            GetValueOutput<TOutput>(0).SetValue(result);
        }
    }


    public abstract class FunctionNode<TOutput, TInput1, TInput2, TInput3, TInput4> : FunctionNodeBase
    {
        public abstract TOutput Execute(TInput1 arg1, TInput2 arg2, TInput3 arg3, TInput4 arg4);

        public override void ExecuteContent(Flow flow)
        {
            var arg1 = GetValueInput<TInput1>(0).GetValue(flow.Context);
            var arg2 = GetValueInput<TInput2>(1).GetValue(flow.Context);
            var arg3 = GetValueInput<TInput3>(2).GetValue(flow.Context);
            var arg4 = GetValueInput<TInput4>(3).GetValue(flow.Context);
            var result = Execute(arg1, arg2, arg3, arg4);
            GetValueOutput<TOutput>(0).SetValue(result);
        }
    }


}