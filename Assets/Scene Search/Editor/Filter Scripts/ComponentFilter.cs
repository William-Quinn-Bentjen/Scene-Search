using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SceneSearch.Filters
{
    [CreateAssetMenu(fileName = "Component Filter", menuName = "Scene Search Filters/Standard/Component")]
    public class ComponentFilter : SearchFilter
    {
        protected Utilities.FilterDelegate TestMethod;
        public Inclusivity Setting;
        [SerializeField, HideInInspector]
        public List<ComponentInfo> info = new List<ComponentInfo>();

        #region Definitions
        public enum Inclusivity
        {
            Matches, //exactly the same components
            IncludesAll, //has all items on the list but may have additional components
            DoesNotIncludeAny, //doesn't include any of the items on the list

            IncludesOne, //has one of the items on the list
            IncludesOneOrMore, //has one or more of the items on the list

            DoesNotIncludeOne, //doesn't include one of the items on the list
            DoesNotIncludeOneOrMore //doesn't include one or more of the items on the list
        }
        /// <summary>
        /// Used to hold component info 
        /// <para>MonoScripts will be stored based on GUID to allow for changes in the code</para>
        /// <para>Any other component types will be stored via their assembely qualified name</para>
        /// </summary>
        [System.Serializable]
        public struct ComponentInfo
        {
            public string assemblyQualifiedName;
            [SerializeField]
            public System.Type type
            {
                get
                {
                    return System.Type.GetType(assemblyQualifiedName);
                }
                set
                {
                    if (value != null) assemblyQualifiedName = value.AssemblyQualifiedName;
                    else assemblyQualifiedName = null;
                }
            }
            [SerializeField]
            public string GUID;
            [SerializeField]
            public bool isMonoBehaviour;
            #region Functions
            public override bool Equals(object obj)
            {
                if (obj is ComponentInfo)
                {
                    ComponentInfo info = (ComponentInfo)obj;
                    return info.type == type;
                }
                return false;
            }
            public override int GetHashCode()
            {
                if (type != null) return type.GetHashCode();
                else return 0;
            }
            public ComponentInfo(Component component) : this()
            {
                GetComponentInfo(component as MonoBehaviour);
                if (!isMonoBehaviour) type = component.GetType();
                assemblyQualifiedName = type == null ? null : type.AssemblyQualifiedName;
            }
            public void GetComponentInfo(MonoBehaviour monobehaviour)
            {
                if (monobehaviour != null)
                {
                    type = monobehaviour.GetType();
                    MonoScript script = MonoScript.FromMonoBehaviour(monobehaviour);
                    if (script != null)
                    {
                        GUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(script));
                        isMonoBehaviour = true;
                    }
                }
                else
                {
                    type = null;
                    GUID = null;
                    isMonoBehaviour = false;
                }
            }
            public void UpdateTypeFromGUID(string newGUID = null)
            {
                if (newGUID == null && GUID != null && GUID != "")
                {
                    string path = AssetDatabase.GUIDToAssetPath(GUID);
                    type = AssetDatabase.GetMainAssetTypeAtPath(path);
                    if (type == typeof(MonoScript))
                    {
                        MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                        type = monoScript.GetClass();
                    }
                }
                else if (newGUID != null)
                {
                    GUID = newGUID;
                    string path = AssetDatabase.GUIDToAssetPath(GUID);
                    type = AssetDatabase.GetMainAssetTypeAtPath(path);
                    if (type == typeof(MonoScript))
                    {
                        MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                        type = monoScript.GetClass();
                    }
                }
            }
            #endregion
        }
        #endregion

        #region Functions
        public override void Filter(List<GameObject> input)
        {
            switch (Setting)
            {
                case Inclusivity.Matches:
                    TestMethod = Matches;
                    break;
                case Inclusivity.IncludesAll:
                    TestMethod = IncludesAll;
                    break;
                case Inclusivity.IncludesOne:
                    TestMethod = IncludesOne;
                    break;
                case Inclusivity.IncludesOneOrMore:
                    TestMethod = IncludesOneOrMore;
                    break;
                case Inclusivity.DoesNotIncludeOneOrMore:
                    TestMethod = DoesNotIncludeOneOrMore;
                    break;
                case Inclusivity.DoesNotIncludeOne:
                    TestMethod = DoesNotIncludeOne;
                    break;
                case Inclusivity.DoesNotIncludeAny:
                    TestMethod = DoesNotIncludeAny;
                    break;
            }
            for (int i = 0; i < input.Count; i++)
            {
                if (!TestMethod(input[i]))
                {
                    input.RemoveAt(i);
                    i--;
                }
            }
        }
        public bool IncludesOne(GameObject gameObject)
        {
            return MatchesCount(GetComponentInfo(gameObject)) == 1;
        }
        public bool IncludesOneOrMore(GameObject gameObject)
        {
            return MatchesCount(GetComponentInfo(gameObject)) >= 1;
        }
        public bool IncludesAll(GameObject gameObject)
        {
            return MatchesCount(GetComponentInfo(gameObject)) == info.Count;
        }
        public bool DoesNotIncludeOne(GameObject gameObject)
        {
            return MatchesCount(GetComponentInfo(gameObject)) == info.Count - 1;
        }
        public bool DoesNotIncludeOneOrMore(GameObject gameObject)
        {
            return MatchesCount(GetComponentInfo(gameObject)) < info.Count;
        }
        public bool DoesNotIncludeAny(GameObject gameObject)
        {
            return MatchesCount(GetComponentInfo(gameObject)) == 0;
        }
        public bool Matches(GameObject gameObject)
        {
            List<ComponentInfo> objectComponentInfo = GetComponentInfo(gameObject);
            if (objectComponentInfo.Count != info.Count) return false;
            return MatchesCount(objectComponentInfo) == info.Count;
        }
        public List<ComponentInfo> GetComponentInfo(GameObject gameObject)
        {
            List<ComponentInfo> retVal = new List<ComponentInfo>();
            foreach (Component component in gameObject.GetComponents<Component>())
            {
                retVal.Add(new ComponentInfo(component));
            }
            return retVal;
        }
        public int MatchesCount(List<ComponentInfo> objectComponentInfo)
        {
            int matches = 0;
            foreach (ComponentInfo componentInfo in info)
            {
                if (objectComponentInfo.Contains(componentInfo))
                {
                    matches++;
                    objectComponentInfo.Remove(componentInfo);
                }
            }
            return matches;
        }
        #endregion
    }
}