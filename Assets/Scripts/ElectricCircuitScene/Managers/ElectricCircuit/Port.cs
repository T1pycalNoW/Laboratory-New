using System;
using UnityEngine;
using System.Collections.Generic;
using GogoGaga.OptimizedRopesAndCables;
using System.Collections;

public class Port : MonoBehaviour, IInteractable
{
    #region Port
    
    private static int counter = 1;

    [Header("Identificator")]
    [SerializeField] private int id;

    private CircuitComponent parentComponent;
    private int portNumber;

    [SerializeField] private float amperageCount;
    public float AmperageCount { get => amperageCount; set => amperageCount = value > 0 ? value : 0; }

    public int ID
    {
        get => id;
    }

    private void Awake () 
    {
        id = counter;

        counter ++;

        parentComponent = transform.parent.GetComponent<CircuitComponent>();

        switch (this.gameObject.name)
        {
            case "Port1":
                portNumber = 1;
                break;
            case "Port2":
                portNumber = 2;
                break;
            case "Unique":
                portNumber = 3;
                break;
        }
    }

    public void Interact()
    {
        LineManager.Instance.ButtonIsPressed(this.gameObject);
    }

    public void AddNewPort (int _portID)
    {
        switch (portNumber)
        {
            case 1:
                parentComponent.AddNewPortTo1P(_portID);
                break;
            case 2:
                parentComponent.AddNewPortTo2P(_portID);
                break;
        }
    }
    
    #endregion
    
    #region Line

    [Header(("Connected Lines"))] 
    [SerializeField] private List<Rope> connectedLines = new();

    public void AddNewLine(Rope newLine)
    {
        connectedLines.Add(newLine);
    }

    public void StartCountingAmperStrength(float ammerStrength)
    {
        StartCoroutine(StartCounting(ammerStrength));
    }

    public IEnumerator StartCounting (float ammerStrength)
    {
        if (PortTracker.Instance.CheckPort(this) || PortTracker.Instance.CheckAmmeterNum())
        {
            yield break;
        }

        PortTracker.Instance.AddNewPort(this);

        Debug.Log("Start Counting Amperage");
        
        Port oppositePort = transform.parent.GetComponent<PortManager>().GetOppositePort(this);

        this.amperageCount = ammerStrength;

        if (oppositePort.AmperageCount == 0f)
        {
            yield return StartCoroutine(oppositePort.ContinueCountingAmperStrength(ammerStrength));

            oppositePort.amperageCount = ammerStrength;
        }
        else
        {
            Debug.Log("Ammeter count has already been counted");
        }
    }

    public IEnumerator ContinueCountingAmperStrength(float ammerStrength)
    {
        if (PortTracker.Instance.CheckPort(this) || PortTracker.Instance.CheckAmmeterNum())
        {
            Debug.Log("Quitting...");
            yield break;
        }

        PortTracker.Instance.AddNewPort(this);

        ammerStrength = (float)Math.Round(ammerStrength, 2);

        Debug.Log("Continue Counting Amperage");
        Debug.Log(this.ID);
        Debug.Log(ammerStrength);

        if (this.transform.parent.TryGetComponent(out Ammeter ammeterComp))
        {
            ammeterComp.UpdateAmperageCount();
        }
        
        List<CircuitComponent> resistors = new List<CircuitComponent>();
        
        List<CountResistor> countResistors = new List<CountResistor>();
        List<float> countAmperages = new List<float>();

        if (connectedLines.Count == 0)
        {
            throw new Exception("Did not found any Lines");
        }
        
        foreach (Rope rope in connectedLines)
        {
            Transform port1 =  rope.StartPoint.parent;
            Transform port2 = rope.EndPoint.parent;

            if (!port1 || !port2)
            {
                throw new System.Exception("Ports are not found");
            }
            
            Transform oppositePort = port1 == this.transform ? port2 : port1;
            
            if(oppositePort.parent.TryGetComponent<Resistor>(out Resistor resistor))
            {
                resistors.Add(resistor);
                
                CountResistor newCountResistor = new(resistor.componentCount, resistor, oppositePort.GetComponent<Port>());

                Debug.Log($"Показатель резистора: {resistor.componentCount}");
            
                countResistors.Add(newCountResistor);
            }
            else if (oppositePort.parent.TryGetComponent<Ammeter>(out Ammeter ammeter))
            {
                resistors.Add(ammeter);
                
                CountResistor newCountResistor = new(ammeter.componentCount, ammeter, oppositePort.GetComponent<Port>());
            
                countResistors.Add(newCountResistor);

                ammeter.UpdateAmperageCount();
            }
            else
            {
                Debug.Log("Did not found Resistor or Ammeter"); 
                
                yield break;
            }
        }

        if (resistors.Count == 1)
        {
            Debug.Log("Only one");
            
            countResistors[0].EnterPort.StartCountingAmperStrength(ammerStrength);

            yield break;
        }
        
        Debug.Log($"Отладка: параллельное соединение. Найдено резисторов (включая амперметры) - {resistors.Count}, " +
                  $"переведено в расчетную систему - {countResistors.Count}.");
        
        float otherResistorSum = 0f;
        
        foreach (var other in countResistors)
        {
            otherResistorSum += other.ResistorCount;
        }

        foreach (var resistor in countResistors)
        {
            float newNnAmperValue = 0f;
            
            newNnAmperValue = (float)Math.Round(resistor.ResistorCount / otherResistorSum * ammerStrength, 2);
            
            Debug.Log($"Добавление абстрактного показателя тока: resistor.Count - {resistor.ResistorCount}, resistorSum - {otherResistorSum}, ammerStrength - {ammerStrength}");
            
            countAmperages.Add(newNnAmperValue);
        }

        for (int x = 0; x < resistors.Count; x++)
        {
            for (int y = 0; y < resistors.Count; y++)
            {
                if (countResistors[x].ResistorCount > countResistors[y].ResistorCount)
                {
                    CountResistor countResistor = countResistors[x];
                    countResistors[x] = countResistors[y];
                    countResistors[y] =  countResistor;
                }
            }
        }
        
        countAmperages.Sort();

        yield return null;

        Coroutine[] coroutines = new Coroutine[countResistors.Count];
        for (int i = 0; i < countResistors.Count; i++)
        {
            if (PortTracker.Instance.CheckAmmeterNum()) yield break;

            Debug.Log($"Итерация №{i}");
            
            Debug.Log($"Порту {countResistors[i].EnterPort.ID} присвоено значение - {countAmperages[i]}");
            
            coroutines[i] = StartCoroutine(countResistors[i].EnterPort.StartCounting(countAmperages[i]));
        }

        foreach (var coroutine in coroutines)
        {
            yield return coroutine;
        }

        if (countResistors.Count == 0)
        {
            throw new Exception("Did not found any Ports");
        }
    }
    
    private class CountResistor
    {
        private float resistorCount;

        public float ResistorCount
        {
            get => resistorCount;
        }

        private CircuitComponent resistorObject;

        public CircuitComponent ResistorObject
        {
            get => resistorObject;
        }

        private Port enterPort;

        public Port EnterPort
        {
            get => enterPort;
        }

        public CountResistor(float _resistorCount, CircuitComponent _resistorObject, Port _enterPort)
        {
            resistorCount = _resistorCount;
            resistorObject = _resistorObject;
            enterPort = _enterPort;
        }
    }
    
    #endregion
}
