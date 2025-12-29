using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ChangeObjectInformation : MonoBehaviour
{
    public static ChangeObjectInformation Instance;

    [Header("N Panel")]
    [SerializeField] private TextMeshProUGUI componentName;
    [SerializeField] private TMP_InputField inputField;

    private CircuitComponent curComp;

    private MouseRaycast mr;
    private bool isMoving = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mr = GetComponent<MouseRaycast>();

        inputField.onEndEdit.AddListener(OnEndEdit);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isMoving)
        {
            GameObject clickObject = mr.GetMouseClickObject("CircuitComponent");

            if (clickObject != null)
            {
                if (curComp)
                {
                    StopMoving();
                }

                CircuitComponent circComp = clickObject.GetComponent<CircuitComponent>();
                ColorChanger cc = clickObject.GetComponent<ColorChanger>();

                curComp = circComp;
                cc.ChangeMaterial(circComp.transform, "Purple");

                componentName.text = circComp.ComponentType.ToString();
                inputField.text = "";
            }
        }

        if (curComp)
        {
            if (Input.GetKeyDown(KeyCode.M) && !isMoving)
            {
                StartMoving();
            }

            if (isMoving && Input.GetKeyDown(KeyCode.Escape))
            {
                StopMoving();
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ChangeRotation(-90);
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ChangeRotation(90);
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                ObjectManager.Instance.RemoveObject(curComp.gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        if (ObjectManager.Instance.State == ColliderState.Small)
        {
            ObjectManager.Instance.State = ColliderState.Default;
        }
    }

    private void OnEndEdit(string newText)
    {
        if (!curComp) return;

        if (float.TryParse(newText, out float newValue))
        {
            curComp.componentCount = newValue;
            curComp.UpdateCode();
        }
    }

    private void ChangeRotation(float angle)
    {
        if (!curComp) return;

        curComp.transform.Rotate(0f, angle, 0f);
    }

    private void StartMoving()
    {
        curComp.AddComponent<MoveObject>();
        curComp.GetComponent<Collider>().isTrigger = true;

        isMoving = true;
    }

    public void StopMoving()
    {
        if (curComp == null) return;

        ColorChanger cc = curComp.GetComponent<ColorChanger>();

        cc.ChangeMaterial(cc.transform, "White");
        isMoving = false;

        MoveObject mo = curComp.GetComponent<MoveObject>();

        if(mo)
        {
            if (!mo.CanPlace())
            {
                mo.ReturnToStartPosition();
            }

            Destroy(mo);

            if(CanvasManager.Instance.NPanelIsOpen)
            {
                cc.ChangeMaterial(cc.transform, "Purple");
            }
        }

        curComp.GetComponent<Collider>().isTrigger = false;
    }
}
