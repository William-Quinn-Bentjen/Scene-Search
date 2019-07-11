using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SceneSearch.Filters.Utilities;
namespace SceneSearch
{
    namespace Filters
    {
        [CustomEditor(typeof(Scale))]
        public class ScaleEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                serializedObject.Update();
                Scale ScaleFilter = (serializedObject.targetObject as Scale);
                using (var vertScope = new EditorGUILayout.VerticalScope("Box"))
                {
                    if (ScaleFilter.exactOrWithin == ExactOrWithin.Within)
                    {
                        ScaleFilter.MinMax.SetMin(EditorGUILayout.Vector3Field("Min", ScaleFilter.MinMax.Min));
                        ScaleFilter.MinMax.SetMax(EditorGUILayout.Vector3Field("Max", ScaleFilter.MinMax.Max));
                        if (GUILayout.Button("Fix min and max Values")) ScaleFilter.MinMax.Verifiy();
                    }
                    else
                    {
                        ScaleFilter.MinMax.SetMax(ScaleFilter.MinMax.Min); //set max to min so values are the same
                        Vector3 vector = EditorGUILayout.Vector3Field("Exactly", ScaleFilter.MinMax.Min);
                        ScaleFilter.MinMax.SetMinMax(vector, vector); // update min max settings
                    }
                }
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}