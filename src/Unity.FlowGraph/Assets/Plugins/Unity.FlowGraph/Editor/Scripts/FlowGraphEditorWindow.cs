using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using FlowGraph.Model;
using FlowGraph.Editor.GUIExtensions;
using System.IO;

namespace FlowGraph.Editor
{

    public class FlowGraphEditorWindow : EditorWindow
    {
        const string windowName = "Flow Graph";


        FlowGraphController flowController;

        #region Window Setting

        private Vector2 windowOffset = -windowCenter;
        private readonly static Vector2 windowSize = new Vector2(10000, 10000);
        private readonly static Vector2 windowCenter = windowSize * 0.5f;

        #endregion

        #region Node Setting

        private int nodeWidth = 150;
        private int nameHeight = 20;
        private int portWidth = 10;
        private int portHeight = 16;
        private int portLabelSpace = 1;
        private int portLabelWidth;
        private int portOffset = 3;
        private Color nodeNameSelectedColor = Color.blue;
        private Color nodeErrorColor = Color.red;

        private Color portSelectedColor = Color.blue;
        private Color portNormalColor = Color.white;

        private bool setStyle;
        private GUIStyle nodeNameStyle;
        private GUIStyle rightPortLabelStyle;
        private GUIStyle leftPortLabelStyle;
        GUIStyle portInputDefaultStyle;

        private int linkLineWidth = 1;

        #endregion

        #region Details Panel Setting

        private int detailsPanelWidth = 200;
        private GUIStyle detailsNameStyle;
        private GUIStyle detailsFullNameStyle;
        private int detailsFieldLabelWidth = 80;
        #endregion

        [NonSerialized]
        private FlowNodeData selected;
        private object selectedPort;

        private Vector2 dragStart;
        private Vector2 dragOffset;
        private Vector2 dragOriginPos;
        private object downPort;
        private object dragTarget;
        private int dragTargetState;
        private bool isDraging;

        private GenericMenu newNodeMenu;
        private Vector2 showNewMenuPos;
        GenericMenu addVariableMenu;
        Color linkNormalColor = Color.white;
        Color linkActiveColor = Color.yellow;
        private int lineSpaceHeight = 5;

        static Color DefaultColor = Color.white;
        static Color32 DefaultColor32 = Color.white;

        static AnimationCurve DefaultCurve = AnimationCurve.Linear(0, 0, 1, 1);

        private static Dictionary<Type, FlowNodeInfo> flowNodeInfoMaps;
        private static Dictionary<MethodInfo, FlowNodeInfo> flowNodeInfoMapMethods;

        private Vector2 detailsPanelScrollPos;
        int dragWindowState;
        Vector2 dragWindowDown;
        Vector2 dragWindowOffset;

        static string DragObjectName = "object";
        public const string ArrowRightChar = "▶";
        public const string ArrowLeftChar = "◀";
        public const string InOutChar = "■";
        public UnityEngine.Object target;
        private static FlowGraphData graph;
        private string fieldName;
        private Dictionary<Type, GenericMenu> typeMenus;

        private static Texture2D icon;
        public static float iconOffset;


        class FlowNodeInfo
        {
            public string Category;
            public string Description;
            public string Name;
            public string CategoryName;
            public string DisplayFullName;
            public Type Type;
            public MethodInfo method;
            public NodeType NodeType;
            public List<MemberInfo> dataMembers;
            public Type ValueType;
            public bool ignore;

        }

        enum NodeType
        {
            Action,
            Function,
            FlowControler,
            Variable,
            Value,
        }


        FlowGraphData Graph
        {
            get
            {
                return graph;
            }
            set
            {
                graph = value;
            }
        }

        UnityEngine.Object Target
        {
            get
            {
                return target;
            }
        }

        int ScreenWidth
        {
            get
            {
                float scale = 96 / Screen.dpi;
                return (int)(Screen.width * scale);
            }
        }
        private static string packageDir;
        public static string PackageDir
        {
            get
            {
                if (string.IsNullOrEmpty(packageDir))
                    packageDir = GetPackageDirectory("Unity.FlowGraph");

                return packageDir;
            }
        }

        private static string GetPackageDirectory(string packageName)
        {
            foreach (var dir in Directory.GetDirectories("Assets", "*", SearchOption.AllDirectories))
            {
                if (string.Equals(Path.GetFileName(dir), packageName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return dir;
                }
            }

            string path = Path.Combine("Packages", packageName);
            if (Directory.Exists(path))
            {
                return path;
            }

            return null;
        }

        public static void ShowWindow(UnityEngine.Object target, string fieldName)
        {
            var win = GetWindow<FlowGraphEditorWindow>();
            win.flowController = target as FlowGraphController;
            //FlowGraphEditorWindow.target = target;
            win.target = target;
            Selection.activeObject = target;

            win.fieldName = fieldName;


            win.windowOffset = -windowCenter;
            win.Show();
        }

        public static void ShowWindow(UnityEngine.Object target, FlowGraphData g)
        {
            var win = GetWindow<FlowGraphEditorWindow>();
            win.flowController = target as FlowGraphController;
            //FlowGraphEditorWindow.target = target;
            win.target = target;
            win.Graph = g;
            win.fieldName = null;
            win.windowOffset = -windowCenter;
            win.Show();
        }

        public static void ShowWindow(FlowGraphAsset asset)
        {
            var win = GetWindow<FlowGraphEditorWindow>();
            //FlowGraphEditorWindow.target = target;
            win.target = asset;
            //  Selection.activeObject = asset;
            win.fieldName = null;
            win.windowOffset = -windowCenter;
            win.Show();
        }


        public static void ShowWindow(FlowGraphController graphController)
        {
            var g = graphController.Graph;
            ShowWindow(graphController, g);
        }
        public static void ShowWindow(UnityEngine.Object target, FlowGraphField flowGraphField)
        {

            if (flowGraphField.IsAssetData)
            {
                var assetData = flowGraphField.AssetData;
                if (assetData)
                {
                    FlowGraphEditorWindow.ShowWindow(assetData);
                }
            }
            else
            {
                var graphData = flowGraphField.GetFlowGraphData();
                if (graphData == null)
                {
                    return;
                }
                FlowGraphEditorWindow.ShowWindow(target, graphData);
            }
        }

        [InitializeOnLoadMethod]
        static void Init()
        {
            Selection.selectionChanged -= Changed;
            Selection.selectionChanged += Changed;
            EditorApplication.hierarchyWindowItemOnGUI += hierarchyWindowItemOnGUI;

        }

        static void Changed()
        {
        }

        private static Texture2D Icon
        {
            get
            {
                if (!icon)
                    icon = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(PackageDir, "Editor/Textures/icon.psd"));
                return icon;
            }
        }
        private static GUIStyle iconStyle;
        private static GUIStyle IconStyle
        {
            get
            {
                if (iconStyle == null)
                {
                      iconStyle = new GUIStyle();
                    //iconStyle.normal.background = Icon;
                    iconStyle.padding = new RectOffset(1, 1,1, 1); 
                }
                return iconStyle;
            }
        }

        static void hierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (!go)
                return;
            var fg = go.GetComponent<FlowGraphController>();
            if (fg)
            {
 
                if (GUI.Button(new Rect(selectionRect.xMax + iconOffset, selectionRect.y, selectionRect.height, selectionRect.height), Icon, IconStyle))
                {
                    ShowWindow(fg);
                }
            }
        }

        private void OnSelectionChange()
        {

            Repaint();
        }


        private void Update()
        {
            if (Application.isPlaying)
            {
                Repaint();
            }
        }



        void OnEnable()
        {
            titleContent = new GUIContent(windowName);
            portLabelWidth = (int)(nodeWidth * 0.5f - portWidth - 3);

            if (flowNodeInfoMaps == null)
            {
                ReloadConfig();
            }
        }

        public static void ReloadConfig()
        {
            flowNodeInfoMaps = new Dictionary<Type, FlowNodeInfo>();
            flowNodeInfoMapMethods = new Dictionary<MethodInfo, FlowNodeInfo>();
            Type flowNodeType = typeof(FlowNode);
            FlowNodeInfo info;

            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(o => o.GetTypes()))
            {
                if (type.IsAbstract)
                    continue;
                if (!type.IsSubclassOf(flowNodeType))
                    continue;

                //if (type.GetConstructors().Where(o => o.GetParameters().Length == 0).Count() == 0)
                //    continue;
                info = new FlowNodeInfo()
                {
                    Name = type.Name,
                    Type = type,
                };
                string defaultName = info.Name;
                if (defaultName.EndsWith("Node", StringComparison.InvariantCultureIgnoreCase))
                    defaultName = defaultName.Substring(0, defaultName.Length - 4);
                info.Name = NameAttribute.Get(type, defaultName);
                info.CategoryName = info.Name;
                info.Category = CategoryAttribute.Get(type, info.Category);
                info.Description = DescriptionAttribute.Get(type, info.Description);
                info.DisplayFullName = info.Type.FullName;
                if (type.IsDefined(typeof(HiddenAttribute), true))
                    info.ignore = true;
                if (type.IsSubclassOf(typeof(ActionNodeBase)) || type.IsSubclassOf(typeof(CoroutineNodeBase)))
                {
                    info.NodeType = NodeType.Action;
                }
                else if (type.IsGenericSubclassOf(typeof(FunctionNodeBase)))
                {
                    info.NodeType = NodeType.Function;
                }
                else if (type.IsSubclassOf(typeof(FlowControlerNode)))
                {
                    info.NodeType = NodeType.FlowControler;
                }
                else if (type.IsGenericSubclassOf(typeof(InputableValueNode<>)))
                {
                    info.NodeType = NodeType.Value;
                    Type valueNodeType = type.FindGenericType(typeof(InputableValueNode<>));
                    info.ValueType = valueNodeType.GetGenericArguments()[0];

                }
                else if (type.IsGenericSubclassOf(typeof(VariableNode<>)))
                {
                    info.NodeType = NodeType.Variable;
                    Type varNodeType = type.FindGenericType(typeof(VariableNode<>));
                    info.ValueType = varNodeType.GetGenericArguments()[0];
                }
                else
                {
                    info.NodeType = NodeType.Action;
                }


                var tmp = FlowNodeData.GetSerializeFields(type);
                if (tmp != null && tmp.Length > 0)
                {
                    if (info.dataMembers == null)
                        info.dataMembers = new List<MemberInfo>();
                    info.dataMembers.AddRange(tmp);
                }



                if (string.IsNullOrEmpty(info.Category))
                {
                    info.Category = "Other";
                }

                AddFlowNodeInfo(info);
            }

            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(o => o.GetTypes()))
            {
                if (type.IsDefined(typeof(FlowGraphAttribute), true))
                {
                    FlowGraphConfig.IncludeTypeItem item = new FlowGraphConfig.IncludeTypeItem();
                    item.type = type;
                    ParseType(item);
                }
            }

            var config = FlowGraphConfig.Instance;
            if (config)
            {
                foreach (var item in config.items)
                {
                    ParseType(item);
                }
            }
        }


        public static void SetVariableName(FlowNode node, string varName)
        {
            Type varNodeType = node.GetType().FindGenericType(typeof(VariableNode<>));
            FieldInfo nameField = varNodeType.GetField("name", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            nameField.SetValue(node, varName);
        }

        public static string GetVariableName(FlowNode node)
        {
            Type varNodeType = node.GetType().FindGenericType(typeof(VariableNode<>));
            FieldInfo nameField = varNodeType.GetField("name", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            return nameField.GetValue(node) as string;
        }


        static void ParseType(FlowGraphConfig.IncludeTypeItem item)
        {
            Type type = item.type;
            string typeCategory = null;

            typeCategory = CategoryAttribute.Get(type, null);
            FlowNodeInfo info;

            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
            {
                if (method.DeclaringType != type)
                    continue;
                if (method.IsGenericMethod)
                    continue;
                if (!(method.IsPublic || (method.IsDefined(typeof(CoroutineMethodAttribute), false))))
                    continue;

                info = new FlowNodeInfo();
                info.Name = method.Name;
                info.CategoryName = (method.IsStatic ? "S " : "") + method.ToString();
                info.NodeType = NodeType.Function;
                info.Description = method.DeclaringType.FullName + "." + method.Name;
                info.Category = typeCategory;
                info.Type = typeof(MethodNode);
                info.method = method;
                info.ignore = false;

                var defName = NameAttribute.Get(method, null);
                if (!string.IsNullOrEmpty(defName))
                {
                    info.CategoryName = defName;
                    info.Name = defName;
                }

                info.Category = CategoryAttribute.Get(method, info.Category);
                info.Description = DescriptionAttribute.Get(method, info.Description);

                info.DisplayFullName = type.FullName + "." + info.Name;

                if (string.IsNullOrEmpty(info.Category))
                {
                    if (string.IsNullOrEmpty(type.Namespace))
                    {
                        info.Category = "Other/" + type.Namespace + "/" + type.Name;
                    }
                    else
                    {
                        info.Category = type.Namespace + "/" + type.Name;
                    }
                }

                AddFlowNodeInfo(info);
            }
        }


        private static void AddFlowNodeInfo(FlowNodeInfo info)
        {
            if (!info.Category.EndsWith("/"))
                info.Category += "/";
            info.Category += info.CategoryName;
            if (info.method != null)
            {
                flowNodeInfoMapMethods[info.method] = info;
            }
            else
            {
                flowNodeInfoMaps[info.Type] = info;
            }

        }

        FieldInfo fieldInfo;
        FlowGraphField field;

        public static FlowGraphData CreateGraph()
        {
            var graph = new FlowGraphData();

            graph.AddNode(new GraphStartEvent(), new Vector2(50, 100));
            graph.AddNode(new GraphStopEvent(), new Vector2(50, 300));

            return graph;
        }


        private void OnGUI()
        {
            var oldGraph = Graph;
            /*
            if (Selection.activeObject != null)
            {
                if (Selection.activeObject is GameObject)
                {
                    target = Selection.activeGameObject.GetComponent<FlowGraphController>();

                }
                else
                {
                    target = Selection.activeObject;
                }
            }
            else
            {
                target = null;
            }
            flowController = target as FlowGraphController;
            */


            if (!target)
                return;
            /*
            field = null;

            if (!string.IsNullOrEmpty(fieldName))
            {
                graph = null;

                fieldInfo = target.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (fieldInfo != null)
                {
                    field = fieldInfo.GetValue(target) as FlowGraphField;
                    if (field != null)
                    {
                        if (field.IsAssetData)
                        {
                            if (field.AssetData)
                                graph = field.AssetData.Data;
                        }
                        else
                        {
                            graph = field.Data;
                        }
                    }
                }
            }*/
            /* else if (flowController)
             {
                 graph = flowController.Graph.GetFlowGraphData();
             }*/
            else if (target is FlowGraphAsset)
            {
                FlowGraphAsset asset = (FlowGraphAsset)target;
                graph = asset.Data;
            }

            //if (Graph == null)
            //{
            //    if (GUILayout.Button("New Graph"))
            //    {

            //        var g = CreateGraph();
            //        Graph = graph;

            //        if (field != null)
            //        {
            //            if (target)
            //                Undo.RecordObject(target, null);
            //            field.Data = graph;
            //            if (target)
            //                SetTargetDirty();
            //        }
            //        else if (target is FlowGraphAsset)
            //        {
            //            FlowGraphAsset asset = (FlowGraphAsset)target;
            //            Undo.RecordObject(asset, null);
            //            asset.Data = graph;
            //            SetTargetDirty();
            //        }
            //    }

            //}

            if (Graph == null)
            {
                selected = null;
                selectedPort = null;
                return;
            }

            if (oldGraph != Graph)
            {
                windowOffset = -windowCenter;
            }
            if (Application.isPlaying && !flowController)
                return;

            Event evt = Event.current;

            if (nodeNameStyle == null || !setStyle)
            {
                setStyle = true;

                nodeNameStyle = new GUIStyle("label");
                nodeNameStyle.alignment = TextAnchor.MiddleCenter;
                nodeNameStyle.fontStyle = FontStyle.Bold;
                nodeNameStyle.fontSize = 14;
                nodeNameStyle.normal.textColor = Color.white;
                leftPortLabelStyle = new GUIStyle("label");
                leftPortLabelStyle.alignment = TextAnchor.MiddleLeft;
                rightPortLabelStyle = new GUIStyle("label");
                rightPortLabelStyle.alignment = TextAnchor.MiddleRight;

                detailsNameStyle = new GUIStyle("label");
                detailsNameStyle.fontSize = 14;
                detailsNameStyle.alignment = TextAnchor.MiddleCenter;
                detailsFullNameStyle = new GUIStyle("label");
                detailsFullNameStyle.fontSize = 10;
                detailsFullNameStyle.alignment = TextAnchor.MiddleCenter;

                portInputDefaultStyle = new GUIStyle("label");
                portInputDefaultStyle.fontSize = 9;
                portInputDefaultStyle.normal.textColor = new Color(0.4f, 0.4f, 0.4f, 1);
            }

            using (new GUI.GroupScope(new Rect(windowOffset.x, windowOffset.y, windowSize.x, windowSize.y)))
            {

                foreach (var node in Graph.Nodes)
                {
                    DrawFlowNode(node);
                }

                foreach (var node in Graph.Nodes)
                {
                    if (node.Node != null)
                    {
                        foreach (var flowOutput in node.Node.FlowOutputs)
                        {

                            if (flowOutput.LinkInput != null)
                            {
                                Vector2 startPos = GetFlowOutputPortPosition(flowOutput).center;
                                Vector2 endPos = GetFlowInputPortPosition(flowOutput.LinkInput).center;
                                DrawLink(startPos, endPos, linkNormalColor);
                            }
                        }
                        foreach (var valueInput in node.Node.ValueInputs)
                        {
                            if (valueInput.LinkOutput != null)
                            {

                                Vector2 startPos = GetValueInputPortPosition(valueInput).center;
                                Vector2 endPos = GetValueOutputPortPosition(valueInput.LinkOutput).center;
                                DrawLink(startPos, endPos, linkNormalColor);
                            }
                        }
                    }
                }

                Vector2 dragStartPos = Vector2.zero, dragEndPos = evt.mousePosition;
                object dragToPort = null;

                foreach (var node in Graph.Nodes)
                {
                    if (node.Node == null)
                        continue;
                    foreach (var flowInput in node.Node.FlowInputs)
                    {

                        if (downPort == flowInput)
                        {

                            if (dragTargetState == 2)
                            {
                                //FlowOutput toPort = ScreenPointToFlowOutput(evt.mousePosition);
                                //if (toPort != null && toPort.Node == flowInput.Node)
                                //    toPort = null;
                                /*   if (evt.type == EventType.MouseUp)
                                   {

                                       if (toPort != null)
                                       {
                                           Undo.RecordObject(Target, "Link FlowPort");
                                           Graph.LinkFlowPort(toPort.Node, toPort.Name, flowInput.Node, flowInput.Name);
                                           SetTargetDirty();
                                       }
                                   }*/
                                dragStartPos = GetFlowInputPortPosition(flowInput).center;
                                //if (toPort != null)
                                //{
                                //    dragEndPos = GetFlowOutputPortPosition(toPort).center;
                                //}
                                //dragToPort = toPort;
                            }
                        }
                    }
                    foreach (var flowOutput in node.Node.FlowOutputs)
                    {
                        if (downPort == flowOutput)
                        {
                            if (dragTargetState == 2)
                            {
                                //var toPort = ScreenPointToFlowInput(evt.mousePosition);
                                //if (toPort != null && toPort.Node == flowOutput.Node)
                                //    toPort = null;
                                //if (evt.type == EventType.MouseUp)
                                //{
                                //    if (toPort != null)
                                //    {
                                //        Undo.RecordObject(Target, "Link FlowPort");
                                //        Graph.LinkFlowPort(flowOutput.Node, flowOutput.Name, toPort.Node, toPort.Name);
                                //        SetTargetDirty();
                                //    }
                                //}

                                dragStartPos = GetFlowOutputPortPosition(flowOutput).center;
                                //dragEndPos = evt.mousePosition;
                                //if (toPort != null)
                                //{
                                //    dragEndPos = GetFlowInputPortPosition(toPort).center;
                                //}
                                //dragToPort = toPort;
                            }

                        }

                    }

                    foreach (var valueInput in node.Node.ValueInputs)
                    {
                        if (downPort == valueInput)
                        {
                            if (dragTargetState == 2)
                            {
                                //var toPort = ScreenPointToValueOutput(evt.mousePosition);
                                //if (toPort != null && toPort.Node == valueInput.Node)
                                //    toPort = null;
                                //if (evt.type == EventType.MouseUp)
                                //{
                                //    if (toPort != null)
                                //    {
                                //        Undo.RecordObject(Target, "Link ValuePort");
                                //        Graph.LinkValuePort(toPort.Node, toPort.Name, valueInput.Node, valueInput.Name);
                                //        SetTargetDirty();
                                //    }
                                //}
                                dragStartPos = GetValueInputPortPosition(valueInput).center;
                                //dragEndPos = evt.mousePosition;
                                //if (toPort != null)
                                //{
                                //    dragEndPos = GetValueOutputPortPosition(toPort).center;
                                //}
                                //dragToPort = toPort;
                            }

                        }

                    }

                    foreach (var valueOutput in node.Node.ValueOutputs)
                    {

                        if (downPort == valueOutput)
                        {
                            if (dragTargetState == 2)
                            {
                                //var toPort = ScreenPointToValueInput(evt.mousePosition);
                                //if (toPort != null && toPort.Node == valueOutput.Node)
                                //    toPort = null;
                                //if (evt.type == EventType.MouseUp)
                                //{
                                //    if (toPort != null)
                                //    {
                                //        Undo.RecordObject(Target, "Link ValuePort");
                                //        Graph.LinkValuePort(valueOutput.Node, valueOutput.Name, toPort.Node, toPort.Name);
                                //        SetTargetDirty();
                                //    }
                                //}

                                dragStartPos = GetValueOutputPortPosition(valueOutput).center;
                                //dragEndPos = evt.mousePosition;
                                //if (toPort != null)
                                //{
                                //    dragEndPos = GetValueInputPortPosition(toPort).center;
                                //}
                                //dragToPort = toPort;
                            }

                        }

                    }

                }

                if (evt.button == 0)
                {
                    if (dragTargetState == 1)
                    {

                        if (evt.type == EventType.MouseUp)
                        {
                            dragTargetState = 0;
                            evt.Use();
                        }
                    }
                    else if (dragTargetState == 2)
                    {
                        Color linkColor = linkNormalColor;
                        //if (dragToPort != null)
                        //    linkColor = linkActiveColor;
                        DrawLink(dragStartPos, dragEndPos, linkColor);

                        if (evt.type == EventType.MouseDrag)
                        {
                            //evt.Use();
                        }
                        else if (evt.type == EventType.MouseUp)
                        {
                            dragTargetState = 0;
                            //evt.Use();
                        }
                    }
                }


            }

            DrawTopBar();

            DrawDetailsPanel();

            if (selected != null)
            {
                if (evt.type == EventType.KeyDown)
                {
                    if (evt.keyCode == KeyCode.Delete)
                    {
                        Undo.RecordObject(Target, "Delete Node");
                        Graph.RemoveNode(selected);
                        SetTargetDirty();
                        selected = null;
                        selectedPort = null;
                        evt.Use();
                        Repaint();
                    }
                }


                if (evt.type == EventType.ValidateCommand)
                {
                    if (evt.commandName == "Copy")
                    {
                        if (selected.Node != null)
                        {
                            copy = selected;
                            evt.Use();
                        }
                    }
                }

            }

            if (evt.commandName == "Paste")
            {
                var nodeData = copy as FlowNodeData;
                if (nodeData != null)
                {
                    var nodeInfo = GetNodeInfo(nodeData.Node);
                    if (nodeInfo != null)
                    {
                        CreateNode(nodeInfo, nodeData.Position + new Vector2(50, 50));
                    }
                    //evt.Use();
                }
            }


            if (evt.type == EventType.MouseUp)
            {
                if (evt.button == 0)
                {
                    EditorGUIUtility.editingTextField = false;
                    GUIUtility.hotControl = 0;
                    selected = null;
                    selectedPort = null;
                    //evt.Use();
                    Repaint();
                }
            }


            if (evt.button == 2)
            {
                if (evt.type == EventType.MouseDown)
                {
                    dragWindowState = 1;
                }
                else if (evt.type == EventType.MouseDrag)
                {
                    if (dragWindowState == 1)
                    {
                        dragWindowState++;
                        dragWindowDown = evt.mousePosition;
                        dragWindowOffset = windowOffset;
                    }
                    if (dragWindowState == 2)
                    {
                        windowOffset = dragWindowOffset + (evt.mousePosition - dragWindowDown);

                        evt.Use();
                    }
                }
                else if (evt.type == EventType.MouseUp)
                {
                    if (dragWindowState == 2)
                    {
                        dragWindowState = 0;
                        evt.Use();
                    }
                }
            }
            if (evt.type != EventType.Used)
            {
                switch (evt.type)
                {
                    case EventType.DragPerform:
                    case EventType.DragUpdated:
                        {
                            var obj = DragAndDrop.GetGenericData("object");
                            if (obj != null)
                            {
                                if (obj is VariableInfo)
                                {
                                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                                    if (evt.type == EventType.DragPerform)
                                    {
                                        DragAndDrop.AcceptDrag();
                                        var varInfo = (VariableInfo)obj;
                                        GenericMenu menu = new GenericMenu();
                                        Vector2 pos = evt.mousePosition;
                                        menu.AddItem(new GUIContent("Get Var"), false, () =>
                                        {
                                            FlowNodeInfo nodeInfo = flowNodeInfoMaps[GetGetVariableNodeTypeByValueType(varInfo.Type)];

                                            var nodeData = CreateNode(nodeInfo, WindowPositionToGraphPosition(pos));
                                            SetVariableName(nodeData.Node, varInfo.Name);
                                        });
                                        if (varInfo.Mode == VariableMode.None || varInfo.IsOut)
                                        {
                                            menu.AddItem(new GUIContent("Set Var"), false, () =>
                                            {
                                                FlowNodeInfo nodeInfo = flowNodeInfoMaps[GetSetVariableNodeTypeByValueType(varInfo.Type)];

                                                var nodeData = CreateNode(nodeInfo, WindowPositionToGraphPosition(pos));
                                                SetVariableName(nodeData.Node, varInfo.Name);
                                            });
                                        }
                                        else
                                        {
                                            menu.AddDisabledItem(new GUIContent("Set Var"), false);
                                        }
                                        menu.ShowAsContext();
                                    }
                                    evt.Use();
                                }
                                else if (obj is ValuePort)
                                {
                                    bool isOutputPort = false;
                                    ValuePort port = (ValuePort)obj;
                                    isOutputPort = port.Node.ValueOutputs.Where(o => o == port).Count() > 0;
                                    if (isOutputPort)
                                    {
                                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                                        if (evt.type == EventType.DragPerform)
                                        {
                                            DragAndDrop.AcceptDrag();
                                            ShowTypeMenu(port.ValueType, port, evt.mousePosition);
                                        }
                                    }
                                    evt.Use();
                                }
                            }




                            if (evt.type != EventType.Used && DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length > 0)
                            {

                                foreach (var item in DragAndDrop.objectReferences)
                                {
                                    if (item is FlowGraphAsset)
                                    {
                                        if (evt.type == EventType.DragPerform)
                                        {
                                            Type assetType = typeof(FlowGraphAssetNode);
                                            var node = new FlowGraphAssetNode((FlowGraphAsset)item);
                                            var nodeData = CreateNode(flowNodeInfoMaps[assetType], node, WindowPositionToGraphPosition(evt.mousePosition));
                                        }
                                        if (evt.type != EventType.Used)
                                        {
                                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                                            evt.Use();
                                        }
                                    }
                                    else
                                    {
                                        if (evt.type == EventType.DragPerform)
                                        {
                                            FlowNode node;
                                            if (item is GameObject)
                                            {
                                                var goNode = new GameObjectValue();
                                                goNode.Value = (GameObject)item;
                                                node = goNode;
                                            }
                                            /* else if (item is Transform)
                                             {
                                                 var transNode = new TransformValue();
                                                 transNode.Value = (Transform)item;
                                                 node = transNode;
                                             }*/
                                            else if (item is Component)
                                            {
                                                var cptNode = new ComponentValue();
                                                cptNode.Value = (Component)item;
                                                node = cptNode;
                                            }
                                            else
                                            {
                                                var objNode = new UnityObjectValue();
                                                objNode.Value = item;
                                                node = objNode;
                                            }
                                            var nodeData = CreateNode(flowNodeInfoMaps[node.GetType()], node, WindowPositionToGraphPosition(evt.mousePosition));

                                        }
                                        if (evt.type != EventType.Used)
                                        {
                                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                                            evt.Use();
                                        }
                                    }


                                }
                            }
                        }
                        break;
                    case EventType.DragExited:
                        break;
                }
            }

            if (evt.type == EventType.ContextClick)
            {
                if (newNodeMenu == null)
                {
                    newNodeMenu = new GenericMenu();
                    foreach (var item in flowNodeInfoMaps.Values.Concat(flowNodeInfoMapMethods.Values).Where(o => !o.ignore).OrderBy(o => o.Category).OrderBy(o => o.Category.StartsWith("Other/")))
                    {
                        newNodeMenu.AddItem(new GUIContent(item.Category), false, (d) =>
                        {
                            Vector2 pos = WindowPositionToGraphPosition(showNewMenuPos);
                            CreateNode((FlowNodeInfo)d, pos);
                        }, item);
                    }
                }

                showNewMenuPos = evt.mousePosition;
                newNodeMenu.ShowAsContext();
                evt.Use();
                Repaint();
            }

            if (evt.type == EventType.KeyUp)
            {
                Focus();
            }

        }

        void DrawTopBar()
        {
            int height = 18;
            Rect rect = new Rect(0, 0, ScreenWidth, height);
            GUI.Box(rect, "");
            using (new GUILayout.AreaScope(rect))
            {
                GUILayout.Space(1);
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Space(3);

                    if (GUILayout.Button("", "box", GUILayout.Width(11), GUILayout.Height(11)))
                    {
                        GenericMenu mainMenu = new GenericMenu();

                        if (Graph == null)
                        {
                            mainMenu.AddDisabledItem(new GUIContent("SaveAs (.asset)"), false);
                        }
                        else
                        {
                            mainMenu.AddItem(new GUIContent("SaveAs (.asset)"), false, () =>
                            {
                                string path = EditorUtility.SaveFilePanelInProject("SaveAs", "", "asset", "");
                                if (!string.IsNullOrEmpty(path))
                                {
                                    var asset = CreateInstance<FlowGraphAsset>();
                                    asset.Data = Graph;

                                    AssetDatabase.CreateAsset(asset, path);

                                    if (field != null)
                                    {
                                        Undo.RecordObject(target, null);
                                        field.AssetData = asset;
                                        SetTargetDirty();
                                    }
                                    target = asset;
                                }
                            });
                        }
                        /* if (field != null)
                         {
                             if (field.IsAssetData)
                             {
                                 mainMenu.AddItem(new GUIContent("Asset To Self"), false, () =>
                                 {
                                     if (target)
                                         Undo.RecordObject(target, null);
                                     field.Data = graph;
                                     if (target)
                                         SetTargetDirty();
                                 });

                             }
                             else
                             {
                                 mainMenu.AddDisabledItem(new GUIContent("Asset To Self"), false);
                             }
                         }*/


                        mainMenu.ShowAsContext();
                    }

                    string sourceName = "";
                    string assetPath = AssetDatabase.GetAssetPath(target);
                    string fileName = null;
                    if (!string.IsNullOrEmpty(assetPath))
                        fileName = System.IO.Path.GetFileName(assetPath);
                    if (target)
                    {
                        if (!string.IsNullOrEmpty(fileName))
                            sourceName = fileName;
                        else
                            sourceName = target.name;
                        sourceName = " [" + sourceName + "]";
                        if (GUILayout.Button(sourceName, "label"))
                        {
                            EditorGUIUtility.PingObject(target);
                            Selection.activeObject = target;
                        }
                    }
                    if (!(target is FlowGraphAsset))
                    {
                        sourceName = target.GetType().Name;
                        GUILayout.Label(sourceName);
                    }
                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        if (GUILayout.Button("AssetPath:" + System.IO.Path.GetDirectoryName(assetPath) + "/", "label"))
                        {
                            EditorGUIUtility.PingObject(target);
                            Selection.activeObject = target;
                        }
                    }

                    if (GUILayout.Button("Nodes:" + (Graph == null ? 0 : Graph.Nodes.Count), "label"))
                    {
                        if (Graph.Nodes.Count > 0)
                        {
                            var d = Graph.Nodes[0];
                            windowOffset = -windowCenter + d.Position;
                        }
                    }

                    GUILayout.FlexibleSpace();

                }
            }
        }


        Vector2 WindowPositionToGraphPosition(Vector2 mousePos)
        {
            return -(windowCenter + windowOffset) + mousePos;
        }
        bool IsAssetNode(FlowNode node)
        {
            return node is FlowGraphAssetNode;
        }

        void DrawLink(Vector2 startPos, Vector2 endPos, Color lineColor)
        {

            startPos.DrawLink(endPos, linkLineWidth, lineColor);
        }

        void SetTargetDirty()
        {
            //Asset
            if (Target)
            {
                EditorUtility.SetDirty(Target);
                Repaint();
            }
        }

        public static Type GetGetVariableNodeTypeByValueType(Type valueType)
        {
            var type = typeof(VariableNode<>).MakeGenericType(valueType);

            foreach (var t in flowNodeInfoMaps.Keys)
            {
                if (type.IsAssignableFrom(t) && !t.IsGenericSubclassOf(typeof(SetVariableNode<>)))
                {
                    return t;
                }
            }

            Debug.LogError("GetGetVariableNodeTypeByValueType null," + valueType);
            return null;
        }

        public static Type GetSetVariableNodeTypeByValueType(Type valueType)
        {
            var type = typeof(SetVariableNode<>).MakeGenericType(valueType);

            foreach (var t in flowNodeInfoMaps.Keys)
            {
                if (type.IsAssignableFrom(t))
                {
                    return t;
                }
            }

            Debug.LogError("GetSetVariableNodeTypeByValueType null," + valueType);
            return null;
        }


        void DrawDetailsPanel()
        {
            RectOffset offset = new RectOffset(0, -5, -23, 0);


            Rect rect = offset.Add(new Rect(ScreenWidth - detailsPanelWidth, 0, detailsPanelWidth, 150));
            GUI.Box(rect, "");
            Rect innerRect = new RectOffset(-2, -2, -5, -5).Add(rect);

            using (new GUILayout.AreaScope(innerRect))
            {

                if (selectedPort != null)
                {
                    if (selectedPort is FlowOutput)
                    {

                        FlowOutput flowOutput = (FlowOutput)selectedPort;
                        GUILayout.Label("Flow Output", detailsNameStyle);
                        GUILayout.Label(flowOutput.Name);
                        if (flowOutput.LinkInput != null)
                        {
                            if (GUILayout.Button("Delete Link"))
                            {
                                Undo.RecordObject(Target, "Delete Link");
                                Graph.BreakFlowPort(flowOutput);
                                SetTargetDirty();
                                Repaint();
                            }
                        }
                    }
                    else if (selectedPort is FlowInput)
                    {
                        FlowInput flowInput = (FlowInput)selectedPort;
                        GUILayout.Label("Flow Input", detailsNameStyle);
                        GUILayout.Label(flowInput.Name);

                    }
                    else
                    {
                        ValuePort valuePort = (ValuePort)selectedPort;
                        GUILayout.Label("Value Port", detailsNameStyle);
                        GUILayout.Label(valuePort.Name);
                        GUILayout.Label(valuePort.ValueType.Name);
                        if (valuePort.LinkOutput != null)
                        {
                            if (GUILayout.Button("Delete Link"))
                            {
                                Undo.RecordObject(Target, "Delete Link");
                                Graph.BreakValuePort(valuePort);
                                SetTargetDirty();
                                Repaint();
                            }
                        }
                    }
                }
                else if (selected != null)
                {
                    if (selected.Node != null)
                    {
                        var node = selected.Node;
                        var nodeInfo = GetNodeInfo(selected.Node);
                        if (nodeInfo == null)
                        {
                            var props = selected.Properties;
                            if (props != null)
                            {
                                foreach (var p in props)
                                {
                                    using (new GUILayout.HorizontalScope())
                                    {

                                        GUILayout.Label(new GUIContent(p.field, p.field), GUILayout.Width(50));

                                        if (p.value.Value != null && p.value.TypeCode == SerializableValue.SerializableTypeCode.String)
                                        {
                                            string strValue = EditorGUILayout.DelayedTextField((string)p.value.Value);
                                            if (strValue != (string)p.value.Value)
                                            {
                                                p.value.Value = strValue;
                                                SetTargetDirty();

                                                TryDeserialize();
                                            }

                                        }
                                        else
                                        {
                                            if (p.value.Value == null)
                                            {
                                                GUILayout.Label("null");
                                            }
                                            else
                                            {
                                                GUILayout.Label(new GUIContent(p.value.Value.ToString(), p.value.Value.ToString()));
                                            }
                                        }

                                    }
                                }
                            }

                            return;
                        }
                        detailsNameStyle.LabelFit(new GUIContent(nodeInfo.Name, nodeInfo.Name), (int)innerRect.width);

                        //if (nodeInfo.NodeType == NodeType.Value)
                        if (nodeInfo.dataMembers != null && nodeInfo.dataMembers.Count > 0)
                        {
                            Action<MemberInfo> drawMember = (mInfo) =>
                            {
                                FieldInfo field = mInfo as FieldInfo;
                                if (field != null)
                                {
                                    using (new GUILayout.HorizontalScope())
                                    {
                                        GUILayout.Label(field.Name, GUILayout.Width(detailsFieldLabelWidth));
                                        object value = field.GetValue(node), newValue;

                                        newValue = SerializableValuePropertyDrawer.LayoutValueField(value, field.FieldType);
                                        if (!object.Equals(newValue, value))
                                        {
                                            Undo.RecordObject(Target, null);
                                            field.SetValue(node, newValue);
                                            SetTargetDirty();
                                            node.OnAfterDeserialize();

                                        }
                                    }
                                    return;
                                }
                                PropertyInfo pInfo = mInfo as PropertyInfo;
                                if (pInfo != null)
                                {
                                    using (new GUILayout.HorizontalScope())
                                    {
                                        GUILayout.Label(pInfo.Name, GUILayout.Width(detailsFieldLabelWidth));
                                        object value = pInfo.GetValue(node, null), newValue;

                                        newValue = SerializableValuePropertyDrawer.LayoutValueField(value, pInfo.PropertyType);
                                        if (!object.Equals(newValue, value))
                                        {
                                            Undo.RecordObject(Target, null);
                                            pInfo.SetValue(node, newValue, null);
                                            SetTargetDirty();
                                        }
                                    }
                                }
                            };

                            if (nodeInfo.dataMembers != null)
                            {
                                foreach (var m in nodeInfo.dataMembers)
                                {
                                    if (m.IsDefined(typeof(HideInInspector), false))
                                        continue;
                                    drawMember(m);
                                }
                            }
                        }
                        else
                        {

                            detailsFullNameStyle.LabelFit(new GUIContent(nodeInfo.DisplayFullName, nodeInfo.DisplayFullName), (int)innerRect.width);
                        }


                    }
                    else
                    {

                        string newValue = ((GUIStyle)"label").LabelEditable(new GUIContent(selected.TypeName ?? "", selected.TypeName ?? ""));
                        if (selected.TypeName != newValue)
                        {
                            Undo.RecordObject(Target, null);
                            selected.TypeName = newValue;
                            SetTargetDirty();

                            TryDeserialize();
                        }
                    }
                }
                else
                {
                    //using (new GUILayout.HorizontalScope())
                    //{
                    //    GUIStyle graphNameStyle = new GUIStyle("label");
                    //    graphNameStyle.fontStyle = FontStyle.Bold;
                    //    graphNameStyle.normal.textColor = Color.white;
                    //    graphNameStyle.fontSize = 16;
                    //    graphNameStyle.alignment = TextAnchor.MiddleCenter;
                    //    GUIStyle graphNameEditStyle = new GUIStyle("textfield");
                    //    graphNameEditStyle.fontStyle = FontStyle.Bold;
                    //    graphNameEditStyle.focused.textColor = Color.white;
                    //    graphNameEditStyle.fontSize = 16;
                    //    graphNameEditStyle.alignment = TextAnchor.MiddleCenter;
                    //    string name = string.IsNullOrEmpty(graph.Name) ? "(Name)" : graph.Name;
                    //    var newName = graphNameStyle.LabelEditable(name, graphNameEditStyle);
                    //    if (newName != name)
                    //    {

                    //        Undo.RecordObject(Target, null);
                    //        graph.Name = newName;
                    //    }
                    //}
                    using (var sv = new GUILayout.ScrollViewScope(detailsPanelScrollPos))
                    {
                        detailsPanelScrollPos = sv.scrollPosition;

                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Label("Vars");
                            GUILayout.FlexibleSpace();
                            GUI.enabled = !Application.isPlaying;
                            if (GUILayout.Button("+", "label"))
                            {
                                if (addVariableMenu == null)
                                {
                                    addVariableMenu = new GenericMenu();
                                    foreach (var t in flowNodeInfoMaps.Select(o => o.Value).Where(o => o.NodeType == NodeType.Variable && o.Type.IsGenericSubclassOf(typeof(GetVariableNode<>))).OrderBy(o => o.Name))
                                    {
                                        string displayName = t.Name;


                                        addVariableMenu.AddItem(new GUIContent(displayName), false, (o) =>
                                        {
                                            FlowNodeInfo nodeInfo = (FlowNodeInfo)o;
                                            Type varType = nodeInfo.ValueType;
                                            string varName = null;
                                            for (int i = 1; i < int.MaxValue; i++)
                                            {
                                                varName = "var" + i;
                                                if (Graph.Variables.Where(o1 => o1.Name == varName).Count() == 0)
                                                    break;
                                            }

                                            object defaultValue = null;
                                            if (varType == typeof(Color))
                                                defaultValue = DefaultColor;
                                            else if (varType == typeof(Color32))
                                                defaultValue = DefaultColor32;
                                            else if (varType == typeof(AnimationCurve))
                                                defaultValue = DefaultCurve;
                                            else
                                                defaultValue = varType.CreateDefaultValue();

                                            Undo.RecordObject(Target, "Add Variable");
                                            Graph.AddVariable(varName, varType, defaultValue);
                                            SetTargetDirty();
                                            Repaint();
                                        }, t);
                                    }
                                }
                                addVariableMenu.ShowAsContext();
                            }
                            GUI.enabled = true;
                        }


                        for (int i = 0; i < Graph.Variables.Count; i++)
                        {
                            var variable = Graph.Variables[i];

                            using (new GUILayout.HorizontalScope())
                            {
                                GUI.enabled = !Application.isPlaying;

                                if (GUILayout.Button("", "box", GUILayout.Width(10), GUILayout.Height(10)))
                                {
                                    GenericMenu menu = new GenericMenu();

                                    menu.AddItem(new GUIContent("In"), variable.IsIn, (o) =>
                                    {
                                        VariableInfo varInfo = (VariableInfo)o;
                                        Undo.RecordObject(Target, null);
                                        if (varInfo.IsIn)
                                        {
                                            varInfo.Mode &= (~VariableMode.In);
                                        }
                                        else
                                        {
                                            varInfo.Mode |= VariableMode.In;
                                        }
                                        SetTargetDirty();
                                    }, variable);
                                    menu.AddItem(new GUIContent("Out"), variable.IsOut, (o) =>
                                    {
                                        VariableInfo varInfo = (VariableInfo)o;
                                        Undo.RecordObject(Target, null);
                                        if (varInfo.IsOut)
                                        {
                                            varInfo.Mode &= (~VariableMode.Out);
                                        }
                                        else
                                        {
                                            varInfo.Mode |= VariableMode.Out;
                                        }
                                        SetTargetDirty();
                                    }, variable);

                                    if (i == 0)
                                    {
                                        menu.AddDisabledItem(new GUIContent("Up"));
                                    }
                                    else
                                    {
                                        menu.AddItem(new GUIContent("Up"), false, (o) =>
                                       {
                                           int index = (int)o;
                                           VariableInfo varInfo = Graph.Variables[index];
                                           if (index > 0)
                                           {
                                               var list = GetVariableList(Graph);
                                               Undo.RecordObject(Target, null);
                                               list.RemoveAt(index);
                                               list.Insert(index - 1, varInfo);
                                               SetTargetDirty();
                                           }
                                       }, i);
                                    }
                                    if (i == Graph.Variables.Count - 1)
                                    {
                                        menu.AddDisabledItem(new GUIContent("Down"));
                                    }
                                    else
                                    {
                                        menu.AddItem(new GUIContent("Down"), false, (o) =>
                                        {
                                            int index = (int)o;
                                            VariableInfo varInfo = Graph.Variables[index];
                                            if (index < Graph.Variables.Count)
                                            {
                                                var list = GetVariableList(Graph);
                                                Undo.RecordObject(Target, null);
                                                list.RemoveAt(index);
                                                list.Insert(index + 1, varInfo);
                                                SetTargetDirty();
                                            }
                                        }, i);
                                    }
                                    menu.AddItem(new GUIContent("Delete"), false, (o) =>
                                    {
                                        VariableInfo varInfo = (VariableInfo)o;
                                        var list = GetVariableList(Graph);
                                        int index = list.IndexOf(varInfo);
                                        if (index >= 0)
                                        {
                                            Undo.RecordObject(Target, "Delete Variable");
                                            list.RemoveAt(index);
                                            SetTargetDirty();
                                            Repaint();
                                        }
                                    }, variable);
                                    menu.ShowAsContext();
                                    Event.current.Use();
                                }

                                using (new GUILayout.HorizontalScope())
                                {
                                    GUIStyle style = (GUIStyle)"label";
                                    var nameRect = GUILayoutUtility.GetRect(new GUIContent(variable.Name), style, GUILayout.Width(detailsFieldLabelWidth));

                                    if (nameRect.GUIDragControl())
                                    {
                                        DragAndDrop.PrepareStartDrag();
                                        DragAndDrop.SetGenericData(DragObjectName, variable);
                                        DragAndDrop.StartDrag("variable");
                                    }

                                    string newName = style.LabelEditable(nameRect, new GUIContent(variable.Name, string.Format("{0}({1})", variable.Name, FlowNode.GetValueTypeName(variable.Type))), GUILayout.Width(detailsFieldLabelWidth));
                                    if (!Application.isPlaying)
                                    {
                                        if (newName != variable.Name)
                                        {
                                            string oldName = variable.Name;
                                            Undo.RecordObject(Target, "Change Variable Name");
                                            typeof(VariableInfo).GetProperty("Name").SetValue(variable, newName, null);
                                            foreach (var node in Graph.Nodes)
                                            {
                                                if (node.Node == null)
                                                    continue;
                                                if (node.Node.GetType().IsGenericSubclassOf(typeof(VariableNode<>)))
                                                {
                                                    if (GetVariableName(node.Node) == oldName)
                                                    {
                                                        SetVariableName(node.Node, newName);
                                                    }
                                                }
                                            }
                                            SetTargetDirty();
                                        }
                                    }
                                }

                                GUI.enabled = true;
                                //if (variable.IsLocal || Application.isPlaying)
                                {
                                    object value;
                                    value = GetVariable(variable.Name);

                                    GUI.enabled = variable.Mode != VariableMode.In;
                                    object newValue = SerializableValuePropertyDrawer.LayoutValueField(value, variable.Type, GUILayout.ExpandWidth(true));
                                    GUI.enabled = true;
                                    if (!object.Equals(value, newValue))
                                    {
                                        SetVariable(variable.Name, newValue);
                                    }

                                    GUILayout.Label(GetInOutChar(variable.Mode));
                                }
                                //else
                                //{
                                //    GUI.enabled = false;
                                //    GUILayout.Label(variable.Type.Name);
                                //    GUILayout.FlexibleSpace();
                                //    GUILayout.Label(ArrowRightChar);
                                //    GUI.enabled = true;
                                //}
                            }

                        }
                    }
                }
            }

            if (Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseDown)
            {
                if (rect.Contains(Event.current.mousePosition))
                {
                    Event.current.Use();
                }
            }

        }



        private void TryDeserialize()
        {
            if (selected != null)
            {
                selected.OnAfterDeserialize();
            }
            Graph.OnAfterDeserialize();
        }


        List<VariableInfo> GetVariableList(FlowGraphData g)
        {
            var list = (List<VariableInfo>)Graph.GetType().GetField("variables", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Graph);
            return list;
        }

        void SetVariable(string name, object value)
        {
            VariableInfo variable = Graph.GetVariable(name);

            if (Application.isPlaying)
            {
                flowController.Context.SetVariable(name, value);
            }
            else
            {
                Undo.RecordObject(Target, "Change Variable Value");
                typeof(VariableInfo).GetProperty("DefaultValue").SetValue(variable, value, null);
                SetTargetDirty();
            }

        }


        object GetVariable(string name)
        {

            VariableInfo variable = Graph.GetVariable(name);
            object value = null;
            if (variable == null)
                return null;
            if (Application.isPlaying)
            {
                value = flowController.Context.GetVariable(name);
            }
            else
            {
                value = variable.DefaultValue;
            }
            return value;
        }

        public static string GetInOutChar(VariableMode mode)
        {
            string ch = " ";
            if (mode == VariableMode.InOut)
            {
                ch = InOutChar;
            }
            else if (mode == VariableMode.In)
            {
                ch = ArrowLeftChar;
            }
            else if (mode == VariableMode.Out)
            {
                ch = ArrowRightChar;
            }
            return ch;
        }






        FlowNodeInfo GetNodeInfo(FlowNode node)
        {
            FlowNodeInfo info;
            if (node is MethodNode)
            {
                var methodNode = (MethodNode)node;
                if (methodNode.Method == null)
                    return null;
                if (!flowNodeInfoMapMethods.TryGetValue(methodNode.Method, out info))
                {
                    if (!flowNodeInfoMaps.ContainsKey(methodNode.Method.DeclaringType))
                    {
                        ParseType(new FlowGraphConfig.IncludeTypeItem() { type = methodNode.Method.DeclaringType });
                        info = flowNodeInfoMapMethods[methodNode.Method];
                    }
                }
            }
            else
            {
                if (!flowNodeInfoMaps.TryGetValue(node.GetType(), out info))
                {
                    ParseType(new FlowGraphConfig.IncludeTypeItem() { type = node.GetType() });
                    info = flowNodeInfoMaps[node.GetType()];
                }
            }
            return info;
        }

        void DrawFlowNode(FlowNodeData node)
        {

            FlowNodeInfo nodeInfo = null;
            //if (node.Node == null)
            //    node.OnAfterDeserialize();

            if (node.Node != null)
                nodeInfo = GetNodeInfo(node.Node);

            Event evt = Event.current;
            int nodeHeight;

            nodeHeight = nameHeight;
            if (node.Node != null)
            {
                nodeHeight += Math.Max(node.Node.FlowInputs.Count, node.Node.FlowOutputs.Count) * portHeight;

            }
            nodeHeight += 5;
            if (node.Node != null)
            {
                nodeHeight += Math.Max(node.Node.ValueInputs.Count, node.Node.ValueOutputs.Count) * portHeight;
            }
            Rect rect = new Rect(windowCenter.x + node.Position.x, windowCenter.y + node.Position.y, nodeWidth, nodeHeight);
            if (node.Node is EventTrigger)

                if (selected == node)
                    GUI.backgroundColor = new Color(0, 0, 1, 0.8f);
                else
                    GUI.backgroundColor = Color.white;

            GUI.Box(rect, GUIContent.none);
            GUI.backgroundColor = Color.white;

            using (new GUILayout.AreaScope(rect))
            {

                if (nodeInfo == null)
                {
                    if (selected == node)
                    {
                        GUI.color = nodeNameSelectedColor;
                    }
                    else
                    {
                        GUI.color = Color.red;
                    }

                    nodeNameStyle.LabelFit(new GUIContent(node.DisplayTypeName, node.TypeName), nodeWidth);
                    GUI.color = Color.white;
                }
                else
                {

                    if (nodeInfo.NodeType == NodeType.Variable)
                    {
                        string varName = node.Node.GetType().GetProperty("Name").GetValue(node.Node, null) as string;
                        var variable = Graph.GetVariable(varName);
                        if (selected == node)
                        {
                            GUI.color = nodeNameSelectedColor;
                        }
                        else if (!(nodeInfo.Type == typeof(ObjectGetVariable) || nodeInfo.Type == typeof(ObjectSetVariable)))
                        {
                            if (variable == null)
                            {
                                GUI.color = nodeErrorColor;
                            }
                            else if (!nodeInfo.ValueType.IsAssignableFrom(variable.Type))
                            {
                                GUI.color = nodeErrorColor;
                            }
                            else
                            {
                                GUI.color = Color.white;
                            }
                        }
                        else
                        {
                            GUI.color = Color.white;
                        }

                        nodeNameStyle.LabelFit(new GUIContent(node.Node.GetDisplayName(), node.Node.GetDisplayNameDesc()), nodeWidth);
                    }
                    else if (nodeInfo.NodeType == NodeType.Value)
                    {
                        if (selected == node)
                        {
                            GUI.color = nodeNameSelectedColor;
                        }
                        else
                        {
                            GUI.color = Color.white;
                        }
                        object value = node.Node.GetType().GetProperty("Value").GetValue(node.Node, null);
                        if (nodeInfo.ValueType == typeof(Color))
                        {
                            GUI.enabled = false;
                            EditorGUILayout.ColorField(GUIContent.none, (Color)value, false, true, false);
                            GUI.enabled = true;
                        }
                        else if (nodeInfo.ValueType == typeof(AnimationCurve))
                        {
                            GUI.enabled = false;
                            EditorGUILayout.CurveField((AnimationCurve)value);
                            GUI.enabled = true;
                        }
                        else
                        {
                            nodeNameStyle.LabelFit(new GUIContent(node.Node.GetDisplayName(), node.Node.GetDisplayNameDesc()), nodeWidth);
                        }
                    }
                    else
                    {
                        if (selected == node)
                        {
                            GUI.color = nodeNameSelectedColor;
                        }
                        else
                        {
                            GUI.color = Color.black;
                        }
                        string name = nodeInfo.Name;
                        string desc = nodeInfo.Description;
                        if (node.Node != null)
                        {
                            FlowGraphAssetNode assetNode = node.Node as FlowGraphAssetNode;
                            if (assetNode != null)
                            {
                                if (assetNode.Asset)
                                    name = assetNode.Asset.name;
                            }
                            name = node.Node.GetDisplayName();
                            desc = node.Node.GetDisplayNameDesc();
                        }

                        nodeNameStyle.LabelFit(new GUIContent(name, desc), nodeWidth);
                    }

                    GUI.color = Color.white;
                }

            }


            if (node.Node != null)
            {

                float y;
                y = rect.y + 20;
                float leftY, rightY;

                leftY = y;
                for (int i = 0; i < node.Node.FlowInputs.Count; i++)
                {

                    var input = node.Node.FlowInputs[i];
                    leftY = DrawFlowInput(new Rect(rect.x, y, 0, portHeight), input).yMax;

                }
                rightY = y;
                for (int i = 0; i < node.Node.FlowOutputs.Count; i++)
                {
                    var output = node.Node.FlowOutputs[i];
                    rightY = DrawFlowOutput(new Rect(rect.xMax, rightY, 0, portHeight), output).yMax;

                }
                y = Mathf.Max(leftY, rightY);
                y += 5;


                leftY = y;
                for (int i = 0; i < node.Node.ValueInputs.Count; i++)
                {
                    var input = node.Node.ValueInputs[i];
                    if ((input.Flags & ValuePortFlags.Hidden) == ValuePortFlags.Hidden)
                        continue;
                    leftY = DrawValueInput(input).yMax;

                }

                rightY = y;
                for (int i = 0; i < node.Node.ValueOutputs.Count; i++)
                {
                    var output = node.Node.ValueOutputs[i];
                    if ((output.Flags & ValuePortFlags.Hidden) == ValuePortFlags.Hidden)
                        continue;
                    rightY = DrawValueOutput(output).yMax;
                }

                y = Mathf.Max(leftY, rightY);
            }

            if (evt.button == 0)
            {
                if (evt.type == EventType.MouseDown)
                {
                    if (rect.Contains(evt.mousePosition))
                    {
                        EditorGUIUtility.editingTextField = false;
                        selected = node;
                        downPort = null;
                        selectedPort = null;
                        isDraging = true;
                        dragOriginPos = node.Position;
                        dragOffset = node.Position - evt.mousePosition;
                        evt.Use();
                        Repaint();
                    }
                }
                if (selected == node)
                {
                    if (isDraging)
                    {
                        if (evt.type == EventType.MouseDrag)
                        {
                            node.Position = evt.mousePosition + dragOffset;
                            evt.Use();
                            Repaint();
                        }
                        else if (evt.type == EventType.MouseUp)
                        {
                            isDraging = false;
                            evt.Use();

                            if (dragOriginPos != node.Position)
                            {
                                var newPos = node.Position;
                                node.Position = dragOriginPos;
                                Undo.RecordObject(Target, "Move");
                                node.Position = newPos;
                                SetTargetDirty();
                            }
                        }
                    }

                }
            }

            bool isMouseHover = rect.Contains(evt.mousePosition);
            if (isMouseHover)
            {
                switch (evt.type)
                {
                    case EventType.DragUpdated:
                    case EventType.DragPerform:

                        if (evt.type != EventType.Used && DragAndDrop.GetGenericData("ValueInput") != null)
                        {
                            ValuePort input = (ValuePort)DragAndDrop.GetGenericData("ValueInput");
                            if (input.Node != node.Node)
                            {
                                if (evt.type == EventType.DragPerform)
                                {
                                    DragAndDrop.AcceptDrag();

                                    if (node.Node.ValueOutputs.Count > 1)
                                    {
                                        GenericMenu menu = new GenericMenu();
                                        foreach (var output in node.Node.ValueOutputs)
                                        {
                                            //if(!CanConvertType(input.ValueType, output.ValueType)){
                                            //    continue;
                                            //}

                                            menu.AddItem(new GUIContent(output.Name), false, (o) =>
                                            {
                                                var outputPort = (ValuePort)o;
                                                Undo.RecordObject(Target, null);
                                                graph.LinkValuePort(outputPort.Node, outputPort.Name, input.Node, input.Name);
                                                SetTargetDirty();
                                                dragTargetState = 0;
                                            }, output);
                                        }
                                        menu.ShowAsContext();
                                    }
                                    else
                                    {
                                        Undo.RecordObject(Target, null);
                                        graph.LinkValuePort(node.Node, node.Node.ValueOutputs[0].Name, input.Node, input.Name);
                                        SetTargetDirty();
                                        dragTargetState = 0;
                                    }
                                }
                                DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                                evt.Use();
                            }
                        }

                        if (evt.type != EventType.Used && DragAndDrop.GetGenericData("ValueOutput") != null)
                        {
                            ValuePort output = (ValuePort)DragAndDrop.GetGenericData("ValueOutput");
                            if (output.Node != node.Node)
                            {
                                if (evt.type == EventType.DragPerform)
                                {
                                    DragAndDrop.AcceptDrag();
                                    if (node.Node.ValueInputs.Count > 1)
                                    {
                                        GenericMenu menu = new GenericMenu();
                                        foreach (var input in node.Node.ValueInputs)
                                        {
                                            menu.AddItem(new GUIContent(input.Name), false, (o) =>
                                            {
                                                var inputPort = (ValuePort)o;
                                                Undo.RecordObject(Target, null);
                                                graph.LinkValuePort(output.Node, output.Name, inputPort.Node, inputPort.Name);
                                                SetTargetDirty();
                                                dragTargetState = 0;
                                            }, input);
                                        }
                                        menu.ShowAsContext();
                                    }
                                    else
                                    {
                                        Undo.RecordObject(Target, null);
                                        graph.LinkValuePort(output.Node, output.Name, node.Node, node.Node.ValueInputs[0].Name);
                                        SetTargetDirty();
                                        dragTargetState = 0;
                                    }
                                }
                                DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                                evt.Use();
                            }
                        }

                        if (evt.type != EventType.Used && DragAndDrop.GetGenericData("FlowInput") != null)
                        {
                            FlowInput input = (FlowInput)DragAndDrop.GetGenericData("FlowInput");
                            if (input.Node != node.Node)
                            {
                                if (evt.type == EventType.DragPerform)
                                {
                                    DragAndDrop.AcceptDrag();
                                    if (node.Node.FlowOutputs.Count > 1)
                                    {
                                        GenericMenu menu = new GenericMenu();
                                        foreach (var output in node.Node.FlowOutputs)
                                        {
                                            menu.AddItem(new GUIContent(output.Name), false, (o) =>
                                            {
                                                var outputPort = (FlowOutput)o;
                                                Undo.RecordObject(Target, null);
                                                graph.LinkFlowPort(outputPort.Node, outputPort.Name, input.Node, input.Name);
                                                SetTargetDirty();
                                                dragTargetState = 0;
                                            }, output);
                                        }
                                        menu.ShowAsContext();
                                    }
                                    else
                                    {
                                        Undo.RecordObject(Target, null);
                                        graph.LinkFlowPort(node.Node, node.Node.FlowOutputs[0].Name, input.Node, input.Name);
                                        SetTargetDirty();
                                        dragTargetState = 0;
                                    }
                                }
                                DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                                evt.Use();
                            }
                        }

                        if (evt.type != EventType.Used && DragAndDrop.GetGenericData("FlowOutput") != null)
                        {
                            FlowOutput output = (FlowOutput)DragAndDrop.GetGenericData("FlowOutput");
                            if (output.Node != node.Node)
                            {
                                if (evt.type == EventType.DragPerform)
                                {
                                    DragAndDrop.AcceptDrag();
                                    if (node.Node.FlowInputs.Count > 1)
                                    {
                                        GenericMenu menu = new GenericMenu();
                                        foreach (var input in node.Node.FlowInputs)
                                        {
                                            menu.AddItem(new GUIContent(input.Name), false, (o) =>
                                            {
                                                var inputPort = (FlowOutput)o;
                                                Undo.RecordObject(Target, null);
                                                graph.LinkFlowPort(output.Node, output.Name, inputPort.Node, inputPort.Name);
                                                SetTargetDirty();
                                                dragTargetState = 0;

                                            }, input);
                                        }
                                        menu.ShowAsContext();
                                    }
                                    else
                                    {
                                        Undo.RecordObject(Target, null);
                                        graph.LinkFlowPort(output.Node, output.Name, node.Node, node.Node.FlowInputs[0].Name);
                                        SetTargetDirty();
                                        dragTargetState = 0;
                                    }
                                }
                                DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                                evt.Use();
                            }
                        }
                        evt.Use();
                        break;
                }
            }

        }

        object copy;

        void DrawPort(Rect rect, object port, bool isFlow, bool isInput)
        {
            bool isSelected = selectedPort == port;
            if (isSelected)
            {
                GUI.backgroundColor = portSelectedColor;
            }
            else
            {
                GUI.backgroundColor = portNormalColor;
            }
            GUI.Box(new Rect(rect.x, rect.y + (rect.height - portWidth) * 0.5f, rect.width, portWidth), "");
            GUI.backgroundColor = Color.white;
            Event evt = Event.current;

            if (evt.button == 0)
            {
                if (evt.type == EventType.MouseDown)
                {
                    if (rect.Contains(evt.mousePosition))
                    {
                        downPort = port;
                        dragTargetState = 1;

                        evt.Use();
                    }
                }
                if (downPort == port)
                {
                    if (evt.type == EventType.MouseDrag)
                    {
                        if (dragTargetState == 1)
                        {
                            if (rect.Contains(evt.mousePosition) && downPort != null)
                            {
                                dragTargetState = 2;

                                DragAndDrop.PrepareStartDrag();
                                string dragType;
                                if (isFlow)
                                {
                                    if (isInput)
                                        dragType = "FlowInput";
                                    else
                                        dragType = "FlowOutput";
                                }
                                else
                                {
                                    if (isInput)
                                        dragType = "ValueInput";
                                    else
                                        dragType = "ValueOutput";
                                }
                                DragAndDrop.SetGenericData(DragObjectName, port);
                                DragAndDrop.SetGenericData(dragType, port);
                                DragAndDrop.StartDrag(dragType);
                            }
                            else
                            {
                                dragTargetState = 0;
                            }

                        }

                    }
                    else if (evt.type == EventType.MouseUp)
                    {
                        if (dragTargetState == 1)
                        {
                            selectedPort = downPort;
                            if (port is ValuePort)
                            {
                                var valuePort = ((ValuePort)port);
                                var n = Graph.ToFlowNodeData(valuePort.Node);
                                if (n != selected)
                                {
                                    selected = n;
                                }
                            }
                            else if (port is FlowInput)
                            {
                                var flowInput = ((FlowInput)port);
                                var n = Graph.ToFlowNodeData(flowInput.Node);
                                if (n != selected)
                                {
                                    selected = n;
                                }
                            }
                            else if (port is FlowOutput)
                            {
                                var flowOutput = ((FlowOutput)port);
                                var n = Graph.ToFlowNodeData(flowOutput.Node);
                                if (n != selected)
                                {
                                    selected = n;
                                }
                            }


                        }

                    }
                }
            }
        }


        Rect DrawFlowInput(Rect rect, FlowInput flowInput)
        {
            rect = GetFlowInputPortPosition(flowInput);
            DrawPort(rect, flowInput, true, true);
            float x = rect.xMax + portLabelSpace;
            var size = leftPortLabelStyle.CalcSize(new GUIContent(ArrowRightChar));
            GUI.Label(new Rect(x, rect.y, size.x, rect.height), new GUIContent(ArrowRightChar));
            x += size.x;
            if (flowInput.Name != FlowNode.FLOW_IN)
            {
                size = leftPortLabelStyle.CalcSize(new GUIContent(flowInput.Name));
                GUI.Label(new Rect(x, rect.y, size.x, rect.height), flowInput.Name);
            }
            return rect;
        }

        Rect DrawFlowOutput(Rect rect, FlowOutput flowOutput)
        {
            rect = GetFlowOutputPortPosition(flowOutput);
            DrawPort(rect, flowOutput, true, false);

            float x = rect.x - portLabelSpace;
            var size = rightPortLabelStyle.CalcSize(new GUIContent(ArrowRightChar));
            x -= size.x;
            GUI.Label(new Rect(x, rect.y, size.x, rect.height), new GUIContent(ArrowRightChar));

            if (flowOutput.Name != FlowNode.FLOW_OUT)
            {
                size = rightPortLabelStyle.CalcSize(new GUIContent(flowOutput.Name));
                x -= size.x;
                GUI.Label(new Rect(x, rect.y, size.x, rect.height), flowOutput.Name);
            }
            //if (Event.current.type == EventType.KeyDown)
            //{
            //    if (Event.current.keyCode == KeyCode.Delete)
            //    {
            //        if (selectedPort == flowOutput)
            //        {
            //            if (flowOutput.LinkInput != null)
            //            {
            //                Undo.RecordObject(Target, "Delete Link");
            //                Graph.BreakFlowPort(flowOutput);
            //            }
            //            Event.current.Use();
            //        }
            //    }
            //}
            return rect;
        }

        string GetInjectDisplay(Inject inject)
        {
            string str = "";
            if (inject.Name != null)
                str = "$" + inject.Name;
            if (inject.Type != null)
                str += "(" + inject.Type.Name + ")";
            else if (inject.ValueType != InjectValueType.None)
            {
                str += "(" + inject.ValueType + ")";
            }
            return str;
        }

        Rect DrawValueInput(ValuePort valueInput)
        {
            string name;
            name = valueInput.Name;
            GUIContent nameContent = new GUIContent(name, name + "(" + FlowNode.GetValueTypeName(valueInput.ValueType, valueInput.ValueType.Name) + ")");
            var size = leftPortLabelStyle.CalcSize(nameContent);
            Rect rect;
            rect = GetValueInputPortPosition(valueInput);
            size.x = Mathf.Min(size.x, portLabelWidth);
            DrawPort(rect, valueInput, false, true);
            GUI.Label(new Rect(rect.xMax + portLabelSpace, rect.y, size.x, rect.height), nameContent);

            if (valueInput.LinkOutput == null)
            {
                if (valueInput.Inject != null)
                {
                    string defaultValue = GetInjectDisplay(valueInput.Inject);

                    GUIContent content;
                    content = new GUIContent(defaultValue);
                    size = portInputDefaultStyle.CalcSize(content);
                    GUI.Label(new Rect(rect.x - size.x, rect.y, size.x, size.y), content, portInputDefaultStyle);
                }
            }
            //if (Event.current.type == EventType.KeyDown)
            //{
            //    if (Event.current.keyCode == KeyCode.Delete)
            //    {
            //        if (selectedPort == valueInput)
            //        {
            //            if (valueInput.LinkOutput != null)
            //            {
            //                Undo.RecordObject(Target, "Delete Link");
            //                Graph.BreakValuePort(valueInput);
            //            }
            //            Event.current.Use();
            //        }
            //    }
            //}
            return rect;
        }
        Rect DrawValueOutput(ValuePort valueOutput)
        {
            string name;
            name = valueOutput.Name;
            GUIContent nameContent = new GUIContent(name, name + "(" + FlowNode.GetValueTypeName(valueOutput.ValueType, valueOutput.ValueType.Name) + ")");
            var size = leftPortLabelStyle.CalcSize(nameContent);
            Rect rect;
            rect = GetValueOutputPortPosition(valueOutput);
            size.x = Mathf.Min(size.x, portLabelWidth);
            DrawPort(rect, valueOutput, false, false);
            GUI.Label(new Rect(rect.x - size.x - portLabelSpace, rect.y, size.x, rect.height), nameContent);


            return rect;
        }

        FlowInput ScreenPointToFlowInput(Vector2 screenPoint)
        {
            foreach (var node in Graph.Nodes)
            {
                if (node.Node == null)
                    continue;
                for (int i = 0; i < node.Node.FlowInputs.Count; i++)
                {
                    Rect rect = GetFlowInputPortPosition(node.Node, i);
                    if (rect.Contains(screenPoint))
                    {
                        return node.Node.FlowInputs[i];
                    }
                }
            }
            return null;
        }
        FlowOutput ScreenPointToFlowOutput(Vector2 screenPoint)
        {
            foreach (var node in Graph.Nodes)
            {
                if (node.Node == null)
                    continue;
                for (int i = 0; i < node.Node.FlowOutputs.Count; i++)
                {
                    Rect rect = GetFlowOutputPortPosition(node.Node, i);
                    if (rect.Contains(screenPoint))
                    {
                        return node.Node.FlowOutputs[i];
                    }
                }
            }
            return null;
        }
        ValuePort ScreenPointToValueInput(Vector2 screenPoint)
        {
            foreach (var node in Graph.Nodes)
            {
                if (node.Node == null)
                    continue;
                for (int i = 0; i < node.Node.ValueInputs.Count; i++)
                {
                    Rect rect = GetValueInputPortPosition(node.Node, i);
                    if (rect.Contains(screenPoint))
                    {
                        return node.Node.ValueInputs[i];
                    }
                }
            }
            return null;
        }
        ValuePort ScreenPointToValueOutput(Vector2 screenPoint)
        {
            foreach (var node in Graph.Nodes)
            {
                if (node.Node == null)
                    continue;
                for (int i = 0; i < node.Node.ValueOutputs.Count; i++)
                {
                    Rect rect = GetValueOutputPortPosition(node.Node, i);
                    if (rect.Contains(screenPoint))
                    {
                        return node.Node.ValueOutputs[i];
                    }
                }
            }
            return null;
        }

        Rect GetFlowInputPortPosition(FlowNode node, int index)
        {
            var visualNode = Graph.ToFlowNodeData(node);

            if (node == null || visualNode == null)
                return new Rect();
            Vector2 pos = visualNode.Position;
            pos.x += portOffset;
            pos.y += nameHeight;
            pos.y += index * portHeight;
            pos += windowCenter;
            return new Rect(pos.x, pos.y, portWidth, portHeight);
        }
        Rect GetFlowInputPortPosition(FlowInput input)
        {
            return GetFlowInputPortPosition(input.Node, input.Name);
        }
        Rect GetFlowInputPortPosition(FlowNode node, string portName)
        {
            int index = 0;
            for (int i = 0; i < node.FlowInputs.Count; i++)
            {
                if (node.FlowInputs[i].Name == portName)
                {
                    index = i;
                    break;
                }
            }
            return GetFlowInputPortPosition(node, index);
        }

        Rect GetFlowOutputPortPosition(FlowNode node, int index)
        {
            var visualNode = Graph.ToFlowNodeData(node);
            Vector2 pos = visualNode.Position;
            if (node == null)
                return new Rect();

            pos.x = pos.x + nodeWidth - portOffset;
            pos.y += nameHeight;
            pos.y += index * portHeight;
            pos += windowCenter;
            return new Rect(pos.x - portWidth, pos.y, portWidth, portHeight);
        }
        Rect GetFlowOutputPortPosition(FlowOutput output)
        {
            return GetFlowOutputPortPosition(output.Node, output.Name);
        }
        Rect GetFlowOutputPortPosition(FlowNode node, string portName)
        {
            int index = 0;
            for (int i = 0; i < node.FlowOutputs.Count; i++)
            {
                if (node.FlowOutputs[i].Name == portName)
                {
                    index = i;
                    break;
                }
            }
            return GetFlowOutputPortPosition(node, index);
        }

        Rect GetValueInputPortPosition(FlowNode node, int portIndex)
        {
            var visualNode = Graph.ToFlowNodeData(node);
            Vector2 pos = visualNode.Position;
            if (node == null)
                return new Rect();
            pos.x += portOffset;
            pos.y += nameHeight + (Mathf.Max(node.FlowInputs.Count, node.FlowOutputs.Count) * portHeight) + lineSpaceHeight;

            pos.y += portIndex * portHeight;
            pos += windowCenter;
            return new Rect(pos.x, pos.y, portWidth, portHeight);
        }
        Rect GetValueInputPortPosition(ValuePort input)
        {
            int index = 0;
            for (int i = 0, j = 0; i < input.Node.ValueInputs.Count; i++)
            {
                if ((input.Node.ValueInputs[i].Flags & ValuePortFlags.Hidden) == ValuePortFlags.Hidden)
                    continue;

                if (input.Node.ValueInputs[i].Name == input.Name)
                {
                    index = j;
                    break;
                }
                j++;
            }
            return GetValueInputPortPosition(input.Node, index);
        }
        Rect GetValueOutputPortPosition(FlowNode node, int index)
        {
            var visualNode = Graph.ToFlowNodeData(node);
            Vector2 pos = visualNode.Position;
            if (node == null)
                return new Rect();
            pos.x += nodeWidth - portWidth - portOffset;
            pos.y += nameHeight + (Mathf.Max(node.FlowInputs.Count, node.FlowOutputs.Count) * portHeight) + lineSpaceHeight;

            pos.y += index * portHeight;
            pos += windowCenter;
            return new Rect(pos.x, pos.y, portWidth, portHeight);
        }

        Rect GetValueOutputPortPosition(ValuePort output)
        {
            int index = 0;
            for (int i = 0, j = 0; i < output.Node.ValueOutputs.Count; i++)
            {
                if ((output.Node.ValueOutputs[i].Flags & ValuePortFlags.Hidden) == ValuePortFlags.Hidden)
                    continue;
                if (output.Node.ValueOutputs[i].Name == output.Name)
                {
                    index = j;
                    break;
                }
                j++;
            }
            return GetValueOutputPortPosition(output.Node, index);
        }



        FlowNodeData CreateNode(Type nodeType, Vector2 position)
        {
            return CreateNode(flowNodeInfoMaps[nodeType], position);
        }
        FlowNodeData CreateNode(FlowNodeInfo nodeInfo, Vector2 position)
        {
            FlowNode node;

            if (nodeInfo.method != null)
            {
                node = new CallMethod(nodeInfo.method);
            }
            else
            {
                node = Activator.CreateInstance(nodeInfo.Type, true) as FlowNode;
            }
            if (node is ColorValue)
                ((ColorValue)node).Value = DefaultColor;
            else if (node is AnimationCurveValue)
                ((AnimationCurveValue)node).Value = DefaultCurve;

            return CreateNode(nodeInfo, node, position);
        }
        FlowNodeData CreateNode(FlowNodeInfo nodeInfo, FlowNode node, Vector2 position)
        {

            Undo.RecordObject(Target, "Create Node");

            var vNode = Graph.AddNode(node);
            vNode.Position = position;

            SetTargetDirty();

            return vNode;
        }




        bool IsValueNode(FlowNode node)
        {
            if (node.GetType().IsGenericSubclassOf(typeof(InputableValueNode<>)))
                return true;
            return false;
        }
        bool IsVariableNode(FlowNode node)
        {
            if (node.GetType().IsGenericSubclassOf(typeof(VariableNode<>)))
                return true;
            return false;
        }

        private ValuePort typeMenuThisOut;
        private Vector2 typeMenuPosition;

        GenericMenu ShowTypeMenu(Type type, ValuePort thisOut, Vector2 mousePosition)
        {
            GenericMenu menu;
            if (typeMenus == null)
                typeMenus = new Dictionary<Type, GenericMenu>();
            if (!typeMenus.TryGetValue(type, out menu))
            {
                menu = new GenericMenu();
                string menuItemName;
                foreach (var pInfo in type.GetProperties().OrderBy(o => o.Name))
                {
                    //Network auto generate property
                    if (pInfo.Name.StartsWith("Network"))
                    {
                        var f = type.GetField(pInfo.Name.Substring(7), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        if (f != null && f.IsDefined(typeof(UnityEngine.Networking.SyncVarAttribute), false))
                            continue;
                    }

                    menuItemName = pInfo.DeclaringType.Name + "/" + pInfo.Name;
                    if (pInfo.CanRead)
                    {
                        if (pInfo.GetGetMethod().IsStatic)
                            continue;
                        menu.AddItem(new GUIContent("Get/" + menuItemName, "Property"), false, (o) =>
                        {
                            PropertyInfo p = (PropertyInfo)o;
                            var node = new GetProperty(p);

                            Undo.RecordObject(Target, null);
                            CreateNode(flowNodeInfoMaps[node.GetType()], node, WindowPositionToGraphPosition(typeMenuPosition));
                            if (node.ThisIn != null && typeMenuThisOut != null)
                            {
                                graph.LinkValuePort(typeMenuThisOut.Node, typeMenuThisOut.Name, node, node.ThisIn.Name);
                            }

                            SetTargetDirty();
                        }, pInfo);
                    }

                    if (pInfo.CanWrite)
                    {
                        if (pInfo.GetSetMethod().IsStatic)
                            continue;
                        menu.AddItem(new GUIContent("Set/" + menuItemName, "Property"), false, (o) =>
                        {
                            PropertyInfo p = (PropertyInfo)o;
                            var node = new SetProperty(p);

                            Undo.RecordObject(Target, null);
                            CreateNode(flowNodeInfoMaps[node.GetType()], node, WindowPositionToGraphPosition(typeMenuPosition));
                            if (node.ThisIn != null && typeMenuThisOut != null)
                            {
                                graph.LinkValuePort(typeMenuThisOut.Node, typeMenuThisOut.Name, node, node.ThisIn.Name);
                            }

                            SetTargetDirty();
                        }, pInfo);
                    }
                }
                foreach (var fInfo in type.GetFields().OrderBy(o => o.Name))
                {

                    if (fInfo.IsStatic)
                        continue;

                    menuItemName = fInfo.DeclaringType.Name + "/" + fInfo.Name;

                    menu.AddItem(new GUIContent("Get/" + menuItemName, "Field"), false, (o) =>
                    {
                        FieldInfo p = (FieldInfo)o;
                        var node = new GetField(p);

                        Undo.RecordObject(Target, null);
                        CreateNode(flowNodeInfoMaps[node.GetType()], node, WindowPositionToGraphPosition(typeMenuPosition));
                        if (node.ThisIn != null && typeMenuThisOut != null)
                        {
                            graph.LinkValuePort(typeMenuThisOut.Node, typeMenuThisOut.Name, node, node.ThisIn.Name);
                        }

                        SetTargetDirty();
                    }, fInfo);


                    if (!fInfo.IsInitOnly)
                    {

                        menu.AddItem(new GUIContent("Set/" + menuItemName, "Field"), false, (o) =>
                        {
                            FieldInfo p = (FieldInfo)o;
                            var node = new SetField(p);

                            Undo.RecordObject(Target, null);
                            CreateNode(flowNodeInfoMaps[node.GetType()], node, WindowPositionToGraphPosition(typeMenuPosition));
                            if (node.ThisIn != null && typeMenuThisOut != null)
                            {
                                graph.LinkValuePort(typeMenuThisOut.Node, typeMenuThisOut.Name, node, node.ThisIn.Name);
                            }

                            SetTargetDirty();
                        }, fInfo);
                    }
                }
                typeMenus[type] = menu;
            }
            typeMenuThisOut = thisOut;
            typeMenuPosition = mousePosition;
            if (menu.GetItemCount() > 0)
                menu.ShowAsContext();
            return menu;
        }

    }


}