using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float flySpeed = 10f;

    [Header("Movement Settings")]
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 10f;
    [SerializeField] private float scrollSpeed = 1f;

    private Camera cam;

    private void Start ()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        UpdateCameraMovement();
        UpdateScroller();
    }

    private void UpdateCameraMovement ()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float curSpeed = Input.GetKey(KeyCode.LeftShift) ? flySpeed : moveSpeed;

        Vector3 movement = new Vector3(horizontal, 0f, vertical);

        transform.Translate(movement * curSpeed * Time.deltaTime, Space.World);
    }

    private void UpdateScroller()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if(scroll != 0)
        {
            Vector3 pos = cam.transform.position;
            pos.y = Mathf.Clamp(pos.y - scroll * scrollSpeed, minZoom, maxZoom);

            cam.transform.position = pos;
        }
    }
}
