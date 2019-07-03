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
        [SerializeField]
        private FlowAsset nodeAsset;
        [SerializeField]
        private bool isNodeAsset;

        private static Dictionary<Type, FieldInfo[]> cachedFields;
        private static Dictionary<string, Type> cachedTypes;

        public FlowNodeData(/*FlowGraphData graph, */FlowNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");
            New();
            this.node = node;
            isInitial = true;
            //  this.Graph = graph;
        }

        public FlowNodeData(/*FlowGraphData graph,*/ FlowAsset nodeAsset)
        {
            if (nodeAsset == null)
                throw new ArgumentNullException("nodeAsset");
            this.nodeAsset = nodeAsset;
            New();
            //   this.Graph = graph;
            node = nodeAsset.CreateFlowNode();
            this.isNodeAsset = true;
            isInitial = true;
        }


        public FlowNodeData()
        {

        }

        private void New()
        {
            this.id = System.Guid.NewGuid().ToString("N");

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

        public FlowAsset NodeAsset
        {
            get { return nodeAsset; }
            set { nodeAsset = value; }
        }


        public bool IsNodeAsset
        {
            get { return nodeAsset; }
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
        [NonSerialized]
        private FlowGraphData graph;
        public FlowGraphData Graph { get { return graph; } internal set { graph = value; } }


        public bool HasDeserializeError { get; protected set; }
        [NonSerialized]
        private bool IsDeserialize;
        [NonSerialized]
        private bool isInitial;

        public void Initial(FlowGraphData graph)
        {
            if (isInitial)
                return;
            if (!IsDeserialize)
                return;
            isInitial = true;
            IsDeserialize = false;
            this.graph = graph;
            node = CreateFlowNode();
        }


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

        public void OnBeforeSerialize()
        {
            //Debug.Log(GetType().Name + ".OnBeforeSerialize");
            if (IsDeserialize && !isInitial)
            {
                return;
            }

            if (!isInitial || HasDeserializeError)
                return;

            if (!isNodeAsset)
            {

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

                            PropertyData propData = new PropertyData();
                            propData.field = field.Name;
                            object value = field.GetValue(node);

                            //Debug.Log("SerializeField:" + field+", type:"+field.DeclaringType);
                            propData.value = new SerializableValue(field.FieldType) { Value = value };

                            if (properties == null)
                                properties = new List<PropertyData>();

                            properties.Add(propData);
                        }
                    }

                }
            }

            //Debug.Log(GetType().Name + ".OnBeforeSerialize end");
        } 
         
        public void OnAfterDeserialize()
        {
            //Debug.Log(GetType().Name + ".OnAfterDeserialize  trace:" +FlowGraphData. N);
            node = null;
            HasDeserializeError = false;
            IsDeserialize = true;
            isInitial = false;

            if (!isNodeAsset)
            {


            }
            else
            {
                if (nodeAsset)
                {
                    //ScriptableObject no ready data
                    //node = nodeAsset.CreateFlowNode();
                }
                else
                {
                    //HasDeserializeError = true;
                }
            }

            //Debug.Log(GetType().Name + ".OnAfterDeserialize end trace:" + FlowGraphData.N);
        }

        public FlowNode CreateFlowNode()
        {
            FlowNode node = null;
            if (isNodeAsset)
            {
                if (nodeAsset)
                {
                    node = nodeAsset.CreateFlowNode();
                }
            }
            else
            {
                Type type = null;

                if (string.IsNullOrEmpty(typeName))
                {
                    Debug.LogError("type name null");
                    return null;
                }

                type = FindType(typeName);
                if (type == null)
                {
                    HasDeserializeError = true;
                    Debug.LogError("Not Found Type:" + typeName);
                    return null;
                }

                try
                {
                    node = Activator.CreateInstance(type) as FlowNode;

                    var props = this.properties;
                    //node.ID = id;
                    if (props != null)
                    {
                        var fields = GetSerializeFields(type);

                        if (fields != null)
                        {
                            object value;
                            foreach (var propData in props)
                            {
                                var field = fields.Where(o => o.Name == propData.field).FirstOrDefault();

                                if (field == null)
                                    continue;
                                value = propData.value.Value;
                                if (propData.value.Value == null)
                                    value = null;
                                field.SetValue(node, value);
                            }
                        }

                    }

                    node.OnAfterDeserialize();

                    if (node.HasDeserializeError)
                    {
                        HasDeserializeError = true;
                    }

                }
                catch (Exception ex)
                {
                    HasDeserializeError = true;
                    Debug.LogException(ex);
                }

            }

            return node;
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