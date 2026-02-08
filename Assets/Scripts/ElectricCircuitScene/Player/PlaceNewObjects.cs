using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MouseRaycast))]
public class PlaceNewObjects : MonoBehaviour
{
    public static PlaceNewObjects Instance;

    [Header("Create Component List")]
    [SerializeField] private List<GameObject> demoObjectPrefabs;
    [SerializeField] private List<GameObject> objectPrefabs;

    private bool canPlace = true;
    public bool CanPlace
    {
        get => canPlace;
        set => canPlace = value;
    }

    private MouseRaycast mouseRay;

    private Transform newObjectTransform;
    private bool isDrawing = false;
    private bool isGreen = true;
    private int curIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mouseRay = GetComponent<MouseRaycast>();
    }

    private void Update()
    {
        if (isDrawing && newObjectTransform)
        {
            if (mouseRay.GetMouseClickObject("BuildPlane") != null && canPlace)
            {
                if (!isGreen)
                {
                    ChangeMaterial(newObjectTransform, "Green");
                    isGreen = true;
                }
            }
            else
            {
                if (isGreen)
                {
                    ChangeMaterial(newObjectTransform, "Red");
                    isGreen = false;
                }
            }

            if(mouseRay.GetMouseClickTransform() != null)
            {
                Vector3 newPos = mouseRay.GetMouseClickTransform().position;
                newObjectTransform.position = new Vector3(newPos.x, 0f, newPos.z);
            }
        }

        if (Input.GetMouseButtonDown(0) && isDrawing && isGreen && canPlace)
        {
            GameObject newObj = Instantiate(objectPrefabs[curIndex], mouseRay.GetMouseClickTransform().position, Quaternion.identity);

            ObjectManager.Instance.AddObject(newObj);
            
            newObj.SetActive(true);
            
            StopDrawing();
        }
    }

    public void ButtonIsPressed(int index)
    {
        if(isDrawing) StopDrawing();

        ObjectManager.Instance.State = ColliderState.Default;

        isDrawing = true;
        curIndex = index;

        GameObject newObject = Instantiate(demoObjectPrefabs[index], transform.position, Quaternion.identity);

        newObjectTransform = newObject.transform;
    }


    /// <summary>
    /// Меняет цвет объекта, путём замены всех материалов в его детях.
    /// </summary>
    /// <param name="changeObject">Transform изменяемого объекта.</param>
    /// <param name="type">Цвет: "Green" - заленый, "Red" - красный.</param>
    private void ChangeMaterial(Transform changeObject, string type)
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

    private void StopDrawing()
    {
        ObjectManager.Instance.State = ColliderState.Small;

        Destroy(newObjectTransform.gameObject);

        isDrawing = false;
        newObjectTransform = null;
    }
}
