using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SceneSearch
{
    namespace Filters
    {
        [CreateAssetMenu(fileName = "Layer Filter", menuName = "Scene Search Filters/Standard/Layer")]
        public class Layer : SearchFilter
        {
            public Utilities.IncludeOrExclude inclusivity;
            public UnityEngine.LayerMask layerMask;

            #region Functions
            public override void Filter(List<GameObject> input)
            {
                GameObject gameObject;
                bool include = inclusivity == Utilities.IncludeOrExclude.Include;
                for (int i = 0; i < input.Count;)
                {
                    gameObject = input[i];
                    if (gameObject == null)
                    {
                        input.RemoveAt(i);
                    }
                    else
                    {
                        bool maskCheck = LayerMaskCheck(gameObject.layer);
                        // if part of layer is the same
                        if (include && maskCheck)
                        {
                            i++;
                        }
                        // if not part of layer bool and layers not the same
                        else if (!include && !maskCheck)
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
            bool LayerMaskCheck(int layer)
            {
                return layerMask == (layerMask | (1 << layer));
            }
            #endregion
        }
    }
}