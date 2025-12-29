using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MoveObject : MonoBehaviour
{
    private List<Collider> connectedColliders = new();
    private Vector3 StartPosition;

    private ColorChanger cc;

    private void Start ()
    {
        StartPosition = transform.position;

        cc = GetComponent<ColorChanger>();
    }

    private void Update ()
    {
        Vector3 newPos = MouseRaycast.Instance.GetMouseClickTransform().position;
        this.transform.position = new Vector3(newPos.x, 0f, newPos.z);
    }

    private void OnTriggerEnter(Collider col) 
    {
        if(col.CompareTag("CircuitComponent"))
        {
            connectedColliders.Add(col);
            cc.ChangeMaterial(this.transform, "Red");
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if(col.CompareTag("CircuitComponent"))
        {
            connectedColliders.Remove(col);

            if(CanPlace())
            {
                cc.ChangeMaterial(this.transform, "Green");
            }
        }
    }

    public bool CanPlace ()
    {
        return connectedColliders.Count == 0 ? true : false;
    }

    public void ReturnToStartPosition()
    {
        this.transform.position = StartPosition;
    }
}
