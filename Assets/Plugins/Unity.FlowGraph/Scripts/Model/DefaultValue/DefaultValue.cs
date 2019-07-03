using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FlowGraph.Model
{

    public class DefaultValueAttribute : ValueAttribute
    {
        private object defaultValue;

        public DefaultValueAttribute(object defaultValue)
        {
            this.defaultValue = defaultValue;
        }

        public object DefaultValue
        {
            get { return defaultValue; }
        }

        public override IDefaultValue GetDefaultValue()
        {
            return new DefaultValue(defaultValue);
        }
    }

    public class DefaultValue : IDefaultValue
    {
        private object defaultValue;

        /// <summary>
        /// support some type: <see cref="GameObject"/> <see cref="IContext"/> <see cref="Component"/>
        /// </summary>
        public static readonly IDefaultValue This = new _ThisValue();
        //public static readonly IDefaultValue ThisGameObject = new _ThisGameObjectValue();
        //public static readonly IDefaultValue ThisTransform = new _ThisTransformValue();
        //public static readonly IDefaultValue ThisCollider = new _ThisColliderValue();
        //public static readonly IDefaultValue Context = new _ThisContextValue();
        public static readonly IDefaultValue UnscaledDeltaTime = new UnscaledDeltaTimeValue();
        public static readonly IDefaultValue DeltaTime = new DeltaTimeValue();
        public static readonly IDefaultValue FixedTime = new FixedTimeValue();

        public DefaultValue(object defaultValue)
        {
            this.defaultValue = defaultValue;
        }
        public object GetDefaultValue(ExecutionContext context, Type targetType)
        {
            return defaultValue;
        }

        public override string ToString()
        {
            if (defaultValue == null)
                return "(null)";
            return defaultValue.ToString();
        }
        class _ThisValue : IDefaultValue
        {
            public object GetDefaultValue(ExecutionContext context, Type targetType)
            {
                if (targetType != null)
                {
                    if (targetType == typeof(GameObject))
                        return context.GameObject;
                    if (targetType == typeof(Transform))
                        return context.GameObject.transform;
                    if (targetType.IsAssignableFrom(context.GetType()))
                        return context;
                    if (targetType.IsSubclassOf(typeof(Component)))
                        return context.GameObject.GetComponent(targetType);
                }

                return null;
            }
            public override string ToString()
            {
                return "This";
            }
        }
        //class _ThisGameObjectValue : IDefaultValue
        //{
        //    public object GetDefaultValue(ExecutionContext context, Type targetType)
        //    {
        //        return context.GameObject;
        //    }
        //    public override string ToString()
        //    {
        //        return "This GameObject";
        //    }
        //}
        //class _ThisTransformValue : IDefaultValue
        //{
        //    public object GetDefaultValue(ExecutionContext context, Type targetType)
        //    {
        //        return context.GameObject.transform;
        //    }
        //    public override string ToString()
        //    {
        //        return "This Transform";
        //    }
        //}
        //class _ThisColliderValue : IDefaultValue
        //{
        //    public object GetDefaultValue(ExecutionContext context, Type targetType)
        //    {
        //        return context.GameObject.GetComponent<Collider>();
        //    }
        //    public override string ToString()
        //    {
        //        return "This Collider";
        //    }
        //}
        //class _ThisContextValue : IDefaultValue
        //{
        //    public object GetDefaultValue(ExecutionContext context, Type targetType)
        //    {
        //        return context;
        //    }
        //    public override string ToString()
        //    {
        //        return "This Context";
        //    }
        //}

        class UnscaledDeltaTimeValue : IDefaultValue
        {
            public object GetDefaultValue(ExecutionContext context, Type targetType)
            {
                return Time.unscaledDeltaTime;
            }
            public override string ToString()
            {
                return "unscaledDeltaTime";
            }
        }
        class DeltaTimeValue : IDefaultValue
        {
            public object GetDefaultValue(ExecutionContext context, Type targetType)
            {
                return Time.deltaTime;
            }
            public override string ToString()
            {
                return "deltaTime";
            }
        }
        class FixedTimeValue : IDefaultValue
        {
            public object GetDefaultValue(ExecutionContext context, Type targetType)
            {
                return Time.fixedTime;
            }
            public override string ToString()
            {
                return "fixedTime";
            }
        }
    }


}