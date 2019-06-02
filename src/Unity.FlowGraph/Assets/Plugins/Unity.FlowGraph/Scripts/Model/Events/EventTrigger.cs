using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using EvtTrigger = UnityEngine.EventSystems.EventTrigger;

namespace FlowGraph.Model
{

    [Name("Event Trigger")]
    [Category(EventsCategory)]
    public class EventTrigger : FlowNode
    {

        private List<EvtTrigger.Entry> entries;

        private static Array EventIds;

        protected override void RegisterPorts()
        {
            if (EventIds == null)
            {
                EventIds = Enum.GetValues(typeof(EventTriggerType));
            }

            foreach (EventTriggerType eventId in EventIds)
            {
                AddFlowOutput(eventId.ToString());
            }


        }

        public override void OnGraphStarted(ExecutionContext g)
        {
            var trigger = g.GameObject.EnsureComponent<EvtTrigger>();

            var init = new Action<EventTriggerType>((eventId) =>
            {
                var entry = new EvtTrigger.Entry();
                entry.eventID = eventId;
                var evt = new EvtTrigger.TriggerEvent();
                evt.AddListener((data) =>
                {
                    GetFlowOutput(eventId.ToString()).Execute(g);
                });
                entry.callback = evt;
                trigger.triggers.Add(entry);
                if (entries == null)
                    entries = new List<EvtTrigger.Entry>();
                entries.Add(entry);
            });

            foreach (EventTriggerType eventId in EventIds)
            {
                init(eventId);
            }

            base.OnGraphStarted(g);
        }

        public override void OnGraphStoped(ExecutionContext g)
        {
            var trigger = g.GameObject.GetComponent<EvtTrigger>();
            if (trigger && entries != null)
            {
                foreach (var entry in entries)
                {
                    trigger.triggers.Remove(entry);
                }
                entries.Clear();
            }
            base.OnGraphStoped(g);
        }


    }

}