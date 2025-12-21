using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DemoObject : MonoBehaviour
{
    private List<GameObject> colliderObjects = new();

    private void OnTriggerEnter(Collider col) 
    {
        if(!col.CompareTag("CircuitComponent")) return;

        PlaceNewObjects.Instance.CanPlace = false;

        colliderObjects.Add(col.gameObject);
    }

    private void OnTriggerExit (Collider col)
    {
        if(!col.CompareTag("CircuitComponent")) return;

        colliderObjects.Remove(col.gameObject);

        if(colliderObjects.Count == 0) PlaceNewObjects.Instance.CanPlace = true;
    }
}
