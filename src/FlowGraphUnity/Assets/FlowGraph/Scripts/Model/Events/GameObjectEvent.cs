using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{

    [Name("GameObject")]
    [Category(EventsCategory)]
    public class GameObjectEvent : FlowNode
    {
        protected override void RegisterPorts()
        {
            AddFlowOutput("Enable");
            AddFlowOutput("Disable");
            AddFlowOutput("Destroy");
        }
        public override void OnGraphStarted(ExecutionContext g)
        {
            var trigger = g.GameObject.EnsureComponent<GameObjectEventBehaviour>();
            var entry = new EventEntry(this, g);
            EventEntry.Add(trigger.entries, entry);
            base.OnGraphStarted(g);
        }


        public override void OnGraphStoped(ExecutionContext g)
        {
            var trigger = g.GameObject.GetComponent<GameObjectEventBehaviour>();
            if (trigger)
                EventEntry.Remove(trigger.entries, g);
            base.OnGraphStoped(g);
        }
    }
    [Name("Update")]
    [Category(EventsCategory)]
    public class UpdateEvent : FlowNode
    {
        protected override void RegisterPorts()
        {
            AddFlowOutput("Update");
        }
        public override void OnGraphStarted(ExecutionContext g)
        {
            var trigger = g.GameObject.EnsureComponent<GameObjectEventBehaviour>();
            var entry = new EventEntry(this, g);
            EventEntry.Add(trigger.entries, entry);
            base.OnGraphStarted(g);
        }


        public override void OnGraphStoped(ExecutionContext g)
        {
            var trigger = g.GameObject.GetComponent<GameObjectEventBehaviour>();
            if (trigger)
                EventEntry.Remove(trigger.entries, g);
            base.OnGraphStoped(g);
        }
    }

    [Name("LateUpdate")]
    [Category(EventsCategory)]
    public class LateUpdateEvent : FlowNode
    {
        protected override void RegisterPorts()
        {
            AddFlowOutput("LateUpdate");
        }
        public override void OnGraphStarted(ExecutionContext g)
        {
            var trigger = g.GameObject.EnsureComponent<GameObjectEventBehaviour>();
            var entry = new EventEntry(this, g);
            EventEntry.Add(trigger.entries, entry);
            base.OnGraphStarted(g);
        }


        public override void OnGraphStoped(ExecutionContext g)
        {
            var trigger = g.GameObject.GetComponent<GameObjectEventBehaviour>();
            if (trigger)
                EventEntry.Remove(trigger.entries, g);
            base.OnGraphStoped(g);
        }
    }


    class GameObjectEventBehaviour : MonoBehaviour
    {
        public List<EventEntry> entries = new List<EventEntry>();

        private void OnEnable()
        {
            EventEntry.Trigger(entries, "Enable");
        }

        private void OnDisable()
        {
            EventEntry.Trigger(entries, "Disable");
        }

        private void OnDestroy()
        {
            EventEntry.Trigger(entries, "Destroy");
        }

        private void Update()
        {
            EventEntry.Trigger(entries, "Update");
        }
        private void LateUpdate()
        {
            EventEntry.Trigger(entries, "LateUpdate");
        }
    }
}