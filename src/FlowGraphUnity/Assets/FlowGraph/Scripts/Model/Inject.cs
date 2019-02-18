using System;

namespace FlowGraph.Model
{

    public class Inject
    {
        private Type type;
        private string name;
        private InjectValueType valueType;

        public Inject(Type type)
        {
            this.type = type;
        }
        public Inject(string name)
        {
            this.name = name;
        }
        public Inject(InjectValueType valueType)
        {
            this.valueType = valueType;
        }


        public Type Type
        {
            get { return type; }
        }

        public string Name
        {
            get { return name; }
        }

        public InjectValueType ValueType
        {
            get { return valueType; }
        }
    }


    public enum InjectValueType
    {
        None,
        GameObject,
        Transform,
        DeltaTime,
        UnscaledDeltaTime,
        FixedTime,
    }

}