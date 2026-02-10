using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class CircuitComponent : MonoBehaviour
{
    #region Component

    [Header("Component Settings")]
    public int ID;
    public ElectricCircuitCompType ComponentType;
    public float componentCount;

    public void ChangeValue(float value)
    {
        componentCount = value;
        if (uiText != null)
        {
            uiText.text = value.ToString();
        }
    }
    
    [SerializeField] private TextMeshProUGUI uiText;

    #endregion

    #region Port

    [Header("Ports Settings")]
    protected int port1;
    public int Port1
    {
        get => port1;
    }

    private List<int> connectedToPort1 = new();

    public List<int> ConnectedToPort1
    {
        get => connectedToPort1;
    }

    public void AddNewPortTo1P (int _port)
    {
        connectedToPort1.Add(_port);

        UpdateCode();
    }

    private int port2;
    public int Port2
    {
        get => port2;
    }

    private List<int> connectedToPort2 = new();
    public List<int> ConnectedToPort2
    {
        get => connectedToPort2;
    }

    public void InitPorts (int _port1, int _port2)
    {
        port1 = _port1;
        port2 = _port2;
    }

    public void AddNewPortTo2P (int _port)
    {
        connectedToPort2.Add(_port);

        UpdateCode();
    }

    #endregion

    #region Main Code

    private void Start ()
    {
        if(this.ComponentType == ElectricCircuitCompType.Point)
        {
            Port _port = transform.GetChild(1).GetComponent<Port>();
            InitPorts(_port.ID, _port.ID);

            return;
        }

        if (uiText != null)
        {
            ChangeValue(componentCount);
        }
        
        Port _port1 = transform.GetChild(1).GetComponent<Port>();
        Port _port2 = transform.GetChild(2).GetComponent<Port>();

        InitPorts(_port1.ID, _port2.ID);
    }

    #endregion

    #region Code

    /* 
    Особый код компонента составлен по данному правилу:

    {ID Первого порта этого компонента}, {ID Второго порта этого компонента}
    
    :

    {Component Count этого компонента}

    :

    {Порты подключенные к первому порту через запятую}

    :

    {Порты подключенные ко второму порту через запятую}
    */

    [Header("Special Code")]
    [SerializeField] private string specialCode;

    public string SpecialCode
    {
        get => specialCode;
    }

    public void UpdateCode ()
    {
        string newcode = "";

        newcode += port1.ToString();
        newcode += ",";
        newcode += port2.ToString();
        newcode += ",";
        newcode += ":";

        newcode += componentCount.ToString();
        newcode += ":";

        if(connectedToPort1.Count != 0)
        {
            for (int i = 0; i < connectedToPort1.Count; i++)
            {
                newcode += connectedToPort1[i].ToString();

                newcode += ",";
            }
        }

        newcode += ":";

        if(connectedToPort2.Count != 0)
        {
            for (int x = 0; x < connectedToPort2.Count; x++)
            {
                newcode += connectedToPort2[x].ToString();

                newcode += ",";
            }
        }
        
        newcode += ":";

        newcode += ComponentType.ToString();
    
        specialCode = newcode;
    }

    #endregion
}

public enum ElectricCircuitCompType
{
    VoltageSource, // Источник ЭДС
    Resistor, // Резистор
    Point, // Проводник
    Ammeter, // Амперметр
}