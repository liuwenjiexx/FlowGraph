using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UnityEditor;
using System.Reflection;

namespace FlowGraph
{

    internal static class Extensions
    {

        public static void LabelFit(this GUIStyle style, GUIContent content, int maxWidth)
        {
            var size = style.CalcSize(content);
            if (size.x > maxWidth)
            {
                GUIStyle s = new GUIStyle(style);
                while (size.x > maxWidth && s.fontSize > 8)
                {
                    s.fontSize--;
                    size = s.CalcSize(content);
                }
                GUILayout.Label(content, s);
            }
            else
            {
                GUILayout.Label(content, style);
            }
        }

        #region DrawLine

        static float OffsetAngle(Vector2 dir, Vector2 normal)
        {
            float angle = Vector2.Angle(dir, normal);
            if (angle > 90)
            {
                angle = 180 - angle;
            }
            return angle;
        }

        static Material lineMat;
        internal static void DrawLink(this Vector2 startPos, Vector2 endPos, int lineWidth, Color lineColor)
        {
            if (Event.current.type == EventType.Repaint)
            {

                if (lineMat == null)
                    lineMat = new Material(Shader.Find("UI/Default"));
                GL.PushMatrix();
                lineMat.SetPass(0);
                GL.Color(lineColor);
                GL.LoadPixelMatrix();

                IEnumerable<Vector3> path;
                Vector2 dir;
                float angle = OffsetAngle((endPos - startPos), Vector2.right);
                bool horizontal = false;
                if (angle < 45f)
                {
                    horizontal = true;
                }
                else
                {
                    horizontal = false;
                }
                if (horizontal)
                {

                    dir = new Vector2(endPos.x - startPos.x, 0);
                }
                else
                {
                    dir = new Vector2(0, endPos.y - startPos.y);
                }

                float dist = dir.magnitude;
                dir = dir.normalized;
                float arrowDist = dist * 0.3f;
                Vector2 startTangent = startPos + dir * arrowDist;
                Vector2 endTangent = endPos + dir * -arrowDist;

                path = EnumerateBezierPoints(startPos, endPos, startTangent, endTangent, 10).Select(o => (Vector3)o);


                GLDrawPath(path, lineColor, lineWidth, false);
                GL.PopMatrix();
            }
        }

        internal static IEnumerable<Vector3> EnumerateBezierPoints(this Vector3 start, Vector3 end, Vector3 startTangent, Vector3 endTangent, float stepDistance)
        {
            if (stepDistance <= 0f)
            {
                yield return start;
                yield return end;
                yield break;
            }

            Vector3 dir = end - start;
            float totalDist = dir.magnitude;
            if (totalDist <= 0f)
            {
                yield return start;
                yield return end;
                yield break;
            }
            float t = 0;
            float n = stepDistance / totalDist;

            while (true)
            {

                if (t < 1f)
                {
                    yield return GetBezierPoint(start, end, startTangent, endTangent, t);
                    t += n;
                }
                else
                {
                    yield return end;
                    break;
                }

            }
        }
        internal static Vector3 GetBezierPoint(this Vector3 start, Vector3 end, Vector3 startTangent, Vector3 endTangent, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            float tt = t * t;
            float oneMinusT2 = oneMinusT * oneMinusT;
            return
                 oneMinusT2 * oneMinusT * start +
                 3f * oneMinusT2 * t * startTangent +
                 3f * oneMinusT * tt * endTangent +
                 tt * t * end;
        }
        internal static void GLDrawPath(this IEnumerable<Vector3> points, Color color, int lineWidth, bool closed)
        {
            GL.Begin(GL.LINE_STRIP);
            GL.Color(color);
            Vector3 first = new Vector3();
            Vector3 prev = new Vector3();
            bool isFirst = true;
            foreach (var point in points)
            {
                if (isFirst)
                {
                    first = point;
                    isFirst = false;
                }
                else
                {
                    GLDrawLine(prev, point, lineWidth);
                }

                prev = point;
            }
            if (!isFirst && closed)
            {
                GLDrawLine(prev, first, lineWidth);
            }
            GL.End();
        }
        internal static void GLDrawLine(this Vector3 start, Vector3 end, int lineWidth)
        {
            float x1 = start.x, y1 = start.y, z1 = start.z, x2 = end.x, y2 = end.y, z2 = end.z;

            int halfWidth = (int)(lineWidth * 0.5f);
            GL.Vertex3(x1, y1, z1);
            GL.Vertex3(x2, y2, z2);
            for (float i = 0; i < lineWidth; ++i)
            {
                if (i % 2 == 0)
                {
                    for (float j = -halfWidth; j < halfWidth; ++j)
                    {
                        GL.Vertex3((x1 - (i * 0.5f)), (y1 + j), z1);
                        GL.Vertex3((x2 - (i * 0.5f)), (y2 + j), z2);
                    }
                }
                else
                {
                    for (float j = -halfWidth; j < halfWidth; ++j)
                    {
                        GL.Vertex3((x1 + (i * 0.5f)), (y1 + j), z1);
                        GL.Vertex3((x2 + (i * 0.5f)), (y2 + j), z2);
                    }
                }
            }

        }
        #endregion


        public static object GetObjectOfProperty(this SerializedProperty prop)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }
            return obj;
        }

        public static void SetObjectOfProperty(this SerializedProperty prop, object value)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements.Take(elements.Length - 1))
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }

            if (System.Object.ReferenceEquals(obj, null)) return;

            try
            {
                var element = elements.Last();

                if (element.Contains("["))
                {
                    var tp = obj.GetType();
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    var field = tp.GetField(elementName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var arr = field.GetValue(obj) as System.Collections.IList;
                    if (arr != null)
                        arr[index] = value;
                    //var elementName = element.Substring(0, element.IndexOf("["));
                    //var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    //var arr = DynamicUtil.GetValue(element, elementName) as System.Collections.IList;
                    //if (arr != null) arr[index] = value;
                }
                else
                {
                    var tp = obj.GetType();
                    var field = tp.GetField(element, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field != null)
                    {
                        field.SetValue(obj, value);
                    }
                    //DynamicUtil.SetValue(obj, element, value);
                }

            }
            catch
            {
                return;
            }
        }

        public static object GetObjectPropertyOwner(this SerializedProperty prop)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements.Take(elements.Length - 1))
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }
            return obj;
        }

        private static object GetValue_Imp(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();

            while (type != null)
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);

                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }
            return null;
        }

        private static object GetValue_Imp(object source, string name, int index)
        {
            var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
            if (enumerable == null) return null;
            var enm = enumerable.GetEnumerator();
            //while (index-- >= 0)
            //    enm.MoveNext();
            //return enm.Current;

            for (int i = 0; i <= index; i++)
            {
                if (!enm.MoveNext()) return null;
            }
            return enm.Current;
        }
        public static string ClampRight(this string s, int maxLength)
        {
            if (s == null)
                return s;
            if (s.Length > maxLength)
                return s.Substring(0, maxLength);
            return s;
        }

        /*
   public static void SetEnumValue<T>(this SerializedProperty prop, T value) where T : struct
   {
       if (prop == null) throw new System.ArgumentNullException("prop");
       if (prop.propertyType != SerializedPropertyType.Enum) throw new System.ArgumentException("SerializedProperty is not an enum type.", "prop");

       //var tp = typeof(T);
       //if(tp.IsEnum)
       //{
       //    prop.enumValueIndex = prop.enumNames.IndexOf(System.Enum.GetName(tp, value));
       //}
       //else
       //{
       //    int i = ConvertUtil.ToInt(value);
       //    if (i < 0 || i >= prop.enumNames.Length) i = 0;
       //    prop.enumValueIndex = i;
       //}
       prop.intValue = ConvertUtil.ToInt(value);
   }

   public static void SetEnumValue(this SerializedProperty prop, System.Enum value)
   {
       if (prop == null) throw new System.ArgumentNullException("prop");
       if (prop.propertyType != SerializedPropertyType.Enum) throw new System.ArgumentException("SerializedProperty is not an enum type.", "prop");

       if (value == null)
       {
           prop.enumValueIndex = 0;
           return;
       }

       //int i = prop.enumNames.IndexOf(System.Enum.GetName(value.GetType(), value));
       //if (i < 0) i = 0;
       //prop.enumValueIndex = i;
       prop.intValue = ConvertUtil.ToInt(value);
   }

   public static void SetEnumValue(this SerializedProperty prop, object value)
   {
       if (prop == null) throw new System.ArgumentNullException("prop");
       if (prop.propertyType != SerializedPropertyType.Enum) throw new System.ArgumentException("SerializedProperty is not an enum type.", "prop");

       if (value == null)
       {
           prop.enumValueIndex = 0;
           return;
       }

       //var tp = value.GetType();
       //if (tp.IsEnum)
       //{
       //    int i = prop.enumNames.IndexOf(System.Enum.GetName(tp, value));
       //    if (i < 0) i = 0;
       //    prop.enumValueIndex = i;
       //}
       //else
       //{
       //    int i = ConvertUtil.ToInt(value);
       //    if (i < 0 || i >= prop.enumNames.Length) i = 0;
       //    prop.enumValueIndex = i;
       //}
       prop.intValue = ConvertUtil.ToInt(value);
   }

   public static T GetEnumValue<T>(this SerializedProperty prop) where T : struct, System.IConvertible
   {
       if (prop == null) throw new System.ArgumentNullException("prop");

       try
       {
           //var name = prop.enumNames[prop.enumValueIndex];
           //return ConvertUtil.ToEnum<T>(name);
           return ConvertUtil.ToEnum<T>(prop.intValue);
       }
       catch
       {
           return default(T);
       }
   }

   public static System.Enum GetEnumValue(this SerializedProperty prop, System.Type tp)
   {
       if (prop == null) throw new System.ArgumentNullException("prop");
       if (tp == null) throw new System.ArgumentNullException("tp");
       if (!tp.IsEnum) throw new System.ArgumentException("Type must be an enumerated type.");

       try
       {
           //var name = prop.enumNames[prop.enumValueIndex];
           //return System.Enum.Parse(tp, name) as System.Enum;
           return ConvertUtil.ToEnumOfType(tp, prop.intValue);
       }
       catch
       {
           return System.Enum.GetValues(tp).Cast<System.Enum>().First();
       }
   }*/



    }

}