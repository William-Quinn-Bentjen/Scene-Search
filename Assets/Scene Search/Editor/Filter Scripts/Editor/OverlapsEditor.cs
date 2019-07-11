using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace SceneSearch
{
    namespace Filters
    {
        [CustomEditor(typeof(Overlaps))]
        public class OverlapsEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                Overlaps overlap = (serializedObject.targetObject as Overlaps);
                base.OnInspectorGUI();
                overlap.Collider = EditorGUILayout.ObjectField("Collider", overlap.Collider, typeof(Collider), true) as Collider;
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
