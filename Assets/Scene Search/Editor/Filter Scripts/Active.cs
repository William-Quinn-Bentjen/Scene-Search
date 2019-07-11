using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SceneSearch
{
    namespace Filters
    {
        [CreateAssetMenu(fileName = "Active Filter", menuName = "Scene Search Filters/Standard/Active")]
        public class Active : SearchFilter
        {
            public enum ActiveSetting
            {
                InHierarchy,
                Self
            }
            public Utilities.IncludeOrExclude inclusivity;
            public bool active = true;
            public ActiveSetting activeIn;
            protected Utilities.FilterDelegate TestMethod;

            #region Functions
            public override void Filter(List<GameObject> input)
            {
                bool include = inclusivity == Utilities.IncludeOrExclude.Include;
                // Active In Hierarchy
                if (activeIn == ActiveSetting.InHierarchy) TestMethod = ActiveInHierarchy;
                else TestMethod = ActiveSelf;
                for (int i = 0; i < input.Count;)
                {
                    if (TestMethod(input[i]) == active)
                    {
                        if (include) i++;
                        else input.RemoveAt(i);
                    }
                    else
                    {
                        if (include) input.RemoveAt(i);
                        else i++;
                    }
                }
            }
            protected bool ActiveInHierarchy(GameObject gameObject)
            {
                return gameObject.activeInHierarchy;
            }
            protected bool ActiveSelf(GameObject gameObject)
            {
                return gameObject.activeSelf;
            }
            #endregion
        }
    }
}