using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance;

    #region Col Changes

    [Header("Collider List")]
    [SerializeField] private List<BoxCollider> allObjectsColliders = new();
    
    private List<Vector3> allObjectsColliderDefaultSize = new();

    private ColliderState state = ColliderState.Default;

    public ColliderState State
    {
        get => state;

        set
        {
            if(state == value) return;
            
            if(value == ColliderState.Small)
            {
                SetAllCollidersSize("Small");
            }
            else if(value == ColliderState.Default)
            {
                SetAllCollidersSize("Default");
            }

            state = value;
        }
    }

    private void AddObjectCollider (BoxCollider col)
    {
        allObjectsColliders.Add(col);
        allObjectsColliderDefaultSize.Add(col.size);

        if(state == ColliderState.Small) SetAllCollidersSize(col);
    }

    private void RemoveObjectCollider (BoxCollider col)
    {
        int index = allObjectsColliders.IndexOf(col);
        
        allObjectsColliders.Remove(allObjectsColliders[index]);
        allObjectsColliderDefaultSize.Remove(allObjectsColliderDefaultSize[index]);
    }

    public void SetAllCollidersSize (string type)
    {
        switch(type)
        {
            case "Small":
                foreach (var i in allObjectsColliders)
                {
                    SetAllCollidersSize(i);
                }
                break;
            case "Default":
                for (int i = 0; i < allObjectsColliders.Count; i++)
                {
                    allObjectsColliders[i].size = allObjectsColliderDefaultSize[i];
                }
                break;
        }
    }

    public void SetAllCollidersSize (BoxCollider col) // Вызывается, если объекты в маленьком режиме
    {
        col.size = new Vector3(0.1f, 0.1f, 0.1f);
    }

    #endregion

    #region Base

    [Header("Object List")]
    [SerializeField] private List<GameObject> allObjectList = new();

    public void AddObject (GameObject obj)
    {
        BoxCollider boxCol = obj.GetComponent<BoxCollider>();

        AddObjectCollider(boxCol);
        allObjectList.Add(obj);
    }

    public void RemoveObject (GameObject obj)
    {
        BoxCollider boxCol = obj.GetComponent<BoxCollider>();

        RemoveObjectCollider(boxCol);
        allObjectList.Remove(obj);
        
        Destroy(obj);
    }

    private void Awake ()
    {
        Instance = this;
    }

    private void Start ()
    {
        State = ColliderState.Small;
    }

    #endregion
}

public enum ColliderState
{
    Small,
    Default
}
