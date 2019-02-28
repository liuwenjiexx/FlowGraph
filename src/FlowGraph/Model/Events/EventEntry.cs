using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{

    public class EventEntry
    {
        private ExecutionContext context;
        private FlowNode node;

        public EventEntry(FlowNode node, ExecutionContext context)
        {
            this.context = context;
            this.node = node;
        }


        public ExecutionContext Context
        {
            get { return context; }
        }

        public FlowNode Node
        {
            get { return node; }
        }


        public static void Trigger(List<EventEntry> entries, string eventId)
        {
            Trigger(entries, eventId, null, null);
        }

        public static void Trigger(List<EventEntry> entries, string eventId, string argName, object arg)
        {
            foreach (var entry in entries)
            {
                if (entry.node != null)
                {
                    if (!string.IsNullOrEmpty(argName))
                    {
                        var valueOut = entry.node.GetValueOutput(argName);
                        if (valueOut != null)
                        {
                            valueOut.SetValue( arg);
                        }
                    }
                    var flowOutput = entry.node.GetFlowOutput(eventId);
                    if (flowOutput != null)
                    {
                        flowOutput.Execute(entry.context);
                    }
                }
            }
        }

        public static void Add(List<EventEntry> entries, EventEntry entry)
        {
            entries.Add(entry);
        }
        public static void Remove(List<EventEntry> entries, EventEntry entry)
        {
            entries.Remove(entry);
            //for (int i = 0; i < entries.Count; i++)
            //{
            //    if (entries[i].Controller == controller)
            //    {
            //        entries.RemoveAt(i);
            //        i--;
            //    }
            //}
        }
        public static void Remove(List<EventEntry> entries, ExecutionContext context)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].context == context)
                {
                    entries.RemoveAt(i);
                    i--;
                }
            }
        }
    }

}
