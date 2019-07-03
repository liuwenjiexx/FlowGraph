using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FlowGraph.Model
{

    public abstract class ValueNode<TValue> : FlowNode
    {
        [NonSerialized]
        [HideInInspector]
        //[SerializeField]
        public List<string> setMembers;

        public Type ValueType
        {
            get { return typeof(TValue); }
        }


        protected virtual string OutputName
        {
            get { return GetValueTypeName(ValueType); }
        }

        protected override void RegisterPorts()
        {
            AddValueOutput<TValue>(OutputName);

            if (setMembers != null)
            {
                var members = GetSetMembers(ValueType);
                foreach (var m in setMembers)
                {
                    MemberInfo mInfo;
                    if (!members.TryGetValue(m, out mInfo))
                    {
                        Debug.LogError("not found member :" + m + ", type:" + ValueType);
                        continue;
                    }
                    var field = mInfo as FieldInfo;
                    if (field != null)
                        AddValueInput(m, field.FieldType);
                    else
                    {
                        var pInfo = mInfo as PropertyInfo;
                        AddValueInput(m, pInfo.PropertyType);
                    }
                }

            }
        }

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            if (HasDeserializeError)
                return;
            ResetPorts();
        }

        public override void ExecuteContent(Flow flow)
        {
            object value = GetValue(flow);
            ValueOutputs[0].SetValue(value);
            if (setMembers != null && setMembers.Count > 0)
            {
                var members = GetSetMembers(ValueType);
                foreach (var member in setMembers)
                {
                    MemberInfo mInfo;
                    if (!members.TryGetValue(member, out mInfo))
                        continue;
                    var input = GetValueInput(member);
                    if (input != null)
                    {
                        object inputValue = input.GetValue(flow.Context);
                        if (mInfo.MemberType == MemberTypes.Field)
                        {
                            ((FieldInfo)mInfo).SetValue(value, inputValue);
                        }
                        else
                        {
                            ((PropertyInfo)mInfo).GetSetMethod().Invoke(value, new object[] { inputValue });
                        }
                    }
                }
            }
        }

        protected abstract TValue GetValue(Flow flow);


        internal void AddSetMember(string member)
        {
            if (setMembers == null)
                setMembers = new List<string>();
            if (setMembers.Contains(member))
                return;
            setMembers.Add(member);
        }

        internal void RemoveSetMember(string member)
        {
            if (setMembers == null)
                return;

            setMembers.Remove(member);
        }


        static Dictionary<Type, Dictionary<string, MemberInfo>> cachedSetMembers;



        public static Dictionary<string, MemberInfo> GetSetMembers(Type type)
        {

            if (cachedSetMembers == null)
                cachedSetMembers = new Dictionary<Type, Dictionary<string, MemberInfo>>();
            Dictionary<string, MemberInfo> members;
            if (!cachedSetMembers.TryGetValue(type, out members))
            {
                members = new Dictionary<string, MemberInfo>();
                BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetField;
                foreach (var fInfo in type.GetFields(bindingFlags))
                {
                    if (fInfo.IsInitOnly)
                        continue;
                    if (fInfo.IsDefined(typeof(NonSerializedAttribute), true))
                        continue;
                    if (!fInfo.IsPublic)
                    {
                        if (!fInfo.IsDefined(typeof(SerializeField), true))
                            continue;
                    }
                    members[fInfo.Name] = fInfo;
                }
                bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty;
                foreach (var pInfo in type.GetProperties(bindingFlags))
                {

                    var setter = pInfo.GetSetMethod();
                    if (setter == null)
                        continue;
                    if (!setter.IsPublic)
                        continue;

                    members[pInfo.Name] = pInfo;
                }
                cachedSetMembers[type] = members;
            }

            return members;

        }



    }


    [Serializable]
    public class SetMember
    {
        public string memberName;
        public Type memberType;
    }


}