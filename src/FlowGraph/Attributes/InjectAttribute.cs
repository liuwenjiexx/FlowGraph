using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace FlowGraph.Model
{

    [AttributeUsage(AttributeTargets.Parameter)]
    public class InjectAttribute : Attribute
    {
        public InjectAttribute()
        {
        }

        public InjectAttribute(Type type)
        {
            this.Type = type;
        }

        public InjectAttribute(string name)
        {
            this.Name = name;
        }

        public InjectAttribute(InjectValueType valueType)
        {
            this.ValueType = valueType;
        }

        public Type Type { get; set; }
        public string Name { get; set; }
        public InjectValueType ValueType { get; set; }
    }




}