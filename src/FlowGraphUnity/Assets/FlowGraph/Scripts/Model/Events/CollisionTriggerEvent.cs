using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FlowGraph.Model
{

    [Name("Collision Trigger")]
    [Category(EventsCategory)]
    public class CollisionTriggerEvent : FlowNode
    {

        protected override void RegisterPorts()
        {
            AddFlowOutput("Enter");
            AddFlowOutput("Exit");
            AddValueOutput<Collider>("collider");
        }

        public override void OnGraphStarted(ExecutionContext g)
        {
            var trigger = g.GameObject.EnsureComponent<TriggerEventBehaviour>();
            var entry = new EventEntry(this, g);
            EventEntry.Add(trigger.entries, entry);

            base.OnGraphStarted(g);
        }


        public override void OnGraphStoped(ExecutionContext g)
        {
            var trigger = g.GameObject.GetComponent<TriggerEventBehaviour>();
            if (trigger)
                EventEntry.Remove(trigger.entries, g);
            base.OnGraphStoped(g);
        }

        class TriggerEventBehaviour : MonoBehaviour
        {
            public List<EventEntry> entries = new List<EventEntry>();
             
            private void OnTriggerEnter(Collider other)
            {
                EventEntry.Trigger(entries, "Enter", "collider", other);
            }

            private void OnTriggerExit(Collider other)
            {
                EventEntry.Trigger(entries, "Exit", "collider", other);
            }
        }
    }


}