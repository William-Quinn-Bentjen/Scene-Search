using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace SceneSearch
{
    namespace Filters
    {
        [CustomEditor(typeof(Tag))]
        public class TagEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                serializedObject.Update();
                Tag tagObject = (serializedObject.targetObject as Tag);
                tagObject.tag = EditorGUILayout.TagField("", tagObject.tag);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}