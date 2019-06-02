using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Model
{
    partial class UnityActions
    {
        private const string FindCategory = UnityCategory + "/Find";


        [Name("Find")]
        [Category(FindCategory)]
        public static GameObject FindGameObject(string name)
        {
            return GameObject.Find(name);
        }


        [Name("Find GameObject")]
        [Category(FindCategory)]
        public static GameObject FindGameObject([Inject]Transform root, string name)
        {
            var t = _FindTransform(root, name);
            if (t)
                return t.gameObject;
            return null;
        }



        [Name("Find Transform")]
        [Category(FindCategory)]
        public static Transform FindTransform([Inject]Transform root, string name)
        {
            return _FindTransform(root, name);
        }

        private static Transform _FindTransform(Transform root, string name)
        {
            Transform c;
            for (int i = 0, len = root.childCount; i < len; i++)
            {
                c = root.GetChild(i);
                if (c.name == name)
                    return c;
            }

            for (int i = 0, len = root.childCount; i < len; i++)
            {
                c = _FindTransform(root.GetChild(i), name);
                if (c)
                    return c;
            }

            return null;
        }



        [Name("Find Component")]
        [Category(FindCategory)]
        public static Component FindComponent([Inject]Transform root, string name, string typeName)
        {

            Transform t = null;
            if (!string.IsNullOrEmpty(name))
            {
                t = _FindTransform(root, name);
            }
            if (!t)
                t = root;

            Type type = Extensions.GetTypeInAllAssemblies(typeName, false);
            if (type == null)
                throw new Exception("FindComponent not found type, typeName:" + typeName);
            var c = t.GetComponentInChildren(type);
            return c;
        }



    }

}