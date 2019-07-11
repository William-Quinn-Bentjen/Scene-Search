using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using SceneSearch.Filters;
namespace SceneSearch
{
    /// <summary>
    /// The window used to search 
    /// </summary>
    public class SearchWindow : EditorWindow
    {
        Editor filterEditor;
        SearchFilter filter;
        Vector2 scrollPos = new Vector2();
        static bool dockOutputWindow;

        #region Window Functions
        [MenuItem("Window/Tools/Scene Search %&s")]
        public static void ShowWindow()
        {
            GetWindow(typeof(SearchWindow), false, "Scene Search", true);
        }
        void OnGUI()
        {
            if (GUILayout.Button("Search"))
            {
                if (filter != null)
                {
                    List<GameObject> results = GetAllGameObjects();
                    filter.Filter(results);
                    ResultsWindow.DisplayResult(results, filter, dockOutputWindow);
                }
            }
#if UNITY_2019_OR_NEWER
            dockOutputWindow = EditorGUILayout.ToggleLeft("Dock results window", dockOutputWindow);
#else
            GUI.enabled = false;
            dockOutputWindow = EditorGUILayout.ToggleLeft("Dock results window (available in 2019 or newer)", dockOutputWindow);
            GUI.enabled = true;
#endif
            using (var scope = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Filter");
                filter = (SearchFilter)EditorGUILayout.ObjectField(filter, typeof(SearchFilter), true);
                if (filter != null)
                {
                    SearchFilterGroup filterGroup = (filter as SearchFilterGroup);
                    if (filterGroup == null)
                    {
                        filterGroup = CreateInstance<SearchFilterGroup>();
                        filterGroup.AddSearchFilter(filter);
                        filter = filterGroup;
                    }
                }
                else
                {
                    filter = CreateInstance<SearchFilterGroup>();
                }
            }
            if (filter != null)
            {
                using (var scrollScope = new GUILayout.ScrollViewScope(scrollPos))
                {
                    scrollPos = scrollScope.scrollPosition;
                    filterEditor = Editor.CreateEditor(filter);
                    filterEditor.OnInspectorGUI();
                }
            }
        }
        #endregion

        #region Utility Functions (help find all gameobejcts in the scene)
        /// <summary>
        /// Get all the GameObjects in the loaded scenes
        /// <para>NOTE: includes inactive objects</para>
        /// </summary>
        /// <returns>All GameObjects in the loaded scenes</returns>
        private List<GameObject> GetAllGameObjects()
        {
            List<GameObject> allGameObjects = new List<GameObject>();
            foreach (GameObject obj in GetRootObjects())
            {
                //add root object
                allGameObjects.Add(obj);
                //add all children of that object
                ExploreRootObject(obj.transform, allGameObjects);
            }
            return allGameObjects;
        }
        /// <summary>
        /// Adds all children of a root GameObject to the list of all children
        /// </summary>
        /// <param name="rootTransform">The Transform of the GameObject at the root of the scene (has no parent)</param>
        /// <param name="allChildren">A reference to the list of all children in the scene</param>
        private void ExploreRootObject(Transform rootTransform, List<GameObject> allChildren)
        {
            List<Transform> unexplored = new List<Transform>{ rootTransform };
            while (unexplored.Count > 0)
            {
                unexplored.AddRange(Explore(unexplored[0], allChildren));
                unexplored.RemoveAt(0);
            }
        }
        /// <summary>
        /// Adds children of parent object to the all children list and returns the children of each child
        /// </summary>
        /// <param name="parent">The parent that will have it's children explored</param>
        /// <param name="allChildren">A reference to the list of all children in the scene</param>
        /// <returns></returns>
        private List<Transform> Explore(Transform parent, List<GameObject> allChildren)
        {
            List<Transform> children = new List<Transform>();
            foreach (Transform child in parent)
            {
                children.Add(child);
                allChildren.Add(child.gameObject);
            }
            return children;
        }
        /// <summary>
        /// Gets the root objects in the loaded scenes
        /// </summary>
        /// <returns>A list of all objects in the loaded scenes that have no parent</returns>
        private List<GameObject> GetRootObjects()
        {
            List<GameObject> rootObjects = new List<GameObject>();
            Scene[] LoadedScenes = GetScenes();
            for(int i =0; i < LoadedScenes.Length; i++)
            {
                rootObjects.AddRange(LoadedScenes[i].GetRootGameObjects());
            }
            return rootObjects;
        }
        /// <summary>
        /// Gets all loaded scenes from the scene manager
        /// </summary>
        /// <returns>all loaded scenes from the scene manager</returns>
        private Scene[] GetScenes()
        {
            int countLoaded = SceneManager.sceneCount;
            Scene[] loadedScenes = new Scene[countLoaded];
            for (int i = 0; i < countLoaded; i++)
            {
                loadedScenes[i] = SceneManager.GetSceneAt(i);
            }
            return loadedScenes;
        }
        #endregion
    }
}