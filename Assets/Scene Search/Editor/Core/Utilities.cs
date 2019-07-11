using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEditor;
/// <summary>
/// Enums Classes and delegate definitions that are used in some of the basic filters
/// </summary>
namespace SceneSearch.Filters.Utilities
{
    #region Enums
    public enum GlobalOrLocal
    {
        Global,
        Local
    }
    public enum ExactOrWithin
    {
        Exactly,
        Within
    }
    public enum IncludeOrExclude
    {
        Include,
        Exclude
    }
    #endregion
    #region Classes
    /// <summary>
    /// Utility for checking if a Vector3 is within a minimum and maximum value
    /// </summary>
    [System.Serializable]
    public class Vector3MinMax
    {
        [SerializeField]
        public Vector3 Min { get; private set; }
        [SerializeField]
        public Vector3 Max { get; private set; }

        #region Common public functions
        /// <summary>
        /// Test if a vector is within the min an max values
        /// </summary>
        /// <param name="vector">Vector to test against</param>
        /// <returns>If the vector is within the min and max values</returns>
        public bool AcceptableValue(Vector3 vector)
        {
            return (Min.x < vector.x || Mathf.Approximately(Min.x, vector.x) &&
                (Max.x > vector.x || Mathf.Approximately(Max.x, vector.x)) &&
                (Min.y < vector.y || Mathf.Approximately(Min.y, vector.y)) &&
                (Max.y > vector.y || Mathf.Approximately(Max.y, vector.y)) &&
                (Min.z < vector.z || Mathf.Approximately(Min.z, vector.z)) &&
                (Max.z > vector.z || Mathf.Approximately(Max.z, vector.z)));
        }
        /// <summary>
        /// Sets the min and max values based on 2 vectors
        /// </summary>
        /// <param name="firstInput">First vector</param>
        /// <param name="secondInput">Second vector</param>
        public void SetMinMax(Vector3 firstInput, Vector3 secondInput)
        {
            SetX(new Vector2(firstInput.x, secondInput.x));
            SetY(new Vector2(firstInput.y, secondInput.y));
            SetZ(new Vector2(firstInput.z, secondInput.z));
        }
        /// <summary>
        /// Used to make sure that the min and max values aren't conflicting
        /// </summary>
        public void Verifiy()
        {
            SetMinMax(Min, Max);
        }
        #endregion

        #region Set Min & Max
        public void SetX(Vector2 input)
        {
            MinMax(ref input);
            SetMinX(input.x);
            SetMaxX(input.y);
        }
        public void SetY(Vector2 input)
        {
            MinMax(ref input);
            SetMinY(input.x);
            SetMaxY(input.y);
        }
        public void SetZ(Vector2 input)
        {
            MinMax(ref input);
            SetMinZ(input.x);
            SetMaxZ(input.y);
        }
        #endregion

        #region Set Min or Max
        public void SetMax(Vector3 input)
        {
            Max = input;
        }
        public void SetMin(Vector3 input)
        {
            Min = input;
        }
        public void SetMinX(float value)
        {
            Min = new Vector3(value, Min.y, Min.z);
        }
        public void SetMaxX(float value)
        {
            Max = new Vector3(value, Max.y, Max.z);
        }
        public void SetMinY(float value)
        {
            Min = new Vector3(Min.x, value, Min.z);
        }
        public void SetMaxY(float value)
        {
            Max = new Vector3(Max.x, value, Max.z);
        }
        public void SetMinZ(float value)
        {
            Min = new Vector3(Min.x, Min.y, value);
        }
        public void SetMaxZ(float value)
        {
            Max = new Vector3(Max.x, Max.y, value);
        }
        #endregion
        
        /// <summary>
        /// Sets smaller to X and larger to Y
        /// </summary>
        /// <param name="input">A vector containing values to be ordered so X is the smaller and Y is the larger of the two</param>
        void MinMax(ref Vector2 input)
        {
            if (input.x > input.y)
            {
                float buffer = input.y;
                input.y = input.x;
                input.x = buffer;
            }
        }
    }
    /// <summary>
    /// A class that holds extention methods for other classes
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Copy a component to this gameobject
        /// </summary>
        /// <typeparam name="T">The type of component</typeparam>
        /// <param name="destination">The gameobject that will receive the new component</param>
        /// <param name="original">The original component to be coppied</param>
        /// <returns>A new copy of the original component</returns>
        public static T CopyComponent<T>(this GameObject destination, T original) where T : UnityEngine.Component
        {
            System.Type type = original.GetType();
            UnityEngine.Component copy = destination.AddComponent(type);
            FieldInfo[] fields = type.GetFields();
            foreach (FieldInfo field in fields)
            {
                field.SetValue(copy, field.GetValue(original));
            }
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default /*| BindingFlags.DeclaredOnly*/;
            PropertyInfo[] pinfos = type.GetProperties(flags);
            foreach (var pinfo in pinfos)
            {
                if (pinfo.CanWrite)
                {
                    try
                    {
                        if (!pinfo.IsDefined(typeof(System.ObsoleteAttribute), true))
                            pinfo.SetValue(copy, pinfo.GetValue(original, null), null);
                    }
                    catch { }
                }
            }
            FieldInfo[] finfos = type.GetFields(flags);
            foreach (var finfo in finfos)
            {
                finfo.SetValue(copy, finfo.GetValue(original));
            }
            return copy as T;
        }
    }
    #endregion
    /// <summary>
    /// Used to avoid large and repetitive code
    /// <para>add a method based on the filter's settings</para>
    /// <para>then call the delegate when looping through input</para>
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public delegate bool FilterDelegate(GameObject gameObject);
}
