using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SceneSearch
{
    namespace Filters
    {
        [CreateAssetMenu(fileName = "Tag Filter", menuName = "Scene Search Filters/Standard/Tag")]
        public class Tag : SearchFilter
        {
            public Utilities.IncludeOrExclude inclusivity;
            [HideInInspector]
            public string tag = "Untagged";
            public override void Filter(List<GameObject> input)
            {
                bool include = inclusivity == Utilities.IncludeOrExclude.Include;
                if (tag != null)
                {
                    GameObject gameObject;
                    for (int i = 0; i < input.Count;)
                    {
                        gameObject = input[i];
                        if (gameObject == null)
                        {
                            input.RemoveAt(i);
                        }
                        else
                        {
                            bool tagCheck = gameObject.tag == tag;
                            // if part of layer is the same
                            if (include && tagCheck)
                            {
                                i++;
                            }
                            // if not part of layer bool and layers not the same
                            else if (!include && !tagCheck)
                            {
                                i++;
                            }
                            else
                            {
                                input.RemoveAt(i);
                            }
                        }
                    }
                }
            }
        }
    }
}