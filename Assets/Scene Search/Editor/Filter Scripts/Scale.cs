using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SceneSearch.Filters.Utilities;
namespace SceneSearch
{
    namespace Filters
    {
        [CreateAssetMenu(fileName = "Scale Filter", menuName = "Scene Search Filters/Standard/Scale")]
        public class Scale : SearchFilter
        {
            public IncludeOrExclude inclusivity;
            public GlobalOrLocal globalOrLocal;
            public ExactOrWithin exactOrWithin;
            [HideInInspector]
            public Vector3MinMax MinMax;
            protected FilterDelegate TestMethod;

            #region Functions
            public override void Filter(List<GameObject> input)
            {
                bool include = inclusivity == IncludeOrExclude.Include;
                if (globalOrLocal == GlobalOrLocal.Global) TestMethod = Global;
                else TestMethod = Local;
                MinMax.Verifiy();
                for (int i = 0; i < input.Count; i++)
                {
                    if (TestMethod(input[i]))
                    {
                        if (!include)
                        {
                            input.RemoveAt(i);
                            i--;
                        }
                    }
                    else if (include)
                    {
                        input.RemoveAt(i);
                        i--;
                    }
                }
                base.Filter(input);
            }
            public override SearchFilter DeepCopy()
            {
                Scale returnValue = base.DeepCopy() as Scale;
                returnValue.MinMax = new Vector3MinMax();
                returnValue.MinMax.SetMinMax(MinMax.Min, MinMax.Max);
                return returnValue;
            }
            protected bool Global(GameObject gameObject)
            {
                return MinMax.AcceptableValue(gameObject.transform.lossyScale);
            }
            protected bool Local(GameObject gameObject)
            {
                return MinMax.AcceptableValue(gameObject.transform.localScale);
            }
            #endregion
        }
    }
}
