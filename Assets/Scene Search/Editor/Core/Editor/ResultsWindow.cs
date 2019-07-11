using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SceneSearch.Filters;
namespace SceneSearch
{
    /// <summary>
    /// Used to hold and display the results and filters of a search 
    /// </summary>
    public class ResultsWindow : EditorWindow
    {
        public SearchFilter Filter;
        [System.Serializable]
        public class SelectionEntry
        {
            public bool Selected = true;
            public GameObject GameObject;
            public int InstanceID;
            public SelectionEntry(GameObject gameObject)
            {
                GameObject = gameObject;
                if (gameObject != null) InstanceID = gameObject.GetInstanceID();
            }
            public bool FindGameObjectReference()
            {
                Object instance = EditorUtility.InstanceIDToObject(InstanceID);
                if (instance != null)
                {
                    GameObject = instance as GameObject;
                    return true;
                }
                return false;
            }
        }
        public List<SelectionEntry> SelectionEntries;
        public bool showSearchFilter = false;
        Vector2 scrollPos = new Vector2();
        static GUILayoutOption buttonLayout = GUILayout.Width(100);

        #region Functions

        #region GUI Functions
        public static void DisplayResult(List<GameObject> results, SearchFilter filter = null, bool dock = true)
        {
            // Window setup
#if UNITY_2019_OR_NEWER
            OutputWindow window = dock ? CreateWindow<OutputWindow>(typeof(SearchWindow)) : CreateWindow<OutputWindow>();
#else 
            ResultsWindow window = CreateInstance<ResultsWindow>();
#endif
            window.minSize = new Vector2(400, 200);
            window.titleContent.text = "Search Results";
            // Filter
            if (filter == null) filter = CreateInstance<SearchFilter>(); // new empty filter group
            else filter = filter.DeepCopy(); // copy filter group to preserve settings
            window.Filter = filter;
            // Results
            List<SelectionEntry> selection = new List<SelectionEntry>();
            foreach (GameObject result in results)
            {
                selection.Add(new SelectionEntry(result));
            }
            window.SelectionEntries = selection;
            window.Show();
        }
        void OnGUI()
        {
            // Buttons
            DisplayFilter();
            DisplayButtons();
            DisplayResults();
        }
        void DisplayResults()
        {
            SelectionEntry selectionEntry;
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            for (int i = 0; i < SelectionEntries.Count; i++)
            {
                selectionEntry = SelectionEntries[i];
                if (selectionEntry.GameObject != null)
                {
                    using (var ResultScope = new EditorGUILayout.HorizontalScope("Box"))
                    {
                        if (selectionEntry.GameObject == null) selectionEntry.FindGameObjectReference(); // attempt to fix references that are bad
                        if (selectionEntry.GameObject != null)
                        {
                            if (GUILayout.Button("Ping", GUILayout.Width(40))) EditorGUIUtility.PingObject(selectionEntry.GameObject);
                            SelectionEntries[i].Selected = GUILayout.Toggle(selectionEntry.Selected, selectionEntry.GameObject.name);
                        }
                        else
                        {
                            // This should never be hit but it's here anyway
                            if (GUILayout.Button("Remove From Results", GUILayout.Width(80)))
                            {
                                SelectionEntries.RemoveAt(i);
                                i--;
                            }
                            GUILayout.Label("Missing GameObject");
                        }
                    }
                }
            }
            GUILayout.EndScrollView();
        }
        void DisplayFilter()
        {
            showSearchFilter = GUILayout.Toggle(showSearchFilter, "Display Search Filters");
            if (showSearchFilter)
            {
                using (var filterScope = new EditorGUILayout.VerticalScope("Box"))
                {
                    GUI.enabled = false;
                    Editor filterEditor;
                    filterEditor = Editor.CreateEditor(Filter);
                    filterEditor.OnInspectorGUI();
                    GUI.enabled = true;
                }
            }
        }
        void DisplayButtons()
        {
            using (var ButtonsScope = new EditorGUILayout.VerticalScope())
            {
                using (var buttonsScope = new EditorGUILayout.HorizontalScope("Box"))
                {
                    GUILayout.Label("Selection");
                    if (GUILayout.Button("Remove From", buttonLayout)) RemoveFrom();
                    if (GUILayout.Button("Add To", buttonLayout)) AddTo();
                    if (GUILayout.Button("Select", buttonLayout)) Select();
                }
                using (var buttonsScope = new EditorGUILayout.HorizontalScope("Box"))
                {
                    GUILayout.Label("Check");
                    if (GUILayout.Button("All", buttonLayout)) SetChecks(true);
                    if (GUILayout.Button("None", buttonLayout)) SetChecks(false);
                    if (GUILayout.Button("Invert", buttonLayout)) Invert();
                }
            }
        }
        #endregion

        #region Utilities
        private void OnEnable()
        {
            EditorApplication.quitting += Close;
        }
        private void OnDestroy()
        {
            EditorApplication.quitting -= Close;
        }
        void SetChecks(bool check)
        {
            for (int i = 0; i < SelectionEntries.Count; i++)
            {
                SelectionEntries[i].Selected = check;
            }
        }
        void Invert()
        {
            for (int i = 0; i < SelectionEntries.Count; i++)
            {
                SelectionEntries[i].Selected = !SelectionEntries[i].Selected;
            }
        }
        void RemoveFrom()
        {
            List<Object> objects = new List<Object>(Selection.objects);
            for (int i = 0; i < SelectionEntries.Count; i++)
            {
                if (SelectionEntries[i].Selected) objects.Remove(SelectionEntries[i].GameObject);
            }
            Selection.objects = objects.ToArray();
        }
        void AddTo()
        {
            List<Object> objects = new List<Object>(Selection.objects);
            for (int i = 0; i < SelectionEntries.Count; i++)
            {
                if (SelectionEntries[i].Selected && !objects.Contains(SelectionEntries[i].GameObject)) objects.Add(SelectionEntries[i].GameObject);
            }
            Selection.objects = objects.ToArray();
        }
        void Select()
        {
            List<GameObject> objects = new List<GameObject>();
            for (int i = 0; i < SelectionEntries.Count; i++)
            {
                if (SelectionEntries[i].Selected) objects.Add(SelectionEntries[i].GameObject);
            }
            Selection.objects = objects.ToArray();
        }
        #endregion

        #endregion
    }
}