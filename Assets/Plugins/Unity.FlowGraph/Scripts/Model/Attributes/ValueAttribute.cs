using System;


namespace FlowGraph.Model
{

    [AttributeUsage(AttributeTargets.Parameter)]
    public abstract class ValueAttribute : Attribute
    {
        public abstract IDefaultValue GetDefaultValue();

    }




    //public class ThisGameObjectAttribute : DefaultValueBaseAttribute
    //{
    //    public override IDefaultValue GetDefaultValue()
    //    {
    //        return DefaultValue.ThisGameObject;
    //    }
    //}
    //public class ThisTransformAttribute : DefaultValueBaseAttribute
    //{
    //    public override IDefaultValue GetDefaultValue()
    //    {
    //        return DefaultValue.ThisTransform;
    //    }
    //}
    public class UnscaledDeltaTimeAttribute : ValueAttribute
    {
        public override IDefaultValue GetDefaultValue()
        {
            return DefaultValue.UnscaledDeltaTime;
        }
    }
    public class DeltaTimeAttribute : ValueAttribute
    {
        public override IDefaultValue GetDefaultValue()
        {
            return DefaultValue.DeltaTime;
        }
    }
    public class FixedTimeAttribute : ValueAttribute
    {
        public override IDefaultValue GetDefaultValue()
        {
            return DefaultValue.FixedTime;
        }
    }
}