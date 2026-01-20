using System;
using UnityEngine;
using System.Collections.Generic;
using GogoGaga.OptimizedRopesAndCables;

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
        Debug.Log("Start Counting Amperage");
        
        Port oppositePort = transform.parent.GetComponent<PortManager>().GetOppositePort(this);

        this.amperageCount = ammerStrength;

        if (oppositePort.AmperageCount == 0f)
        {
            oppositePort.ContinueCountingAmperStrength(ammerStrength);

            oppositePort.amperageCount = ammerStrength;
        }
        else
        {
            Debug.Log("Ammeter count has already been counted");
        }
    }

    public void ContinueCountingAmperStrength(float ammerStrength)
    {
        Debug.Log("Continue Counting Amperage");
        
        List<Resistor> resistors = new List<Resistor>();
        
        List<CountResistor> countResistors = new List<CountResistor>();
        List<float> countAmperages = new List<float>();
        
        foreach (Rope rope in connectedLines)
        {
            Transform port1 =  rope.StartPoint.parent;
            Transform port2 = rope.EndPoint.parent;

            if (port1 != null && port2 != null)
            {
                throw new System.Exception("Ports are not found");
            }
            
            Transform oppositePort = port1 == this.transform ? port2 : port1;
            
            if(oppositePort.parent.TryGetComponent<Resistor>(out Resistor resistor))
            {
                resistors.Add(resistor);
                
                CountResistor newCountResistor = new(resistor.componentCount, resistor, oppositePort.GetComponent<Port>());
            
                countResistors.Add(newCountResistor);
            }
            else if (oppositePort.parent.TryGetComponent<Resistor>(out Ammeter ammeter))
            {
                resistors.Add(resistor).Add(ammerStrength);
            }
            else
            {
                throw new System.Exception("Did not found Resistor or Ammeter"); 
            }
        }

        if (resistors.Count == 1)
        {
            countResistors[0].EnterPort.StartCountingAmperStrength(ammerStrength);
        }

        foreach (var resistor in countResistors)
        {
            float otherResistorSum = 0f;

            float newNnAmperValue = 0f;
            
            foreach (var other in countResistors)
            {
                if (other == resistor) continue;

                otherResistorSum += other.ResistorCount;
            }
            
            newNnAmperValue = (resistor.ResistorCount / otherResistorSum) * ammerStrength;
            
            countAmperages.Add(newNnAmperValue);
        }
        
        countResistors.Sort((x, y) => y.ResistorCount.CompareTo(y.ResistorCount)); // Сортировка по возрастанию
        countAmperages.Sort();

        for (int i = 0; i < countResistors.Count; i++)
        {
            countResistors[i].EnterPort.StartCountingAmperStrength(countAmperages[i]);
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

        private Resistor resistorObject;

        public Resistor ResistorObject
        {
            get => resistorObject;
        }

        private Port enterPort;

        public Port EnterPort
        {
            get => enterPort;
        }

        public CountResistor(float _resistorCount, Resistor _resistorObject, Port _enterPort)
        {
            resistorCount = _resistorCount;
            resistorObject = _resistorObject;
            enterPort = _enterPort;
        }
    }
    
    #endregion
}
