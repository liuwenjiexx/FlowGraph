using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using UnityEngine;
using System.Linq;

namespace FlowGraph.Model
{


    public abstract class FlowNode : ISerializationCallbackReceiver
    {

        private List<ValuePort> valueInputs;
        private List<ValuePort> valueOutputs;
        private ReadOnlyCollection<ValuePort> valueInputsReadOnly;
        private ReadOnlyCollection<ValuePort> valueOutputsReadOnly;

        private List<FlowInput> flowInputs;
        private List<FlowOutput> flowOutputs;

        private ReadOnlyCollection<FlowInput> flowInputsReadOnly;
        private ReadOnlyCollection<FlowOutput> flowOutputsReadOnly;


        private static MethodInfo cachedAddValueInputGenericMethod;
        private static MethodInfo cachedAddValueOutputGenericMethod;
        private Dictionary<Type, MethodInfo> cachedTypeMethods;

        public const string FLOW_IN = "In";
        public const string FLOW_OUT = "Out";
        public const string FLOW_ON = "On";
        public const string VALUE = "value";
        public const string THIS = "this";
        public const string TRIGGER = "Trigger";
        public const string EVENT = "Event";
        public const string ARGUMENT1 = "arg1";
        public const string ARGUMENT2 = "arg2";
        public const string ARGUMENT3 = "arg3";
        public const string ARGUMENT4 = "arg4";
        public const string ARGUMENT5 = "arg5";

        public const string ActionsCategory = "Actions";
        public const string FunctionsCategory = "Functions";
        public const string EventsCategory = "Events";
        public const string GamesCategory = "Games";
        public const string PlayersCategory = "Players";
        public const string CamerasCategory = "Cameras";
        public const string InputsCategory = "Inputs";
        public const string ItemsCategory = "Items";
        public const string EnvironmentsCategory = "Environments";

        public const string UnityCategory = "Unity";
        public const string UnityObjectCategory = UnityCategory + "/Object";
        public const string UnityGameObjectCategory = UnityCategory + "/GameObject";
        public const string UnityComponentCategory = UnityCategory + "/Componet";
        public const string UnityTransformCategory = UnityCategory + "/Transform";



        public FlowNode()
        {
            valueInputs = new List<ValuePort>();
            valueOutputs = new List<ValuePort>();
            flowInputs = new List<FlowInput>();
            flowOutputs = new List<FlowOutput>();

            RegisterPorts();
        }


        public FlowNodeStaus Status { get; protected set; }

        protected bool IsRunning
        {
            get { return Status == FlowNodeStaus.Waiting; }
        }

        public ReadOnlyCollection<FlowInput> FlowInputs
        {
            get
            {
                if (flowInputsReadOnly == null)
                    flowInputsReadOnly = flowInputs.AsReadOnly();
                return flowInputsReadOnly;
            }
        }

        public ReadOnlyCollection<FlowOutput> FlowOutputs
        {
            get
            {
                if (flowOutputsReadOnly == null)
                    flowOutputsReadOnly = flowOutputs.AsReadOnly();
                return flowOutputsReadOnly;
            }
        }


        public ReadOnlyCollection<ValuePort> ValueInputs
        {
            get
            {
                if (valueInputsReadOnly == null)
                    valueInputsReadOnly = valueInputs.AsReadOnly();
                return valueInputsReadOnly;
            }
        }

        public ReadOnlyCollection<ValuePort> ValueOutputs
        {
            get
            {
                if (valueOutputsReadOnly == null)
                    valueOutputsReadOnly = valueOutputs.AsReadOnly();
                return valueOutputsReadOnly;
            }
        }

        public bool HasDeserializeError { get; protected set; }

        public virtual bool IsReusable
        {
            get { return true; }
        }


        /// <summary>
        /// 注册端口名称应该固定，而不是动态变量
        /// </summary>
        protected virtual void RegisterPorts()
        {
            ClearAllPort();
            AddFlowOutput(FLOW_OUT);
            AddFlowInput(FLOW_IN);
        }


        public void ResetPorts()
        {
            ClearAllPort();
            RegisterPorts();
        }


        protected void ClearAllPort()
        {
            flowInputs.Clear();
            flowOutputs.Clear();
            valueInputs.Clear();
            valueOutputs.Clear();
        }

        protected FlowInput AddFlowInput(string name)
        {
            FlowInput input = new FlowInput(this, name);
            if (flowInputs == null)
                flowInputs = new List<FlowInput>();
            flowInputs.Add(input);
            return input;
        }

        public FlowInput GetFlowInput(string name)
        {
            foreach (var input in flowInputs)
            {
                if (input.Name == name)
                    return input;
            }
            return null;
        }
        protected FlowOutput AddFlowOutput(string name)
        {
            FlowOutput output = new FlowOutput(this, name);

            if (flowOutputs == null)
                flowOutputs = new List<FlowOutput>();
            flowOutputs.Add(output);

            return output;
        }

        public FlowOutput GetFlowOutput(string name)
        {
            foreach (var output in flowOutputs)
            {
                if (output.Name == name)
                    return output;
            }
            return null;
        }

        protected ValuePort<T> AddValueInput<T>(string name, IDefaultValue defaultValue = null)
        {
            ValuePort<T> port = new ValuePort<T>(this, name, defaultValue);

            if (valueInputs == null)
                valueInputs = new List<ValuePort>();
            valueInputs.Add(port);
            return port;
        }
        protected ValuePort<T> AddValueOutput<T>(string name)
        {
            ValuePort<T> port = new ValuePort<T>(this, name);

            if (valueOutputs == null)
                valueOutputs = new List<ValuePort>();
            valueOutputs.Add(port);

            return port;
        }



        //public IEnumerable<string> EnumerateValueInputNames()
        //{
        //    if (valueInputs == null)
        //        yield break;
        //    foreach (var item in valueInputs)
        //        yield return item.Name;
        //}
        public ValuePort GetValueInput(string name)
        {
            if (valueInputs != null)
            {
                foreach (var item in valueInputs)
                    if (name == item.Name)
                        return item;
            }
            return null;
        }
        //public IEnumerable<string> EnumerateValueOutputNames()
        //{
        //    if (valueOutputs == null)
        //        yield break;
        //    foreach (var item in valueOutputs)
        //        yield return item.Name;
        //}
        public ValuePort GetValueOutput(string name)
        {
            if (valueOutputs != null)
            {
                foreach (var item in valueOutputs)
                    if (name == item.Name)
                        return item;
            }
            return null;
        }
        public ValuePort<T> GetValueInput<T>(int index)
        {
            return (ValuePort<T>)valueInputs[index];
        }

        public ValuePort<T> GetValueOutput<T>(int index)
        {
            return (ValuePort<T>)valueOutputs[index];
        }


        public Exception Exception { get; set; }




        public virtual void ExecuteContent(Flow flow)
        {

        }


        public virtual void Flow(Flow flow)
        {
            var flowOut = GetFlowOutput(FLOW_OUT);
            if (flowOut != null)
                flowOut.Execute(flow);
        }


        public virtual void OnGraphStarted(ExecutionContext context)
        {
        }

        public virtual void OnGraphStoped(ExecutionContext context)
        {
        }


        protected ValuePort AddValueInput(string name, Type type, IDefaultValue defaultValue = null)
        {
            if (cachedAddValueInputGenericMethod == null)
            {
                foreach (var mInfo in typeof(FlowNode).GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
                {
                    if (mInfo.Name == "AddValueInput" && mInfo.IsGenericMethod && mInfo.GetParameters().Length == 2)
                    {
                        cachedAddValueInputGenericMethod = mInfo;
                        break;
                    }
                }
            }
            var m = cachedAddValueInputGenericMethod.MakeGenericMethod(type);
            return (ValuePort)m.Invoke(this, new object[] { name, defaultValue });
        }

        protected ValuePort AddValueOutput(string name, Type type)
        {
            if (cachedAddValueOutputGenericMethod == null)
            {
                foreach (var mInfo in typeof(FlowNode).GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
                {
                    if (mInfo.Name == "AddValueOutput" && mInfo.IsGenericMethod && mInfo.GetParameters().Length == 1)
                    {
                        cachedAddValueOutputGenericMethod = mInfo;
                        break;
                    }
                }
            }
            var m = cachedAddValueOutputGenericMethod.MakeGenericMethod(type);
            return (ValuePort)m.Invoke(this, new object[] { name });
        }

        protected void RegisterNodeMethodPorts(MethodInfo method, bool hasReturn, string returnOutputName = null)
        {
            RegisterMethodPorts(method, true, hasReturn, returnOutputName);
        }
        protected void RegisterNodeMethodPorts(string methodName)
        {
            RegisterNodeMethodPorts(methodName, true);
        }

        protected void RegisterNodeMethodPorts(string methodName, bool hasReturn, string returnOutputName = null)
        {
            if (methodName == null) throw new ArgumentNullException("methodName");


            Type type = GetType();
            MethodInfo method;

            if (cachedTypeMethods == null)
                cachedTypeMethods = new Dictionary<Type, MethodInfo>();
            if (!cachedTypeMethods.TryGetValue(type, out method))
            {
                method = type.GetMethod(methodName);
                cachedTypeMethods[type] = method;
            }

            if (method == null)
                throw new ArgumentNullException(string.Format("not found method. type:{0} method:{1}", type.FullName, methodName));

            RegisterMethodPorts(method, true, hasReturn, returnOutputName);
        }

        protected void RegisterMethodPorts(MethodInfo method, bool hasReturn, string returnOutputName = null)
        {
            RegisterMethodPorts(method, method.IsStatic, hasReturn, returnOutputName);
        }
        protected void RegisterMethodPorts(MethodInfo method, bool igoreInstance, bool hasReturn, string returnOutputName = null)
        {

            if (!method.IsStatic && !igoreInstance)
            {
                var thisType = method.DeclaringType;
                var thisIn = AddValueInput(THIS, thisType, DefaultValue.This);
                thisIn.Flags = ValuePortFlags.This;
            }

            if (hasReturn && method.ReturnType != typeof(void))
            {

                var nameAttr = method.ReturnParameter.GetCustomAttributes(true).FirstOrDefault() as NameAttribute;
                if (nameAttr != null && !string.IsNullOrEmpty(nameAttr.Name))
                    returnOutputName = nameAttr.Name;

                if (string.IsNullOrEmpty(returnOutputName))
                    returnOutputName = GetValueTypeName(method.ReturnType, VALUE);

                AddValueOutput(returnOutputName, method.ReturnType);
            }

            IDefaultValue defaultValue;
            ValuePortFlags flags;
            ValuePort port;
            foreach (var pInfo in method.GetParameters())
            {

                defaultValue = null;
                flags = ValuePortFlags.None;

                if (pInfo.IsDefined(typeof(HiddenMenuAttribute), false))
                    flags |= ValuePortFlags.Hidden;

                var injectAttr = pInfo.GetCustomAttributes(typeof(ValueAttribute), false).FirstOrDefault() as ValueAttribute;
                if (injectAttr != null)
                {
                    defaultValue = injectAttr.GetDefaultValue();
                }

                if (pInfo.IsRef())
                {
                    port = AddValueInput(pInfo.Name, pInfo.ParameterType, defaultValue);
                    port.Flags = flags;
                    port = AddValueOutput(pInfo.Name, pInfo.ParameterType);
                    port.Flags = flags;
                }
                else if (pInfo.IsOut)
                {
                    port = AddValueOutput(pInfo.Name, pInfo.ParameterType);
                    port.Flags = flags;
                }
                else
                {
                    port = AddValueInput(pInfo.Name, pInfo.ParameterType, defaultValue);
                    port.Flags = flags;
                }


            }
        }

        internal void DiryAllValueInputNodes(Flow flow)
        {
            if (flow != null)
            {
                if (flow.FindFlow(this) != null)
                    return;
            }

            //if (Status != FlowNodeStaus.Waiting)
            {
                Status = FlowNodeStaus.Executing;
            }

            foreach (var input in this.valueInputs)
            {
                var output = input.LinkOutput;
                if (output != null)
                {

                    if (!(output.Node.Status == FlowNodeStaus.Executing /*|| output.Node.Status == FlowNodeStaus.Waiting*/))
                    {
                        output.Node.DiryAllValueInputNodes(flow);
                    }
                }
            }

        }


        internal void ExecuteAllValueInputNode(ExecutionContext context, FlowNode node, Flow flow)
        {
            if (node.Status != FlowNodeStaus.Executing)
                return;

            foreach (var input in node.ValueInputs)
            {
                var output = input.LinkOutput;
                if (output != null)
                {

                    var f = flow.FindFlow(output.Node);
                    if (f != null)
                    {
                        output.SetValue(f.GetOutput(output));
                    }
                    else
                    {
                        if (output.Node.Status == FlowNodeStaus.Executing)
                        {
                            ExecuteAllValueInputNode(context, output.Node, flow);
                            if (output.Node.Status == FlowNodeStaus.Error)
                            {
                                node.Status = FlowNodeStaus.Error;
                                break;
                            }
                        }
                    }

                }
            }

            if (node.Status == FlowNodeStaus.Executing)
            {

                try
                {
                    using (new UnityEngine.Profiling.ProfilerSample("FlowGraph.Node."+node.GetType().Name))
                    {
                        node.ExecuteContent(flow);
                    }
                    if (node.Status == FlowNodeStaus.Executing)
                        node.Status = FlowNodeStaus.Complete;
                }
                catch (Exception ex)
                {
                    node.Exception = ex;
                    node.Status = FlowNodeStaus.Error;
                    Debug.LogException(ex);
                }
            }
        }

        public static object ChangeType(object value, Type type)
        {
            if (value != null)
            {
                Type valueType = value.GetType();
                if (!type.IsAssignableFrom(valueType))
                {
                    if (type == typeof(GameObject))
                    {
                        if (value is Component)
                            value = ((Component)value).gameObject;
                    }
                    else if (typeof(Component).IsAssignableFrom(type))
                    {
                        if (value is GameObject)
                            value = ((GameObject)value).GetComponent(type);
                        else if (value is Component)
                        {
                            value = ((Component)value).GetComponent(type);
                        }
                    }
                    if (value != null)
                    {
                        valueType = value.GetType();
                        if (!type.IsAssignableFrom(valueType))
                        {
                            value = Convert.ChangeType(value, type);
                        }
                    }
                }
            }
            else
            {
                value = type.CreateDefaultValue();
            }
            return value;
        }

        protected void SetDeserializeError(string errorMsg = null)
        {
            HasDeserializeError = true;
            throw new Exception(errorMsg);
        }
        public virtual void OnBeforeSerialize()
        {

        }

        public virtual void OnAfterDeserialize()
        {

        }



        #region Type Names

        public const string Type_Int8 = "byte";
        public const string Type_Int16 = "short";
        public const string Type_Int32 = "int";
        public const string Type_Int64 = "long";
        public const string Type_Float32 = "float";
        public const string Type_Float64 = "double";
        public const string Type_String = "string";
        public const string Type_Bool = "bool";
        public const string Type_UnityObject = "Object";
        public const string Type_Object = "object";
        public const string Type_Vector2 = "vector2";
        public const string Type_Vector2Int = "vector2Int";
        public const string Type_Vector3 = "vector3";
        public const string Type_Vector3Int = "vector3Int";
        public const string Type_Vector4 = "vector4";
        public const string Type_Color = "color";
        public const string Type_Color32 = "color32";
        public const string Type_AnimationCurve = "curve";
        public const string Type_Quaternion = "quaternion";
        #region Components

        public const string Type_Transform = "transform";
        public const string Type_GameObject = "gameObject";
        public const string Type_Component = "component";
        public const string Type_Collider = "collider";
        public const string Type_Camera = "camera";
        #endregion

        private static Dictionary<Type, string> type_names;



        public static string GetValueTypeName(Type type, string defaultName = null)
        {

            if (type_names == null)
            {
                type_names = new Dictionary<Type, string>();

                type_names[typeof(byte)] = Type_Int8;
                type_names[typeof(short)] = Type_Int16;
                type_names[typeof(int)] = Type_Int32;
                type_names[typeof(long)] = Type_Int64;
                type_names[typeof(float)] = Type_Float32;
                type_names[typeof(double)] = Type_Float64;
                type_names[typeof(string)] = Type_String;
                type_names[typeof(bool)] = Type_Bool;
                type_names[typeof(object)] = Type_Object;
                type_names[typeof(UnityEngine.Object)] = Type_UnityObject;
                type_names[typeof(Vector2)] = Type_Vector2;
                type_names[typeof(Vector2Int)] = Type_Vector2Int;
                type_names[typeof(Vector3)] = Type_Vector3;
                type_names[typeof(Vector3Int)] = Type_Vector3Int;
                type_names[typeof(Vector4)] = Type_Vector4;
                type_names[typeof(Color)] = Type_Color;
                type_names[typeof(Color32)] = Type_Color32;
                type_names[typeof(AnimationCurve)] = Type_AnimationCurve;
                type_names[typeof(Transform)] = Type_Transform;
                type_names[typeof(GameObject)] = Type_GameObject;
                type_names[typeof(Component)] = Type_Component;
                type_names[typeof(Collider)] = Type_Collider;
                type_names[typeof(Camera)] = Type_Camera;

            }

            string name;
            if (type_names.TryGetValue(type, out name))
                return name;

            name = type.Name[0].ToString().ToLower() + type.Name.Substring(1);
            type_names[type] = name;
            return name;
            //return string.IsNullOrEmpty(defaultName) ? VALUE : defaultName;
        }
        public static string GetDisplayValueTypeName(Type type, string defaultName = null)
        {
            string name = GetValueTypeName(type, defaultName);
            return name[0].ToString().ToUpper() + name.Substring(1);
        }
        #endregion



        public string GetNodeTypeDisplayName()
        {
            string name = null;


            name = GetType().Name;
            if (name.EndsWith("Node"))
                name = name.Substring(0, name.Length - 4);

            return name;
        }


        public static bool CanInjectType(Type type)
        {
            if (type == typeof(GameObject))
                return true;
            else if (type == typeof(ExecutionContext))
                return true;
            else if (typeof(Component).IsAssignableFrom(type))
                return true;
            return false;
        }

        public static IDefaultValue GetDefaultInject(Type type)
        {
            return DefaultValue.This;
        }

    }


    public enum FlowNodeStaus
    {
        None,
        Executing,
        Waiting,
        Complete,
        Error
    }



}