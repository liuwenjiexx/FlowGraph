using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{

    [Name("Collision Event")]
    [Category(EventsCategory)]
    public class CollisionEvent : FlowNode
    {
        public override bool IsReusable
        {
            get { return false; }
        }

        protected override void RegisterPorts()
        {
            AddFlowOutput("Enter");
            AddFlowOutput("Exit");
            AddValueOutput<Collider>("collider");
        }

        public override void OnGraphStarted(ExecutionContext g)
        {
            var trigger = g.GameObject.EnsureComponent<CollisionEventBehaviour>();
            var entry = new EventEntry(this, g);
            EventEntry.Add(trigger.entries, entry);

            base.OnGraphStarted(g);
        }


        public override void OnGraphStoped(ExecutionContext g)
        {
            var trigger = g.GameObject.GetComponent<CollisionEventBehaviour>();
            if (trigger)
                EventEntry.Remove(trigger.entries, g);
            base.OnGraphStoped(g);
        }

        class CollisionEventBehaviour : MonoBehaviour
        {
            public List<EventEntry> entries = new List<EventEntry>();

            private void OnCollisionEnter(Collision collision)
            {
                EventEntry.Trigger(entries, "Enter", "collision", collision);
            }

            private void OnCollisionExit(Collision collision)
            {
                EventEntry.Trigger(entries, "Exit", "collision", collision);
            }
        }
    }


}