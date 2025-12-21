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

    private void Awake ()
    {
        Instance = this;
    }

    private void Start()
    {
        mr = GetComponent<MouseRaycast>();

        inputField.onValueChanged.AddListener(OnValueChanged);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject clickObject = mr.GetMouseClickObject("CircuitComponent");

            if (clickObject != null)
            {
                CircuitComponent circComp = clickObject.GetComponent<CircuitComponent>();
                curComp = circComp;

                componentName.text = circComp.ComponentType.ToString();
            }
        }

        if(curComp)
        {
            if(Input.GetMouseButtonDown(0))
            {
                isMoving = !isMoving;
            }

            if(isMoving)
            {
                
            }
        
            if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                ChangeRotation(-90);
            }

            if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ChangeRotation(90);
            }

            if(Input.GetKeyDown(KeyCode.Delete))
            {
                ObjectManager.Instance.RemoveObject(curComp.gameObject);
            }
        }
    }

    private void FixedUpdate() 
    {
        if(ObjectManager.Instance.State == ColliderState.Small)
        {
            ObjectManager.Instance.State = ColliderState.Default;
        }    
    }

    private void OnValueChanged(string newText)
    {
        if(!curComp) return;

        if (float.TryParse(newText, out float newValue))
        {
            curComp.componentCount = newValue;
        }
    }

    private void ChangeRotation (float angle)
    {
        if(!curComp) return;

        curComp.transform.Rotate(0f, angle, 0f);
    }
}
