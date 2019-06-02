using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{

    partial class UnityActions
    {
        private const string Vector3Category = UnityCategory + "/Vector3";

        [Name("New Vector3")]
        [Category(Vector3Category)]
        [return: Name("vector3")]
        public static Vector3 NewVector3(float x, float y, float z)
        {
            return new Vector3(x, y, z);
        }


        [Name("Extract Vector3")]
        [Category(Vector3Category)]
        public static void ExtractVector3(Vector3 vector3, out float x, out float y, out float z)
        {
            x = vector3.x;
            y = vector3.y;
            z = vector3.z;
        }

    }
}
