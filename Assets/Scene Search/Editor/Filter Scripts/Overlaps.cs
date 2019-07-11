using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SceneSearch.Filters.Utilities;
namespace SceneSearch
{
    namespace Filters
    {
        [CreateAssetMenu(fileName = "Overlaps Filter", menuName = "Scene Search Filters/Standard/Overlaps")]
        public class Overlaps : SearchFilter
        {
            public IncludeOrExclude inclusivity;
            public bool CheckForTransformsWithoutColliders = true;
            public bool ExcludeCastingCollider = false;
            public QueryTriggerInteraction triggerInteraction;
            [HideInInspector]
            public Collider Collider;
            Collider instantiatedCollider;

            #region Functions
            public override void Filter(List<GameObject> input)
            {
                if (Collider != null)
                {
                    bool include = inclusivity == IncludeOrExclude.Include;
                    // instantiate a copy of the collider on a new gameobject to make ensure the collider we check raycasts with is active and enabled without having to change the scene
                    instantiatedCollider = new GameObject().CopyComponent(Collider);
                    instantiatedCollider.transform.position = Collider.transform.position;
                    instantiatedCollider.transform.rotation = Collider.transform.rotation;
                    instantiatedCollider.transform.localScale = Collider.transform.lossyScale; // this is fine because the new gameobject is not parented
                    // Create a list of all overlapping gameobjects
                    List<GameObject> overlapping = new List<GameObject>();
                    // Check for objects in the collider's bounding box
                    Vector3 ColliderPosition = instantiatedCollider.transform.position;
                    Quaternion ColliderRotation = instantiatedCollider.transform.rotation;
                    foreach (Collider col in Physics.OverlapBox(instantiatedCollider.bounds.center, instantiatedCollider.bounds.extents))
                    {
                        // check for collider overlap with object
                        Vector3 otherPosition = col.gameObject.transform.position;
                        Quaternion otherRotation = col.gameObject.transform.rotation;

                        Vector3 direction;
                        float distance;

                        if (Physics.ComputePenetration(
                            instantiatedCollider, ColliderPosition, ColliderRotation,
                            col, otherPosition, otherRotation,
                            out direction, out distance
                        ) && input.Contains(col.gameObject)) overlapping.Add(col.gameObject); // if colliders overlapping add to list of overlapping objects

                    }
                    // Transforms without collider detection
                    if (CheckForTransformsWithoutColliders)
                    {
                        for (int i = 0; i < input.Count; i++)
                        {
                            // check if each item is not on the list of colliding objects but in the bounding box of the collider
                            if (!overlapping.Contains(input[i]) && instantiatedCollider.bounds.Contains(input[i].transform.position))
                            {
                                // if inside the collider add to the list
                                if (PointInside(input[i].transform.position)) overlapping.Add(input[i]);
                            }
                        }
                    }
                    // Change input list to reflect overlapping colliders
                    if (include)
                    {
                        // leave only the overlapping colliders who were included in the input list
                        for (int i = 0; i < overlapping.Count; i++)
                        {
                            if (!input.Contains(overlapping[i]))
                            {
                                overlapping.RemoveAt(i);
                                i--;
                            }
                        }
                        input.Clear();
                        input.AddRange(overlapping);
                    }
                    else
                    {
                        // remove any overlapping colliders
                        foreach (GameObject obj in overlapping)
                        {
                            input.Remove(obj);
                        }
                    }
                    // Remove the copy if it somehow got included because it wasn't part of the search to begin with
                    input.Remove(instantiatedCollider.gameObject);
                    DestroyImmediate(instantiatedCollider.gameObject);
                    if (ExcludeCastingCollider) input.Remove(Collider.gameObject); // Exlcude the collider that was used to search 
                }
            }
            bool PointInside(Vector3 point)
            {
                int hitCount = 0;
                Vector3 outside = instantiatedCollider.transform.position + instantiatedCollider.bounds.extents * 1.25f;
                Vector3 path = point - outside;
                Ray rayIn = new Ray(outside, path);
                Ray rayOut = new Ray(point, -path);
                foreach (RaycastHit hit in Physics.RaycastAll(rayIn, path.magnitude, ~0, triggerInteraction))
                {
                    if (hit.collider == instantiatedCollider) hitCount++;
                }
                foreach (RaycastHit hit in Physics.RaycastAll(rayOut, path.magnitude, ~0, triggerInteraction))
                {
                    if (hit.collider == instantiatedCollider) hitCount++;
                }
                if (hitCount % 2 == 1)
                {
                    return true;
                }
                return false;
            }
            #endregion
        }
    }
}