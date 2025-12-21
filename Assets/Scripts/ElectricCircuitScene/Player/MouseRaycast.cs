using UnityEngine;

public class MouseRaycast : MonoBehaviour
{
    public static MouseRaycast Instance;

    [Header("Empty Object")]
    [SerializeField] private GameObject EmptyObject;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            MouseInteract();
        }
    }

    private void MouseInteract() // Вызывает метод для поиска объектов с тэгом Interactable
    {
        GameObject interactObj = GetMouseClickObject("Interactable");

        if (interactObj == null)
        {
            Debug.Log("Ошибка! Объект не найден");
            return;
        }

        interactObj.GetComponent<IInteractable>().Interact();
    }

    public Transform GetMouseClickTransform() // Кастует луч и возвращает объект
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            EmptyObject.transform.position = hit.point;
            return EmptyObject.transform;
        }

        return null;
    }

    public GameObject GetMouseClickObject(string tag) // Кастует луч и возвращает объект, если совпадает тэг
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log(hit.collider.gameObject);

            if (hit.collider.CompareTag(tag))
            {
                return hit.collider.gameObject;
            }
        }

        return null;
    }
}
