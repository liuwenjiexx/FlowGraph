using System;
using System.Reflection;
using UnityEngine;

namespace FlowGraph.Model
{
    public abstract class MemberNode : FlowNode
    {
        [HideInInspector]
        [SerializeField]
        protected string typeName;

        [HideInInspector]
        [SerializeField]
        protected string memberName;

        private MemberInfo member;

        public MemberNode()
        {
        }

        public MemberNode(MemberInfo member)
        {
            if (member == null)
                throw new ArgumentNullException("member");
            this.member = member;
            ResetPorts();
        }

        public MemberInfo Member
        {
            get { return member; }
        }

        public override void OnBeforeSerialize()
        {
            if (member != null)
            {
                this.typeName = member.DeclaringType.FullName;
                this.memberName = GetMemberName(member);
            }
            base.OnBeforeSerialize();
        }

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            if (HasDeserializeError)
                return;

            member = null;
            Type type = null;

            if (!string.IsNullOrEmpty(typeName))
            {
                type = FlowNodeData.FindType(typeName);
            }
            if (type == null)
            {
                SetDeserializeError("not found type. type:" + typeName);
                return;
            }
            if (!string.IsNullOrEmpty(memberName))
                member = GetMember(type, memberName);
            if (member == null)
            {
                SetDeserializeError("not found member. field:" + memberName);
                return;
            }
            ResetPorts();
        }
        protected virtual string GetMemberName(MemberInfo member)
        {
            return member.Name;
        }
        protected virtual MemberInfo GetMember(Type type, string memberName)
        {
            var ms = type.GetMember(memberName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
            if (ms.Length > 0)
                return ms[0];
            return null;
        }

      

    }

}