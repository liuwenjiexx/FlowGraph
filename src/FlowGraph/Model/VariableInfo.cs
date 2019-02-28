using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FlowGraph.Model
{


    [Serializable]
    public class VariableInfo : ISerializationCallbackReceiver
    {

        private Type type;
        [SerializeField]
        private string name;
        [SerializeField]
        private SerializableValue defaultValue;
        [SerializeField]
        private VariableMode mode;


        public VariableInfo(string name, Type type, object defaultValue)
        {
            this.name = name;
            this.type = type;
            this.defaultValue = new SerializableValue(type) { Value = defaultValue };
        }

        public Type Type
        {
            get { return type; }
        }

        public string Name
        {
            get { return name; }
            private set { name = value; }
        }

        public object DefaultValue
        {
            get { return defaultValue.Value; }
            set { defaultValue.Value = value; }

        }
        public VariableMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        public bool IsIn
        {
            get { return (mode & VariableMode.In) != 0; }
        }
        public bool IsOut
        {
            get { return (mode & VariableMode.Out) != 0; }
        }


        public void OnAfterDeserialize()
        {
            type = SerializableValue.SerializableTypeCodeToType(defaultValue.TypeCode);
            if (type == null)
                Debug.LogError("Not Found TypeName:" + defaultValue.TypeCode);
        }

        public void OnBeforeSerialize()
        {
        }


        public override string ToString()
        {
            return string.Format("{0}.{1}", name, type.Name);
        }

    }

    public enum VariableMode
    {
        None = 0,
        In = 1,
        Out = 2,
        InOut = In | Out,
    }


}