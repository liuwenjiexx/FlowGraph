using FlowGraph.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;


namespace FlowGraph.Editor
{
    [CustomFlowNodeEditor(typeof(FlowAsset))]
    public class FlowAssetNodeEditor : FlowNodeEditor
    {
        UnityEditor.Editor assetEditor;

        string AssetPath
        {
            get
            {
                var asset = target as FlowAsset;
                if (!asset)
                    return null;
                string assetPath = AssetDatabase.GetAssetPath(asset);
                return assetPath;
            }
        }

        public override GUIContent GetNodeName()
        {
            var asset = target as FlowAsset;
            if (!asset)
            {
                return new GUIContent("missing (FlowAsset)");
            }
            string assetPath = AssetPath;
            if (string.IsNullOrEmpty(assetPath))
            {
                string name;
                name = NameAttribute.Get(asset.GetType(), null);
                if (string.IsNullOrEmpty(name))
                {
                    name = asset.GetType().Name;

                    if (name.EndsWith("FlowAsset", StringComparison.InvariantCultureIgnoreCase))
                        name = name.Substring(0, name.Length - 9);
                    else if (name.EndsWith("Asset"))
                        name = name.Substring(0, name.Length - 5);
                }
                return new GUIContent(name, "FlowAsset");
            }
            else
            {
                string fileName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
                return new GUIContent(fileName + "(Asset)", "FlowAsset");
            }
        }

        public override void OnDetailsGUI()
        {
            var asset = nodeData.NodeAsset;

            bool changed = false;

            //string assetPath = AssetDatabase.GetAssetPath(nodeData.NodeAsset);
            //  if (!string.IsNullOrEmpty(assetPath))
            {
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel("Asset");
                    var newAsset = (FlowAsset)EditorGUILayout.ObjectField(asset, typeof(FlowAsset), false);
                    if (asset != newAsset)
                    {
                        nodeData.NodeAsset = newAsset;
                        asset = newAsset;
                        changed = true;
                    }
                }
            }
            if (!assetEditor || (assetEditor.target != asset))
            {
                assetEditor = UnityEditor.Editor.CreateEditor(asset);
                EditorGUIUtility.editingTextField = false;
                EditorGUIUtility.keyboardControl = 0;
            }
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                // if (assetEditor)
                {
                    assetEditor.OnInspectorGUI();
                    if (check.changed)
                    {
                        EditorUtility.SetDirty(assetEditor.target);
                    }
                }
            }

            if (changed)
            {
                GUI.changed = true;
            }
        }

    }

}