using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.Linq;

namespace FlowGraph.Editor
{

    [Serializable]
    public class FlowGraphConfig : ScriptableObject
    {

        public List<IncludeTypeItem> items;
        private static FlowGraphConfig instance;
        const string EditorFolderGuid = "f68c866efb551b84f88ddca0bf3e9a46";
        const string ConfigAssetGuid = "1d2f84066837a474b8c1fc697883108b";

        public static FlowGraphConfig Instance
        {
            get
            {
                if (!instance)
                {
                    string path;
                    path = AssetDatabase.GUIDToAssetPath(ConfigAssetGuid);
                    if (!string.IsNullOrEmpty(path))
                        instance = AssetDatabase.LoadAssetAtPath<FlowGraphConfig>(path);
                    if (!instance)
                    {
                        path = AssetDatabase.GUIDToAssetPath(EditorFolderGuid);
                        if (string.IsNullOrEmpty(path))
                        {
                            path = "Assets";
                        }
                        instance = Create(path + "/FlowGraphConfig.asset");
                    }
                }
                return instance;
            }
        }

        public static FlowGraphConfig Create(string assetPath)
        {
            var config = ScriptableObject.CreateInstance<FlowGraphConfig>();
            config.Init();

            AssetDatabase.CreateAsset(config, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return config;
        }

        [ContextMenu("Reset")]
        void Reset()
        {
            Init();
        }
        [ContextMenu("Reload")]
        void Reload()
        {
            FlowGraphEditorWindow.ReloadConfig();
        }
        void Init()
        {
            if (items == null)
                items = new List<IncludeTypeItem>();
            else
                items.Clear();

            foreach (var type in new Type[]{ typeof(Mathf),
                    typeof(GameObject),
                    typeof(Transform),
                    typeof(UnityEngine.Object),
                    typeof(Component),
                    typeof(Time),
                    typeof(Debug),typeof(Vector2),typeof(Vector3),typeof(Vector4),typeof(Color),typeof(AnimationCurve),typeof(Camera) })
            {
                IncludeTypeItem item = new IncludeTypeItem()
                {
                    type = type,
                };
                items.Add(item);
            }
            items.Sort(FlowGraphConfig.IncludeTypeItem.Comparer);
        }


        [Serializable]
        public class IncludeTypeItem : ISerializationCallbackReceiver
        {
            public Type type;
            public string typeName;
            public readonly static Comparer<IncludeTypeItem> Comparer = new IncludeTypeItemComparer();

            public void OnAfterDeserialize()
            {
                if (!string.IsNullOrEmpty(typeName))
                    type = Type.GetType(typeName);

            }

            public void OnBeforeSerialize()
            {
                if (type != null)
                    typeName = type.AssemblyQualifiedName;
            }



            class IncludeTypeItemComparer : Comparer<IncludeTypeItem>
            {
                public override int Compare(IncludeTypeItem a, IncludeTypeItem b)
                {
                    string aName = a.type == null ? a.typeName : a.type.FullName;
                    string bName = b.type == null ? b.typeName : b.type.FullName;
                    return string.Compare(aName, bName);
                }
            }
        }
    }



    [CustomEditor(typeof(FlowGraphConfig))]
    class FlowGraphConfigEditor : UnityEditor.Editor
    {
        string typeName;
        Type type;

        void UpdateType()
        {
            if (string.IsNullOrEmpty(typeName))
            {
                type = null;
            }
            else
            {
                type = typeName.GetTypeInAllAssemblies(true);
            }
        }



        public override void OnInspectorGUI()
        {
            FlowGraphConfig config = target as FlowGraphConfig;

            using (new GUILayout.HorizontalScope())
            {
                string newTypeName = EditorGUILayout.TextField(typeName ?? "");
                if (newTypeName != typeName)
                {
                    typeName = newTypeName;
                    UpdateType();
                }

                GUI.enabled = type != null;
                if (GUILayout.Button("Add", GUILayout.ExpandWidth(false)))
                {
                    var item = config.items.Where(o => o.type == type).FirstOrDefault();
                    if (item == null)
                    {
                        item = new FlowGraphConfig.IncludeTypeItem()
                        {
                            type = type,
                        };
                        config.items.Add(item);
                        config.items.Sort(FlowGraphConfig.IncludeTypeItem.Comparer);
                        EditorUtility.SetDirty(config);
                        FlowGraphEditorWindow.ReloadConfig();
                        typeName = string.Empty;
                    }
                }
                GUI.enabled = true;
            }


            using (var sv = new GUILayout.ScrollViewScope(scrollPos))
            {
                scrollPos = sv.scrollPosition;
                for (int i = 0; i < config.items.Count; i++)
                {
                    var item = config.items[i];
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label((i + 1).ToString(), GUILayout.Width(20));
                        if (item.type == null)
                        {
                            GUILayout.Label(item.typeName);
                        }
                        else
                        {
                            GUILayout.Label(item.type.FullName);
                        }
                    }
                    if (Event.current.type == EventType.ContextClick &&
                        GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Delete " + (i + 1)), false, (o) =>
                                                {
                                                    FlowGraphConfig.IncludeTypeItem item1 = (FlowGraphConfig.IncludeTypeItem)o;
                                                    if (config.items.Remove(item1))
                                                    {
                                                        Repaint();
                                                    }

                                                }, item);
                        menu.ShowAsContext();
                    }
                }
            }
        }
        Vector2 scrollPos;


    }
}