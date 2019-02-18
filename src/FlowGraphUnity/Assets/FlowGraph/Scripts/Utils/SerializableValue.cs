using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FlowGraph
{

    [Serializable]
    public class SerializableValue : ISerializationCallbackReceiver
    {
        [SerializeField]
        private SerializableTypeCode typeCode;
        [SerializeField]
        private string stringValue;
        [SerializeField]
        private UnityEngine.Object objectValue;
        [SerializeField]
        private AnimationCurve curveValue;
        private object value;

        public object Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public SerializableTypeCode TypeCode
        {
            get { return typeCode; }
        }

        public SerializableValue(Type type)
            : this(TypeToSerializableTypeCode(type))
        {
        }
        public SerializableValue(TypeCode typeCode)
        {
            this.typeCode = (SerializableTypeCode)typeCode;
        }
        public SerializableValue(SerializableTypeCode typeCode)
        {
            this.typeCode = typeCode;
        }
        public SerializableValue(SerializableTypeCode typeCode, object value)
        {
            this.typeCode = typeCode;
            this.value = value;
        }

        public void OnAfterDeserialize()
        {
            value = null;
            switch (typeCode)
            {
                case SerializableTypeCode.UnityObject:
                    value = objectValue;
                    break;
                case SerializableTypeCode.AnimationCurve:
                    value = curveValue;
                    break;
                case SerializableTypeCode.Object:
                    break;
                default:
                    value = DeserializeFromString(typeCode, stringValue);
                    break;
            }
        }

        public void OnBeforeSerialize()
        {
            objectValue = null;
            stringValue = null;
            switch (typeCode)
            {
                case SerializableTypeCode.UnityObject:
                    if (value != null)
                    {
                        objectValue = value as UnityEngine.Object;
                    }
                    else
                    {
                        objectValue = null;
                    }
                    break;
                case SerializableTypeCode.String:
                    stringValue = value as string;
                    break;
                case SerializableTypeCode.AnimationCurve:
                    curveValue = value as AnimationCurve;
                    break;
                case SerializableTypeCode.Object:
                    break;
                default:
                    stringValue = SerializeToString(typeCode, value);
                    break;
            }
        }

        public static SerializableTypeCode TypeToSerializableTypeCode(Type type)
        {
            var typeCode = (SerializableTypeCode)Type.GetTypeCode(type);

            if (typeCode == SerializableTypeCode.Object)
            {

                if (typeof(UnityEngine.Object).IsAssignableFrom(type))
                {
                    typeCode = SerializableTypeCode.UnityObject;
                }
                else
                {
                    if (type == typeof(Vector2))
                        typeCode = SerializableTypeCode.Vector2;
                    else if (type == typeof(Vector2Int))
                        typeCode = SerializableTypeCode.Vector2Int;
                    else if (type == typeof(Vector3))
                        typeCode = SerializableTypeCode.Vector3;
                    else if (type == typeof(Vector3Int))
                        typeCode = SerializableTypeCode.Vector3Int;
                    else if (type == typeof(Vector4))
                        typeCode = SerializableTypeCode.Vector4;
                    else if (type == typeof(Color))
                        typeCode = SerializableTypeCode.Color;
                    else if (type == typeof(Color32))
                        typeCode = SerializableTypeCode.Color32;
                    else if (type == typeof(AnimationCurve))
                        typeCode = SerializableTypeCode.AnimationCurve;
                }
            }
            return typeCode;
        }

        public static Type SerializableTypeCodeToType(SerializableTypeCode typeCode)
        {
            switch (typeCode)
            {
                case SerializableTypeCode.String:
                    return typeof(string);
                case SerializableTypeCode.Int32:
                    return typeof(int);
                case SerializableTypeCode.Single:
                    return typeof(float);
                case SerializableTypeCode.Boolean:
                    return typeof(bool);
                case SerializableTypeCode.DBNull:
                    return typeof(DBNull);
                case SerializableTypeCode.Char:
                    return typeof(char);
                case SerializableTypeCode.SByte:
                    return typeof(sbyte);
                case SerializableTypeCode.Byte:
                    return typeof(byte);
                case SerializableTypeCode.Int16:
                    return typeof(short);
                case SerializableTypeCode.Int64:
                    return typeof(long);
                case SerializableTypeCode.UInt16:
                    return typeof(ushort);
                case SerializableTypeCode.UInt32:
                    return typeof(uint);
                case SerializableTypeCode.UInt64:
                    return typeof(ulong);
                case SerializableTypeCode.Double:
                    return typeof(double);
                case SerializableTypeCode.Decimal:
                    return typeof(decimal);
                case SerializableTypeCode.DateTime:
                    return typeof(DateTime);
                case SerializableTypeCode.UnityObject:
                    return typeof(UnityEngine.Object);
                case SerializableTypeCode.Vector2:
                    return typeof(Vector2);
                case SerializableTypeCode.Vector2Int:
                    return typeof(Vector2Int);
                case SerializableTypeCode.Vector3:
                    return typeof(Vector3);
                case SerializableTypeCode.Vector3Int:
                    return typeof(Vector3Int);
                case SerializableTypeCode.Vector4:
                    return typeof(Vector4);
                case SerializableTypeCode.Color:
                    return typeof(Color);
                case SerializableTypeCode.Color32:
                    return typeof(Color32);
                case SerializableTypeCode.AnimationCurve:
                    return typeof(AnimationCurve);
            }
            return typeof(object);
        }


        public static string SerializeToString(SerializableTypeCode typeCode, object value)
        {
            string str = null;


            switch (typeCode)
            {
                case SerializableTypeCode.Vector2:
                    {
                        Vector2 v = (Vector2)value;
                        str = v.x.ToString() + "," + v.y.ToString();
                    }
                    break;
                case SerializableTypeCode.Vector2Int:
                    {
                        Vector2Int v = (Vector2Int)value;
                        str = v.x.ToString() + "," + v.y.ToString();
                    }
                    break;
                case SerializableTypeCode.Vector3:
                    {
                        Vector3 v = (Vector3)value;
                        str = v.x + "," + v.y + "," + v.z;
                    }
                    break;
                case SerializableTypeCode.Vector3Int:
                    {
                        Vector3Int v = (Vector3Int)value;
                        str = v.x + "," + v.y + "," + v.z;
                    }
                    break;
                case SerializableTypeCode.Vector4:
                    {
                        Vector4 v = (Vector4)value;
                        str = v.x + "," + v.y + "," + v.z + "," + v.w;
                    }
                    break;
                case SerializableTypeCode.Color:
                    {
                        Color v = (Color)value;
                        str = v.r + "," + v.g + "," + v.b + "," + v.a;
                    }
                    break;
                case SerializableTypeCode.Color32:
                    {
                        Color32 v = (Color32)value;
                        str = v.r + "," + v.g + "," + v.b + "," + v.a;
                    }
                    break;
                default:
                    if (value != null)
                        str = value.ToString();
                    break;
            }
            return str;
        }

        public static object DeserializeFromString(SerializableTypeCode typeCode, string str)
        {
            object value = null;
            switch (typeCode)
            {
                case SerializableTypeCode.String:
                    value = str;
                    break;
                case SerializableTypeCode.Double:
                    {
                        double n;
                        if (double.TryParse(str, out n))
                            value = n;
                        else
                            value = 0d;
                    }
                    break;
                case SerializableTypeCode.Single:
                    {
                        float n;
                        if (float.TryParse(str, out n))
                            value = n;
                        else
                            value = 0f;
                    }
                    break;
                case SerializableTypeCode.Int32:
                    {
                        int n;
                        if (int.TryParse(str, out n))
                            value = n;
                        else
                            value = 0;
                    }
                    break;
                case SerializableTypeCode.Boolean:
                    {
                        bool b;
                        if (bool.TryParse(str, out b))
                            value = b;
                        else
                            value = false;
                    }
                    break;
                case SerializableTypeCode.Vector2:
                    {
                        Vector2 v = new Vector2();
                        float n;
                        string[] parts = str.Split(',');
                        for (int i = 0; i < parts.Length; i++)
                        {
                            if (float.TryParse(parts[i], out n))
                                v[i] = n;
                        }
                        value = v;
                    }
                    break;
                case SerializableTypeCode.Vector2Int:
                    {
                        Vector2Int v = new Vector2Int();
                        int n;
                        string[] parts = str.Split(',');
                        for (int i = 0; i < parts.Length; i++)
                        {
                            if (int.TryParse(parts[i], out n))
                                v[i] = n;
                        }
                        value = v;
                    }
                    break;
                case SerializableTypeCode.Vector3:
                    {
                        Vector3 v = new Vector3();
                        float n;
                        string[] parts = str.Split(',');
                        for (int i = 0; i < parts.Length; i++)
                        {
                            if (float.TryParse(parts[i], out n))
                                v[i] = n;
                        }
                        value = v;
                    }
                    break;
                case SerializableTypeCode.Vector3Int:
                    {
                        Vector3Int v = new Vector3Int();
                        int n;
                        string[] parts = str.Split(',');
                        for (int i = 0; i < parts.Length; i++)
                        {
                            if (int.TryParse(parts[i], out n))
                                v[i] = n;
                        }
                        value = v;
                    }
                    break;
                case SerializableTypeCode.Vector4:
                    {
                        Vector4 v = new Vector4();
                        float n;
                        string[] parts = str.Split(',');
                        for (int i = 0; i < parts.Length; i++)
                        {
                            if (float.TryParse(parts[i], out n))
                                v[i] = n;
                        }
                        value = v;
                    }
                    break;
                case SerializableTypeCode.Color:
                    {
                        Color v = new Color();
                        float n;
                        string[] parts = str.Split(',');
                        for (int i = 0; i < parts.Length; i++)
                        {
                            if (float.TryParse(parts[i], out n))
                                v[i] = n;
                        }
                        value = v;
                    }
                    break;
                case SerializableTypeCode.Color32:
                    {
                        Color32 v = new Color32();
                        byte n;
                        string[] parts = str.Split(',');
                        for (int i = 0; i < parts.Length; i++)
                        {
                            if (byte.TryParse(parts[i], out n))
                            {
                                switch (i)
                                {
                                    case 0:
                                        v.r = n;
                                        break;
                                    case 1:
                                        v.g = n;
                                        break;
                                    case 2:
                                        v.b = n;
                                        break;
                                    case 3:
                                        v.a = n;
                                        break;
                                }
                            }
                        }
                        value = v;
                    }
                    break;

            }

            return value;
        }

        public override string ToString()
        {
            //if (typeCode == SerializableTypeCode.Object)
            //    return objectValue == null ? "null" : objectValue.ToString();
            //else if (typeCode == SerializableTypeCode.AnimationCurve)
            //    return curveValue.ToString();

            //return stringValue;
            return value == null ? "null" : value.ToString();
        }




        public enum SerializableTypeCode
        {
            None = 0,

            #region Base Types

            Object = 1,
            DBNull = 2,
            Boolean = 3,
            Char = 4,
            SByte = 5,
            Byte = 6,
            Int16 = 7,
            UInt16 = 8,
            Int32 = 9,
            UInt32 = 10,
            Int64 = 11,
            UInt64 = 12,
            Single = 13,
            Double = 14,
            Decimal = 15,
            DateTime = 16,
            String = 18,

            #endregion

            #region Unity Types


            UnityObject = 100,
            Vector2 = 101,
            Vector2Int = 102,
            Vector3 = 103,
            Vector3Int = 104,
            Vector4 = 105,
            Color = 106,
            Color32 = 107,
            Rect = 108,
            RectInt = 109,
            Bounds = 110,
            BoundsInt = 111,
            AnimationCurve = 112,

            #endregion
        }

    }

}