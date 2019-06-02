using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FlowGraph.Model
{
    partial class UnityActions
    {

        private const string ConvertCategory = UnityCategory + "/Convert";


        [Name("To GameObject")]
        [Category(ConvertCategory)]
        public static GameObject ToGameObject(Object obj)
        {
            if (!obj)
                return null;

            if (obj is GameObject)
                return (GameObject)obj;
            else if (obj is Component)
                return ((Component)obj).gameObject;

            throw new System.Exception("ToGameObject type error " + obj.GetType().Name);
        }


        [Name("To Transform")]
        [Category(ConvertCategory)]
        public static Transform ToTransform(Object obj)
        {
            if (!obj)
                return null;
            if (obj is Transform)
                return (Transform)obj;
            else if (obj is Component)
                return ((Component)obj).transform;
            throw new System.Exception("ToTransform type error " + obj.GetType().Name);
        }


    


    }

}