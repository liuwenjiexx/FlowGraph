using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

namespace FlowGraph.Model
{
    public abstract class EventNode : MemberNode
    {
        private ValuePort thisIn;

        public EventNode()
        {
        }

        public EventNode(EventInfo @event)
            : base(@event)
        {
        }

        public EventInfo Event
        {
            get { return Member as EventInfo; }
        }

        protected ValuePort ThisIn
        {
            get { return thisIn; }
        }

        protected bool IsStatic
        {
            get
            {
                EventInfo evt = Event;
                if (evt != null)
                {
                    var rase = evt.GetRaiseMethod();
                    return rase.IsStatic;
                }
                return false;
            }
        }

        protected override void RegisterPorts()
        {
            base.RegisterPorts();
            EventInfo evt = Event;
            if (evt != null)
            {
                if (!IsStatic)
                    thisIn = AddValueInput(THIS, evt.DeclaringType);
            }
        }
         
    }

}