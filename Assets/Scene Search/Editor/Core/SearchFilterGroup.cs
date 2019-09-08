using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SceneSearch
{
    namespace Filters
    {
        /// <summary>
        /// A filter that contains references other filters to allow adding or removing multiple filters at a time
        /// </summary>
        [CreateAssetMenu(fileName = "Search Filter Group", menuName = "Scene Search Filters/Search Group"), System.Serializable]
        public class SearchFilterGroup : SearchFilter
        {
            [SerializeField, HideInInspector]
            public List<SearchFilter> searchFilters = new List<SearchFilter>();

            #region Filter Functions (Filter and DeepCopy)
            /// <summary>
            /// Filter wiht all the filters contained in the group
            /// </summary>
            /// <param name="input">The list of gameobjects to filter</param>
            public override void Filter(List<GameObject> input)
            {
                for (int i = 0; i < searchFilters.Count; i++)
                {
                    if (searchFilters[i] != null) searchFilters[i].Filter(input);
                }
            }
            /// <summary>
            /// Get a deep copy of the group and all filters contained in the group
            /// </summary>
            /// <returns>a copy of this group with all filters deep copied</returns>
            public override SearchFilter DeepCopy()
            {
                SearchFilterGroup newGroup = base.DeepCopy() as SearchFilterGroup;
                newGroup.searchFilters.Clear();
                for(int i = 0; i < searchFilters.Count; i++)
                {
                    if (searchFilters[i] != null) newGroup.searchFilters.Add(searchFilters[i].DeepCopy());
                }
                return newGroup;
            }
            #endregion

            #region Utility Functions (helps to add and removes filters safely)
            /// <summary>
            /// To add a filter or filter group
            /// </summary>
            /// <param name="searchFilter">Filter or filter group to add</param>
            public void AddSearchFilter(SearchFilter searchFilter)
            {
                SearchFilterGroup filterGroup = searchFilter as SearchFilterGroup;
                if (filterGroup != null)
                {
                    if (searchFilter == this)
                    {
                        Debug.LogWarning("Can not add filter group to itself");
                    }
                    else if (Contains(filterGroup) || filterGroup.Contains(this))
                    {
                        Debug.LogWarning("Filter already in group or subgroup");
                    }
                    else searchFilters.Add(filterGroup);
                }
                else if (!Contains(searchFilter))
                {
                    searchFilters.Add(searchFilter);
                }
            }
            /// <summary>
            /// Removes a filter or filter group from the group
            /// </summary>
            /// <param name="searchFilter">Filter or filter group to remove</param>
            public bool Remove(SearchFilter searchFilter)
            {
                SearchFilterGroup testGroup = searchFilter as SearchFilterGroup;
                if (testGroup != null) { return Remove(testGroup); }
                else
                {
                    if (searchFilter == null) return false;
                    if (searchFilters.Remove(searchFilter)) return true;
                    for (int i = 0; i < searchFilters.Count; i++)
                    {
                        SearchFilterGroup testSubGroup = searchFilters[i] as SearchFilterGroup;
                        if (testSubGroup != null && testSubGroup.Remove(searchFilter)) return true;
                    }
                    return false;
                }
            }
            public bool Remove(SearchFilterGroup group)
            {
                if (group == this || group == null) return false;
                if (searchFilters.Remove(group)) return true;
                for (int i = 0; i < searchFilters.Count; i++)
                {
                    SearchFilterGroup testGroup = searchFilters[i] as SearchFilterGroup;
                    if (testGroup != null && testGroup.Remove(group)) return true;
                }
                return false;
            }
            /// <summary>
            /// Checks if this group already contains a group
            /// </summary>
            /// <param name="group">Group to check</param>
            /// <returns>If the group is already part of this group</returns>
            public bool Contains(SearchFilterGroup group)
            {
                if (group == this) return true;
                for (int i = 0; i < searchFilters.Count; i++)
                {
                    SearchFilterGroup testGroup = searchFilters[i] as SearchFilterGroup;
                    if (testGroup != null && (testGroup == group || testGroup.Contains(group))) return true;
                }
                return false;
            }
            /// <summary>
            /// Checks if a filter is already in this filter group
            /// </summary>
            /// <param name="searchFilter">Filter to check</param>
            /// <returns>If the filter is already part of this group</returns>
            public bool Contains(SearchFilter searchFilter)
            {
                SearchFilterGroup searchGroupTest = (searchFilter as SearchFilterGroup);
                if (searchGroupTest != null) return Contains(searchGroupTest);
                if (searchFilters.Contains(searchFilter)) { return true; }
                else
                {
                    SearchFilterGroup groupTest;
                    for (int i = 0; i < searchFilters.Count; i++)
                    {
                        groupTest = searchFilters[i] as SearchFilterGroup;
                        if (groupTest != null)
                        {
                            if (groupTest.Contains(searchFilter)) return true;
                        }
                    }
                }
                return false;
            }
            #endregion
        }
    }
}
