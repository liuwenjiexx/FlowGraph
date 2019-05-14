
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowGraph;
using System;
using System.Text;
using System.Reflection;
using System.Linq;

public class TestFlow : MonoBehaviour
{

    static string GetTypeName(Type type)
    {
        if (type == typeof(void))
            return "V";

        TypeCode typeCode = Type.GetTypeCode(type);
        if (typeCode == TypeCode.Object || typeCode == TypeCode.String)
            return type.FullName;
        
        switch (typeCode)
        {
            case TypeCode.Byte:
                return "B";
            case TypeCode.Char:
                return "C";
            case TypeCode.Int16:
                return "S";
            case TypeCode.Int32:
                return "I";
            case TypeCode.Int64:
                return "L";
            case TypeCode.Single:
                return "F";
            case TypeCode.Double:
                return "D";
            case TypeCode.Boolean:
                return "Z";
        }

        return typeCode.ToString();
    }

    public static string GetMethodSignatureString(System.Reflection.MethodInfo method)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(method.Name)
            .Append('(');

        var ps = method.GetParameters();
        for (int i = 0; i < ps.Length; i++)
        {
            var p = ps[i];
            if (i > 0)
                sb.Append(",");

            if (p.ParameterType.IsByRef)
            {
                sb.Append(GetTypeName(p.ParameterType));
            }
            else
            {
                sb.Append(GetTypeName(p.ParameterType));
            }
        }

        sb.Append(')')
            .Append(GetTypeName(method.ReturnType));
        return sb.ToString();
    }

    public void AAA(Vector3 a, object b, out UnityEngine.Object c, ref float d, int e, string str, ref string refStr, out string outStr, int[] intArr, ref int[] refIntArr)
    {
        c = null;
        outStr = null;
    }
    public static Vector3 Vector3Lerp(Vector3 from, Vector3 to, float t)
    {
        return Vector3.Lerp(from, to, t);
    }

    class MyClassParent
    {
        [SerializeField]
        private string parentPrivateField;
        [SerializeField]
        public string parentPublicField;

    }

    class MyClass : MyClassParent
    {
        [SerializeField]
        private string parentPrivateField;

        [SerializeField]
        public string childPublicField;
    }


    // Use this for initialization
    void Start()
    {
        var m = GetType().GetMethod("AAA");
        Debug.Log(" ---------------- " + m.ToString());
        Debug.Log(" ---------------- " + GetMethodSignatureString(m));
        Debug.Log(GetType().GetMethod(GetMethodSignatureString(m)));

        m = GetType().GetMethod("Vector3Lerp");
        Debug.Log(" ---------------- " + GetMethodSignatureString(m));
        Debug.Log(new DateTime(2018, 7, 27).AddDays(100));

        //foreach (var field in GetSerializeFields(typeof(MyClass)))
        //{
        //    Debug.Log(field.Name + "," + field.DeclaringType);
        //}

        //Log log1 = new Log();
        //log1.ValueInputs[0].Value = "log1";

        //Log log2 = new Log();
        //log2.ValueInputs[0].Value = "log2";

        //log1.FlowOutputs[0].LinkInput = log2.FlowInputs[0];

        ////  FlowGraph g = new FlowGraph();
        //FlowGraphData g = GetComponent<FlowController>().graph;

        //g.AddNode(log1);
        //g.AddNode(log2);

        ////GetComponent<FlowController>().graph = g;
        //log1.FlowInputs[0].Execute(g);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
