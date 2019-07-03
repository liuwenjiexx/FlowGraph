using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{

    [Name("Time")]
    [Category("Time")]
    public class TimeValue : ValueNode<float>
    {
        protected override string OutputName
        {
            get { return "time"; }
        }
        protected override float GetValue(Flow flow)
        {
            return Time.time;
        }
    }


    [Name("Unscaled Time")]
    [Category("Time")]
    public class UnscaledTimeValue : ValueNode<float>
    {
        protected override string OutputName
        {
            get { return "unscaledTime"; }
        }
        protected override float GetValue(Flow flow)
        {
            return Time.unscaledTime;
        }
    }

    [Name("TimeScale")]
    [Category("Time")]
    public class TimeScaleValue : ValueNode<float>
    {
        protected override string OutputName
        {
            get { return "timeScale"; }
        }
        protected override float GetValue(Flow flow)
        {
            return Time.timeScale;
        }
    }

    [Name("Delta Time")]
    [Category("Time")]
    public class DeltaTimeValue : ValueNode<float>
    {
        protected override string OutputName
        {
            get { return "deltaTime"; }
        }
        protected override float GetValue(Flow flow)
        {
            return Time.deltaTime;
        }
    }

    [Name("Unscaled Delta Time")]
    [Category("Time")]
    public class UnscaledDeltaTimeValue : ValueNode<float>
    {
        protected override string OutputName
        {
            get { return "unscaledDeltaTime"; }
        }
        protected override float GetValue(Flow flow)
        {
            return Time.unscaledDeltaTime;
        }
    }

}