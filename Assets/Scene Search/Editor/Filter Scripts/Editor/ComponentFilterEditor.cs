using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(SceneSearch.Filters.ComponentFilter))]
public class ComponentFilterEditor : Editor {
    static Component component;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SceneSearch.Filters.ComponentFilter compFilter = serializedObject.targetObject as SceneSearch.Filters.ComponentFilter;
        SceneSearch.Filters.ComponentFilter.ComponentInfo info;
        for (int i = 0; i < compFilter.info.Count; i++) 
        {
            info = compFilter.info[i];
            if (info.type == null) info.UpdateTypeFromGUID();
            if (info.type != null)
            {
                if (info.isMonoBehaviour)
                {
                    using (var scope = new EditorGUILayout.VerticalScope("Box"))
                    {
                        using (var titleScope = new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.LabelField(info.type.FullName);
                            if (GUILayout.Button("Show Script"))
                            {
                                string path = AssetDatabase.GUIDToAssetPath(info.GUID);
                                System.Type type = AssetDatabase.GetMainAssetTypeAtPath(path);
                                if (type == typeof(MonoScript))
                                {
                                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<MonoScript>(path));
                                }
                            }
                        }
                        //EditorGUILayout.TextField("GUID:", info.GUID);
                        if (GUILayout.Button("Copy GUID:" + info.GUID))
                        {
                            EditorGUIUtility.systemCopyBuffer = info.GUID;
                            Debug.Log("GUID Copied " + info.GUID);
                        }
                        if (GUILayout.Button("Remove"))
                        {
                            compFilter.info.RemoveAt(i);
                            i--;
                        }

                    }
                }
                else
                {
                    using (var scope = new EditorGUILayout.VerticalScope("Box"))
                    {
                        EditorGUILayout.LabelField(info.type.FullName);
                        if (GUILayout.Button("Remove"))
                        {
                            compFilter.info.RemoveAt(i);
                            i--;
                        }
                    }
                }

            }
        }
        using (var addScope = new GUILayout.HorizontalScope())
        {
            component = (Component)EditorGUILayout.ObjectField(component, typeof(Component), true);
            if (component != null)
            {
                if (GUILayout.Button("Add") && component != null)
                {
                    compFilter.info.Add(new SceneSearch.Filters.ComponentFilter.ComponentInfo(component));
                }
            }
        }
    }

}
