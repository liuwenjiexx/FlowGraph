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
         
    }



}