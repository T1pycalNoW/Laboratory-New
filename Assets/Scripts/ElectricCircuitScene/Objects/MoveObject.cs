using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MoveObject : MonoBehaviour
{
    private List<Collider> connectedColliders = new();
    private Vector3 StartPosition;

    private void Start ()
    {
        StartPosition = transform.position;
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
            ChangeMaterial(this.transform, "Red");
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if(col.CompareTag("CircuitComponent"))
        {
            connectedColliders.Remove(col);

            if(CanPlace())
            {
                ChangeMaterial(this.transform, "Green");
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

    /// <summary>
    /// Меняет цвет объекта, путём замены всех материалов в его детях.
    /// </summary>
    /// <param name="changeObject">Transform изменяемого объекта.</param>
    /// <param name="type">Цвет: "Green" - заленый, "Red" - красный, "White" - белый, "Default" - серый.</param>
    public void ChangeMaterial(Transform changeObject, string type)
    {
        Debug.Log($"Changing... Change on {type}");
        Color newColor = Color.gray;

        switch (type)
        {
            case "Green":
                newColor = Color.green;
                break;
            case "Red":
                newColor = Color.red;
                break;
            case "White":
                newColor = Color.white;
                break;
        }

        for (int i = 0; i < changeObject.childCount; i++)
        {
            if (changeObject.GetChild(i).TryGetComponent(out MeshRenderer mc))
            {
                //mc.material = changeMaterial;
                mc.material.color = newColor;
            }
        }

        for (int i = 0; i < changeObject.childCount; i++)
        {
            if (changeObject.GetChild(0).GetChild(i).TryGetComponent(out MeshRenderer mc))
            {
                //mc.material = changeMaterial;
                mc.material.color = newColor;
            }
        }
    }
}
