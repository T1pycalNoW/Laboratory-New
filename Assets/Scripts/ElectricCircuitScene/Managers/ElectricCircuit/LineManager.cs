using System.Collections.Generic;
using System.Security.Cryptography;
using GogoGaga.OptimizedRopesAndCables;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    public static LineManager Instance;

    [Header("Line Settings")]
    [SerializeField] private GameObject RopePrefab;

    [Header("Created Lines")]
    public List<GameObject> connectedLines = new List<GameObject>();

    [Header("Points")]
    [SerializeField] private GameObject StartPoint;
    [SerializeField] private GameObject EndPoint;

    private Rope curLine;
    private GameObject curPort;
    private bool isDrawing;
    private Port firstPort;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (isDrawing)
        {
            Transform hitPoint = MouseRaycast.Instance.GetMouseClickTransform();

            if (hitPoint != null)
            {
                curLine.SetEndPoint(hitPoint.transform);
            }

            if(Input.GetKeyDown(KeyCode.Delete))
            {
                StopDrawing();
            }
        }
    }

    public void ButtonIsPressed(GameObject button) // При нажатии мышкой по кнопке
    {
        if (!isDrawing)
        {
            StartDrawLine(button);
            return;
        }

        if (button == curLine.StartPoint.transform.parent.gameObject)
        {
            return;
        }

        FinishDrawLine(button);
    }

    private void StartDrawLine(GameObject obj1)
    {
        GameObject newRope = Instantiate(RopePrefab, Vector3.zero, Quaternion.identity);

        GameObject startPoint = Instantiate(StartPoint, obj1.transform);

        Rope newRopeComp = newRope.GetComponent<Rope>();

        newRopeComp.SetStartPoint(startPoint.transform);
        curLine = newRopeComp;
        curPort = startPoint;
        isDrawing = true;

        firstPort = obj1.GetComponent<Port>();
    }

    private void FinishDrawLine(GameObject obj2)
    {
        if(obj2.name == firstPort.name)
        {
            Debug.Log("Ошибка! Нельзя соединять порты с одинаковым индексом");
            return;
        }
        
        if(obj2.transform.parent == firstPort.transform.parent)
        {
            Debug.Log("Ошибка! Нельзя соединять порты одного и того же объекта");
            return;
        }

        GameObject endPoint = Instantiate(EndPoint, obj2.transform);

        curLine.SetEndPoint(endPoint.transform);
        connectedLines.Add(curLine.gameObject);

        CircuitComponent parent1 = curLine.StartPoint.parent.parent.gameObject.GetComponent<CircuitComponent>();
        CircuitComponent parent2 = endPoint.transform.parent.parent.gameObject.GetComponent<CircuitComponent>();

        Port secondPort = obj2.GetComponent<Port>();

        firstPort.AddNewPort(secondPort.ID);
        secondPort.AddNewPort(firstPort.ID);
        
        CircuitManager.Instance.AddNewCircuitComponents(parent1, parent2);

        curLine = null;
        isDrawing = false;
        firstPort = null;
    }

    public void ClearAllConnections()
    {
        foreach (var line in connectedLines)
        {
            if (line != null)
            {
                Destroy(line.gameObject);
            }
        }
        connectedLines.Clear();
    }

    private void StopDrawing()
    {
        Destroy(curLine.gameObject);
        Destroy(curPort);

        isDrawing = false;
        curLine = null;
        firstPort = null;
    }
}
