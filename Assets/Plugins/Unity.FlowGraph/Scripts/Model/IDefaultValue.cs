using System;

namespace FlowGraph.Model
{

    public interface IDefaultValue
    {
        object GetDefaultValue(ExecutionContext context, Type targetType);
    }


}