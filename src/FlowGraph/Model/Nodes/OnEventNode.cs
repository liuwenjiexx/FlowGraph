using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{

    [Category(EventsCategory)]
    public abstract class OnEventNode : FlowNode
    {
        [SerializeField]
        private string eventName;

        public string EventName
        {
            get { return eventName; }
        }

        public override string GetDisplayName()
        {
            if (string.IsNullOrEmpty(eventName))
                return "null(OnEvent)";
            return string.Format("@{0}", eventName);
        }
    }



}