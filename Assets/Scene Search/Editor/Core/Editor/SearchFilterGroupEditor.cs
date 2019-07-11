using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace SceneSearch
{
    namespace Filters
    {
        [CustomEditor(typeof(SearchFilterGroup))]
        public class FilterGroupEditor : Editor
        {
            static Editor editor = null;
            static SearchFilter nullSearchFilter;
            static SearchFilter addSearchFilter;

            #region Functions
            [InitializeOnLoadMethod]
            static void SetNullSearchFilter()
            {
                if (nullSearchFilter == null) nullSearchFilter = CreateInstance<SearchFilter>();
            }
            public override void OnInspectorGUI()
            {
                using (var vertScope = new EditorGUILayout.VerticalScope())
                {
                    EditorGUI.indentLevel++;
                    serializedObject.Update();
                    SearchFilterGroup group = (serializedObject.targetObject as SearchFilterGroup);
                    SearchFilter[] filters = group.searchFilters.ToArray();
                    SearchFilterGroup groupTest;
                    bool missing = false;
                    for (int i = 0; i < filters.Length; i++)
                    {
                        missing = false;
                        using (var filterScope = new EditorGUILayout.VerticalScope("Box"))
                        {
                            if (filters[i] != null)
                            {
                                groupTest = filters[i] as SearchFilterGroup;
                                if (groupTest != null)
                                {
                                    editor = CreateEditor(groupTest);
                                }
                                else
                                {
                                    editor = CreateEditor(filters[i]);
                                }
                            }
                            else
                            {
                                editor = CreateEditor(nullSearchFilter);
                                missing = true;
                            }
                            editor.OnInspectorGUI();
                            if (missing)
                            {
                                Color col = GUI.contentColor;
                                Color bCol = GUI.backgroundColor;
                                GUI.backgroundColor = Color.red;
                                GUI.contentColor = Color.red;
                                if (GUILayout.Button("Missing reference to filter click to remove")) group.Remove(filters[i]);
                                GUI.contentColor = col;
                                GUI.backgroundColor = bCol;

                            }
                            else
                            {
                                if (GUILayout.Button("Remove"))
                                {
                                    group.Remove(filters[i]);
                                }
                            }
                        }
                        GUILayout.Space(10);
                    }
                    using (var horScope = new EditorGUILayout.HorizontalScope("Box"))
                    {
                        bool hit = GUILayout.Button("Add filter");
                        addSearchFilter = (SearchFilter)EditorGUILayout.ObjectField(addSearchFilter, typeof(SearchFilter), true);
                        if (addSearchFilter != null && hit)
                        {
                            group.AddSearchFilter(addSearchFilter);
                            addSearchFilter = null;
                            EditorUtility.SetDirty(this);
                        }
                    }
                }
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(serializedObject.targetObject);
            }
            #endregion
        }
    }
}
