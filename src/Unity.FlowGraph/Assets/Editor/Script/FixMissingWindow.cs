using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;

public class FixMissingWindow : EditorWindow
{

    [MenuItem("Window/General/Fix Missing")]
    public static void Open()
    {
        GetWindow<FixMissingWindow>().Show();
    }


    List<PartInfo> files = new List<PartInfo>();
    string input;
    string replace;
    Vector2 scrollPos;
    string[] extensions;
    string extensionStr = ".prefab;.asset;.meta;.unity";

    private void OnGUI()
    {

        GameObject go = Selection.activeGameObject;
        Component[] missingCpts = null;
        //if (GUILayout.Button("AA"))
        //{
        //    if (go)
        //    {
        //        SerializedObject so = new SerializedObject(go);
        //        var it = so.GetIterator();
        //        while (it.NextVisible(true))
        //        {
        //            Debug.Log(it.propertyPath + "," + it.propertyType);
        //        }

        //        foreach (var cpt in go.GetComponentsInChildren<Component>(true))
        //        {
        //            string guid;
        //            long localId;
        //            if (!cpt)
        //            {
        //                Debug.Log(AssetDatabase.GetAssetPath(cpt));

        //                Debug.Log(AssetDatabase.TryGetGUIDAndLocalFileIdentifier(cpt.GetInstanceID(), out guid, out localId));
        //                Debug.Log(guid + "  ," + localId);
        //            }
        //        }
        //    }
        //}

        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Label("Extension", GUILayout.ExpandWidth(false));
            extensionStr = EditorGUILayout.TextField(extensionStr ?? "");
            if (GUI.changed)
            {
                extensions = extensionStr.Split(';');
                GUI.changed = false;
            }
        }

        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Label("Input", GUILayout.ExpandWidth(false));
            input = EditorGUILayout.TextField(input ?? "");

            GUI.enabled = input.Length > 0;
            if (GUILayout.Button("Find", GUILayout.ExpandWidth(false)))
            {
                if (input.Length == 0)
                    return;
                files.Clear();

                if (extensions == null)
                    extensions = extensionStr.Split(';');
                List<int> indexs = new List<int>();
                foreach (var file in Directory.GetFiles("Assets", "*", SearchOption.AllDirectories))
                {
                    string fileLower = file.ToLower();
                    if (extensions.Where(o => fileLower.EndsWith(o)).FirstOrDefault() == null)
                        continue;
                    indexs.Clear();
                    string assetPath;
                    assetPath = file;

                    string[] lines = File.ReadAllLines(assetPath, Encoding.UTF8);

                    for (int i = 0; i < lines.Length; i++)
                    {
                        string line = lines[i];
                        if (i == 141)
                        {

                        }
                        if (line == input)
                        {
                            indexs.Add(i);
                        }
                    }

                    if (indexs.Count > 0)
                    {

                        files.Add(new PartInfo()
                        {
                            path = file,
                            indexs = indexs.ToArray(),
                        });

                    }
                }
            }
            GUI.enabled = true;
        }


        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Label("Replace", GUILayout.ExpandWidth(false));
            replace = EditorGUILayout.TextField(replace ?? "");

            GUI.enabled = files.Count > 0;
            if (GUILayout.Button("Replace", GUILayout.ExpandWidth(false)))
            {
                int n = 0;
                string repText = replace;
                foreach (var item in files)
                {
                    string[] lines = File.ReadAllLines(item.path, Encoding.UTF8);
                    for (int i = 0; i < item.indexs.Length; i++)
                    {
                        int index = item.indexs[i];
                        //string origin = lines[index];
                        //   origin.Substring(input.Length);
                        lines[index] = repText;
                        n++;
                    }
                    File.WriteAllLines(item.path, lines, Encoding.UTF8);
                    AssetDatabase.ImportAsset(item.path, ImportAssetOptions.ForceUpdate);
                }
                Debug.LogFormat("replace file :{0}, position:{1}", files.Count, n);
                files.Clear();

            }
            GUI.enabled = true;
        }

        using (var sv = new GUILayout.ScrollViewScope(scrollPos))
        {
            scrollPos = sv.scrollPosition;

            foreach (var file in files)
            {
                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button(file.path, "label"))
                    {
                        if (!file.path.EndsWith(".meta"))
                        {
                            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(file.path, typeof(Object)));
                        }
                    }
                }
            }
        }



    }

    class PartInfo
    {
        public string path;
        public int[] indexs;



    }

}
