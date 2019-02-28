using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;

namespace FlowGraph.Model
{
    public abstract class MethodNode : MemberNode
    {
        [SerializeField]
        private bool isCoroutine;

        public MethodNode()
        {
        }

        public MethodNode(MethodInfo method)
            : base(method)
        {
            isCoroutine = method.IsDefined(typeof(CoroutineMethodAttribute), true);

            ResetPorts();
        }
        public MethodNode(MethodInfo method, bool isCoroutine)
            : base(method)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            this.isCoroutine = isCoroutine;
            ResetPorts();
        }

        public MethodInfo Method
        {
            get { return Member as MethodInfo; }
        }

        public bool IsCoroutine
        {
            get { return isCoroutine; }
        }

        protected override void RegisterPorts()
        {
            base.RegisterPorts();
            var method = Method;
            if (method != null)
            {
                if (isCoroutine)
                    RegisterMethodPorts(method, false);
                else
                    RegisterMethodPorts(method, true);
            }
        }
       
        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            if (HasDeserializeError)
                return;
            var method = Method;
            if (method != null)
            {
                if (method.IsDefined(typeof(CoroutineMethodAttribute), true))
                    isCoroutine = true;
                ResetPorts();
            }
        }

        protected override string GetMemberName(MemberInfo member)
        {
            string memberName = member.ToString();
            return memberName;
        }
        protected override MemberInfo GetMember(Type type, string memberName)
        {
            if (!string.IsNullOrEmpty(memberName))
            {
                foreach (var m in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
                {
                    if (m.ToString() == memberName)
                    {
                        return m;
                    }
                }
            }
            return null;
        }

        public override string GetDisplayName()
        {
            var method = Method;
            if (method == null)
                return "missing(Method)";
            string name = method.Name;
            var nameAttr = method.GetCustomAttributes(typeof(NameAttribute), true).FirstOrDefault() as NameAttribute;
            if (nameAttr != null && !string.IsNullOrEmpty(nameAttr.Name))
                name = nameAttr.Name;
            return name;
        }

        public override string ToString()
        {
            var method = Method;
            if (method == null)
                return "MethodNode: null";
            return string.Format("MethodNode: {0}", method.ToString());
        }
    }

    [Hidden]
    [Name("Call Method")]
    [Category(ActionsCategory)]
    public class CallMethod : MethodNode
    {
        public CallMethod()
        {
        }

        public CallMethod(MethodInfo method)
            : base(method)
        {
        }

        public CallMethod(MethodInfo method, bool isCoroutine)
            : base(method, isCoroutine)
        {

        }

        public override void ExecuteContent(Flow flow)
        {
            object result = null;

            object[] args;
            object thisObj = null;
            var valueInputs = ValueInputs;

            var method = Method;
            var ps = method.GetParameters();
            args = new object[ps.Length];
            int argsIndex = method.IsStatic ? 0 : 1;
            if (method.IsStatic)
            {

            }
            else
            {
                var thisIn = ValueInputs[0];
                Type objType = method.DeclaringType;

                thisObj = valueInputs[0].GetValue(flow.Context, objType);

                if (thisObj == null)
                    throw new ArgumentNullException(valueInputs[0].Name);


            }

            for (int i = 0, j = method.IsStatic ? 0 : 1; i < ps.Length; i++)
            {
                var p = ps[i];
                if (p.IsOut)
                    continue;

                var input = GetValueInput(p.Name);
                if (input != null)
                    args[i] = input.GetValue(flow.Context, p.ParameterType);
            }


            result = method.Invoke(thisObj, args);


            if (IsCoroutine)
            {
                IEnumerator routine = result as IEnumerator;
                if (routine != null)
                {
                    Status = FlowNodeStaus.Waiting;
                    flow.Context.StartCoroutine(routine, () =>
                    {
                        Status = FlowNodeStaus.Complete;
                        Flow(flow);
                    });
                }
            }
            else
            {

                for (int i = 0; i < ps.Length; i++)
                {
                    var p = ps[i];
                    if (p.IsOut || p.IsRef())
                    {
                        GetValueOutput(p.Name).SetValue(args[i]);
                    }
                }

                if (method.ReturnType != typeof(void))
                    ValueOutputs[0].SetValue(result);
            }

        }







    }

}