using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{
    [FlowGraph]
    internal static partial class UnityActions
    {
        private const string UnityCategory = "Unity";


        #region GameObject

        private const string GameObjectCategory = UnityCategory + "/GameObject";


        [Name("Active")]
        [Category(GameObjectCategory)]
        public static void Active(GameObject gameObject)
        {
            if (gameObject)
            {
                gameObject.SetActive(true);
            }
        }

        [Name("Inactive")]
        [Category(GameObjectCategory)]
        public static void Inactive(GameObject gameObject)
        {
            if (gameObject)
            {
                gameObject.SetActive(false);
            }
        }

        [Name("Destroy")]
        [Category(GameObjectCategory)]
        public static void Destroy(GameObject gameObject, float delay)
        {
            if (gameObject)
            {
                Object.Destroy(gameObject, delay);
            }
        }


        #endregion

        #region Component

        private const string ComponentCategory = UnityCategory + "/Componet";


        [Name("Enable")]
        [Category(ComponentCategory)]
        public static void Enable(Component component)
        {
            if (component)
            {
                if (component is Behaviour)
                {
                    ((Behaviour)component).enabled = true;
                }
                else if (component is Collider)
                {
                    ((Collider)component).enabled = true;
                }
            }
        }

        [Name("Disable")]
        [Category(ComponentCategory)]
        public static void Disable(Component component)
        {
            if (component)
            {
                if (component is Behaviour)
                {
                    ((Behaviour)component).enabled = false;
                }
                else if (component is Collider)
                {
                    ((Collider)component).enabled = false;
                }
            }
        }

        #endregion



        #region Transform

        private const string TransformActionsCategory = UnityCategory + "/Transform";

        #endregion


        #region Lerp

        private const string LerpsCategory = FlowNode.FunctionsCategory + "/Lerps";

        [Name("Vector2 Lerp")]
        [Category(LerpsCategory)]
        public static Vector2 Vector2Lerp(Vector2 from, Vector2 to, float t)
        {
            return Vector2.Lerp(from, to, t);
        }

        [Name("Vector3 Lerp")]
        [Category(LerpsCategory)]
        public static Vector3 Vector3Lerp(Vector3 from, Vector3 to, float t)
        {
            return Vector3.Lerp(from, to, t);
        }

        [Name("Quaternion Lerp")]
        [Category(LerpsCategory)]
        public static Quaternion QuaternionLerp(Quaternion from, Quaternion to, float t)
        {
            return Quaternion.Lerp(from, to, t);
        }

        [Name("Color Lerp")]
        [Category(LerpsCategory)]
        public static Color ColorLerp(Color from, Color to, float t)
        {
            return Color.Lerp(from, to, t);
        }


        #endregion

        [Name("TimeInterpolator")]
        [Category(FlowNode.FunctionsCategory)]
        public static float TimeInterpolator(float from, float to, AnimationCurve curve, float time)
        {
            float t = Mathf.InverseLerp(from, to, time);
            return curve.Evaluate(t);
        }
    }

}