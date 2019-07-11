using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SceneSearch
{
    namespace Filters
    {
        /// <summary>
        /// A base class for a scene search filter, inherit from this to add your own filters
        /// </summary>
        [System.Serializable]
        public class SearchFilter : ScriptableObject
        {
            /// <summary>
            /// Function to filter a list of gameobjects
            /// </summary>
            /// <param name="input">Gameobjects that need to be filtered</param>
            public virtual void Filter(List<GameObject> input) { }
            /// <summary>
            /// Used to copy the filter's settings for the output window 
            /// <para>This only needs to be overridden if the default Instantiate doesn't work properly</para>
            /// <para>(like it copying a reference instead of copying the object being referenced)</para>
            /// </summary>
            /// <returns>A copy of the search filter</returns>
            public virtual SearchFilter DeepCopy() { return Instantiate(this); }
        }
    }
}