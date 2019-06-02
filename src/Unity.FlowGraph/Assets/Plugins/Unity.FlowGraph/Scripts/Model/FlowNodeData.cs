using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System.Collections.ObjectModel;

namespace FlowGraph.Model
{

    [Serializable]
    public class FlowNodeData : ISerializationCallbackReceiver
    {
        [SerializeField]
        private string id;
        [SerializeField]
        private string typeName;

        [SerializeField]
        private LinkedPort[] flowOutputs;
        [SerializeField]
        private LinkedPort[] valueInputs;

        [SerializeField]
        private Vector2 position;
        [SerializeField]
        private List<PropertyData> properties;
        private FlowNode node;

        private static Dictionary<Type, FieldInfo[]> cachedFields;
        private static Dictionary<string, Type> cachedTypes;

        public FlowNodeData(FlowNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");
            this.id = System.Guid.NewGuid().ToString("N");
            this.node = node;


        }
        public FlowNodeData()
        {

        }


        public string ID
        {
            get { return id; }
        }

        public string TypeName
        {
            get { return typeName; }
            set
            {
                if (typeName != value)
                {
                    this.typeName = value;
                }
            }
        }



        public string DisplayTypeName
        {
            get
            {
                if (typeName == null)
                    return null;
                return typeName.Split(',')[0];
            }
        }
        public FlowNode Node
        {
            get { return node; }
            set { node = value; }
        }


        public LinkedPort[] FlowOutputs
        {
            get { return flowOutputs; }
            set { flowOutputs = value; }
        }

        public LinkedPort[] ValueInputs
        {
            get
            {
                return valueInputs;
            }

            set
            {
                valueInputs = value;
            }
        }

        public Vector2 Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
            }
        }

        public ReadOnlyCollection<PropertyData> Properties
        {
            get
            {
                if (properties == null)
                    return null;

                return properties.AsReadOnly();
            }
        }

        public bool HasDeserializeError { get; protected set; }


        public static Type FindType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                return null;
            if (cachedTypes == null)
                cachedTypes = new Dictionary<string, Type>();
            Type t;
            if (!cachedTypes.TryGetValue(typeName, out t))
            {
                t = typeName.GetTypeInAllAssemblies(false);
                cachedTypes[typeName] = t;
                return t;
            }
            return t;
        }

        public static FieldInfo[] GetSerializeFields(Type type)
        {
            if (cachedFields == null)
                cachedFields = new Dictionary<Type, FieldInfo[]>();
            FieldInfo[] fields;
            if (cachedFields.TryGetValue(type, out fields))
                return fields;


            fields = null;
            List<FieldInfo> list = null;
            list = new List<FieldInfo>();

            Type parent = type;
            while (parent != null)
            {
                if (cachedFields.ContainsKey(parent))
                {
                    var tmp = cachedFields[parent];
                    if (tmp != null)
                    {
                        list.AddRange(tmp.Reverse());
                    }
                    break;
                }

                foreach (var field in parent.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
                {
                    if (field.DeclaringType != parent)
                        continue;
                    if (!field.IsDefined(typeof(SerializeField), true))
                        continue;
                    if (!list.Contains(field))
                    {
                        list.Add(field);
                    }
                }
                parent = parent.BaseType;
            }

            if (list != null && list.Count > 0)
            {
                list.Reverse();
                fields = list.ToArray();
            }
            cachedFields[type] = fields;
            return fields;
        }

        private PropertyData GetPropertyData(string name)
        {
            PropertyData pData = null;
            if (properties != null)
            {
                foreach (var prop in properties)
                {
                    if (prop.field == name)
                    {
                        pData = prop;
                        break;
                    }
                }
            }
            return pData;
        }


        public void OnAfterDeserialize()
        {
            //Debug.Log(GetType().Name + ".OnAfterDeserialize ");
            node = null;
            var props = this.properties;
            HasDeserializeError = false;
            Type type = null;
            if (!string.IsNullOrEmpty(typeName))
            {
                type = FindType(typeName);
                if (type != null)
                {
                    try
                    {
                        node = Activator.CreateInstance(type) as FlowNode;
                    }
                    catch (Exception ex)
                    {
                        HasDeserializeError = true;
                        Debug.LogException(ex);
                    }
                }
                else
                {
                    HasDeserializeError = true;
                    Debug.LogError("Not Found Type:" + typeName);
                }
            }

            if (node == null || HasDeserializeError)
                return;
            //node.ID = id;
            if (props != null)
            {
                var fields = GetSerializeFields(type);

                if (fields != null)
                {
                    foreach (var propData in props)
                    {
                        var field = fields.Where(o => o.Name == propData.field).FirstOrDefault();

                        if (field == null)
                            continue;
                        field.SetValue(node, propData.value.Value);
                    }
                }

            }
            node.OnAfterDeserialize();

            if (node.HasDeserializeError)
            {
                HasDeserializeError = true;
            }

        }



        public void OnBeforeSerialize()
        {
            //Debug.Log(GetType().Name + ".OnBeforeSerialize");
            if (node != null)
            {

                node.OnBeforeSerialize();
                var type = node.GetType();
                this.typeName = type.FullName;
                if (properties != null)
                    properties.Clear();

                var fields = GetSerializeFields(type);
                if (fields != null)
                {
                    foreach (var field in fields)
                    {

                        SerializeField(field);
                    }
                }

            }

        }

        void SerializeField(FieldInfo field)
        {
            PropertyData propData = new PropertyData();
            propData.field = field.Name;
            object value = field.GetValue(node);


            propData.value = new SerializableValue(field.FieldType) { Value = value };

            if (properties == null)
                properties = new List<PropertyData>();

            properties.Add(propData);
        }

        public override string ToString()
        {
            if (node == null)
                return "node null";
            return node.ToString();
        }



        [Serializable]
        public class PropertyData
        {
            public string field;
            public SerializableValue value;
            public override string ToString()
            {
                return string.Format("{0}={1}", field, value);
            }
        }
    }

}