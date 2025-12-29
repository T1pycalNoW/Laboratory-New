using System;
using System.Collections.Generic;
using UnityEngine;

public class CircuitManager : MonoBehaviour
{
    #region Base

    public static CircuitManager Instance;

    public List<Circuit> AllCircuits = new List<Circuit>();

    public List<CountComp> currentAnalyzeCircuit = new();

    private CircuitComponent curComp1;
    private CircuitComponent curComp2;

    void Awake()
    {
        Instance = this;
    }

    public void StartCircuitByEDC (CircuitComponent EDC)
    {
        foreach (var i in AllCircuits)
        {
            if(i.ComponentList.Contains(EDC))
            {
                i.StartCircuit();
            }
        }
    }

    // Соединение двух новых компонентов

    public void AddNewCircuitComponents(CircuitComponent CC1, CircuitComponent CC2)
    {
        curComp1 = CC1;
        curComp2 = CC2;

        CheckID();
    }

    #endregion

    #region Curcit ID

    /*
    Проверяет ID (наличие или отсутсвие подключения к существующим цепям) добавленных компонентов:
    1. Если кто-то уже подключен, то добавляет второй к нему в цепь.
    2. Если оба подключены к цепи, то объединяет цепи.
    3. Если ни один не подключен, создает новую цепь
    */

    private void CheckID()
    {
        Debug.Log($"ID1 = {curComp1.ID}, ID2 = {curComp2.ID}");

        if (curComp1.ID == 0 && curComp2.ID == 0)
        {
            Circuit newCircuit = new Circuit(curComp1, curComp2);

            curComp1.ID = newCircuit.ID;
            curComp2.ID = newCircuit.ID;

            AllCircuits.Add(newCircuit);
        }
        else if (curComp1.ID == 0 && curComp2.ID != 0)
        {
            curComp1.ID = curComp2.ID;

            Circuit circuit = SearchCircuitByID(curComp1.ID);
            circuit.AddNewComp(curComp1);
        }

        else if (curComp1.ID != 0 && curComp2.ID == 0)
        {
            curComp2.ID = curComp1.ID;

            Circuit circuit = SearchCircuitByID(curComp2.ID);
            circuit.AddNewComp(curComp2);
        }

        else
        {
            ChangeAllCircuitID(curComp1.ID, curComp2.ID);
        }
    }

    // Поиск ID в уже существующих цепях

    private Circuit SearchCircuitByID(int id)
    {
        foreach (var i in AllCircuits)
        {
            if (i.ID == id)
            {
                return i;
            }
        }

        return null;
    }

    // Смена всех ID у цепи (2 случай метода CheckID())

    private void ChangeAllCircuitID(int change_id1, int to_id2)
    {
        Circuit change_circuit = null;
        Circuit to_circuit = null;

        foreach (var i in AllCircuits)
        {
            if (i.ID == change_id1)
            {
                change_circuit = i;
            }
            else if (i.ID == to_id2)
            {
                to_circuit = i;
            }
        }

        if (to_circuit != null)
        {
            foreach (var y in change_circuit.ComponentList)
            {
                y.ID = to_circuit.ID;
                to_circuit.AddNewComp(y);
            }

            AllCircuits.Remove(change_circuit);
        }
    }
}

#endregion

#region Circuit

[Serializable]
public class Circuit
{
    private static int counter = 1;

    public string Name { get; }
    public int ID { get; }
    [SerializeField] private List<CircuitComponent> componentList = new();
    public List<CircuitComponent> ComponentList
    {
        get => componentList;
    }

    [SerializeField] private List<CountComp> convertedCodes = new();
    public List<CountComp> ConvertedCodes
    {
        get => convertedCodes;
    }

    public Circuit(CircuitComponent circComp1, CircuitComponent circComp2)
    {
        Name = $"Circuit ({ID})";

        componentList.Add(circComp1);
        componentList.Add(circComp2);

        ID = counter;
        counter++;
    }

    public void AddNewComp(CircuitComponent comp)
    {
        componentList.Add(comp);
    }

    // Запуск Цепи

    public void StartCircuit()
    {
        List<VoltageSource> allVoltageCircuits = new();

        foreach (var i in componentList)
        {
            if (i.ComponentType == ElectricCircuitCompType.VoltageSource)
            {
                if (i.TryGetComponent(out VoltageSource VC))
                {
                    Debug.Log($"Special: {i.gameObject}");
                    allVoltageCircuits.Add(VC);
                }
            }
        }

        if (allVoltageCircuits.Count == 0)
        {
            Debug.Log("В цепи отсутствует ЭДС, запуск невозможен!");
            return;
        }

        if (allVoltageCircuits.Count == 1)
        {
            ConvertComponentsToSpecialCode();

            if (!CheckStateOfCircuit())
            {
                Debug.Log("Цепь не соединена! Проверьте подкючение");
                return;
            }

            while(true)
            {
                bool state = FindElementsThatCanBeRemoved();
                
                if(!state)
                {
                    break;
                }
            }

            Debug.Log($"Общее сопротивление цепи: {convertedCodes[0].Count}.");
        }
        else
        {

        }
    }

    // Проверка на то, соединены ли все компоненты в цепи или нет (замкнутая ли цепь)
    private bool CheckStateOfCircuit()
    {
        foreach (var i in convertedCodes)
        {
            if (i.ConToPort1.Count == 0 || i.ConToPort2.Count == 0)
            {
                return false;
            }
        }

        return true;
    }

    // Поиск параллельных и последовательных соединений
    private bool FindElementsThatCanBeRemoved()
    {
        for (int x = 0; x < convertedCodes.Count; x++)
        {
            for (int y = 0; y < convertedCodes.Count; y++)
            {
                if (convertedCodes[x].ID != convertedCodes[y].ID)
                {
                    Debug.Log("1 check passed");
                    if (convertedCodes[x].Count != 0 && convertedCodes[y].Count != 0)
                    {
                        if(!FindParallelConnection(convertedCodes[x], convertedCodes[y]))
                        {
                            if(FindNextConnection(convertedCodes[x], convertedCodes[y]))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    // При нахождении параллельного соединения
    private bool FindParallelConnection(CountComp x, CountComp y)
    {
        if (x.ConToPort1.Count == 1 && x.ConToPort2.Count == 1 && y.ConToPort1.Count == 1 && y.ConToPort2.Count == 1)
        {
            if ((x.ConToPort1[0] == y.ConToPort1[0]) && (x.ConToPort2[0] == y.ConToPort2[0]))
            {
                Debug.Log("Parallel connection founded");

                List<CountComp> allParallelConnections = new()
                {
                    x,
                    y
                };

                double sum = 0d;

                foreach (var i in convertedCodes)
                {
                    if (i.ConToPort1.Count == 1 && i.ConToPort2.Count == 1)
                    {
                        if (i.ConToPort1[0] == x.ConToPort1[0] && i.ConToPort2[0] == x.ConToPort2[0])
                        {
                            if (!allParallelConnections.Contains(i))
                            {
                                Debug.Log("Found New Element!");

                                allParallelConnections.Add(i);
                            }
                        }
                    }
                }

                foreach (var l in allParallelConnections)
                {
                    sum += Math.Round(1 / l.Count, 2);
                }

                double answer = Math.Round(1 / sum, 2);

                CountComp newComp = new(x.Port1, x.Port2, (float)answer, x.ConToPort1, x.ConToPort2);

                foreach (var l in allParallelConnections)
                {
                    if(l.ID != x.ID)
                    {
                        ClearAllPortData(l.Port1);
                        ClearAllPortData(l.Port2);
                    }                  

                    convertedCodes.Remove(l);   
                }

                convertedCodes.Add(newComp);

                return true;
            }
        }

        return false;
    }

    // При нахождении последовательного соединения
    private bool FindNextConnection(CountComp x, CountComp y)
    {
        Debug.Log($"Проверка элементов: {x.ID}, {y.ID}. 2 порт {x.ID} элемента: {x.ConToPort2[0]}, 1 порт {y.ID} элемента: {y.Port1}");

        if (x.ConToPort2.Count == 1 && y.ConToPort1.Count == 1)
        {
            if ((x.ConToPort2[0] == y.Port1) || (x.ConToPort2[0] == y.ConToPort1[0]))
            {
                Debug.Log("Next connection founded");

                double answer = x.Count + y.Count;

                CountComp newComp = new(x.Port1, y.Port2, (float)answer, x.ConToPort1, y.ConToPort2);

                convertedCodes.Remove(x);
                convertedCodes.Remove(y);
                convertedCodes.Add(newComp);

                return true;
            }
        }

        return false;
    }

    // Очищает информацию о тех портах, которые были удалены во время поиска параллельных или последовательных соединений
    private void ClearAllPortData(int port)
    {
        foreach (var x in convertedCodes)
        {
            if (x.ConToPort1.Contains(port))
            {
                x.ConToPort1.Remove(port);
            }

            if (x.ConToPort2.Contains(port))
            {
                x.ConToPort2.Remove(port);
            }
        }
    }

    /*
    Метод разбивает специальные коды каждого компонента, добавленного в сеть, на отдельные элементы и записывает их в массив особого класса, 
    который эти элементы и хранит. Это нужно для последующих вычислений
    */

    #region Convertation

    private void ConvertComponentsToSpecialCode()
    {
        List<string> allCircuitComponentCodes = new();
        convertedCodes.Clear();

        foreach (var i in componentList)
        {
            if (i.ComponentType == ElectricCircuitCompType.Resistor)
            {
                allCircuitComponentCodes.Add(i.SpecialCode);
            }
        }

        foreach (var x in allCircuitComponentCodes)
        {
            string[] curStr = x.Split(':');
            string str = "";
            int p1 = 0;
            int p2 = 0;

            for (int i = 0; i < curStr[0].Length; i++)
            {
                if (curStr[0][i] == ',')
                {
                    if (p1 == 0)
                    {
                        p1 = int.Parse(str);
                        str = "";
                    }
                    else
                    {
                        p2 = int.Parse(str);
                        str = "";
                    }
                }
                else
                {
                    str += curStr[0][i];
                }
            }

            float c = float.Parse(curStr[1]); // 1:2:fjkd:5,6,7:8,9,3
            // Debug.Log(c);
            List<int> c1 = new();
            List<int> c2 = new();

            for (int x1 = 0; x1 < curStr[2].Length; x1++)
            {
                if ((curStr[2][x1] == ',') && str != "")
                {
                    c1.Add(int.Parse(str));
                    str = "";
                }
                else
                {
                    str += curStr[2][x1];
                }
            }

            str = "";

            for (int x2 = 0; x2 < curStr[3].Length; x2++)
            {
                if ((curStr[3][x2] == ',') && str != "")
                {
                    c2.Add(int.Parse(str));
                    str = "";
                }
                else
                {
                    str += curStr[3][x2];
                }
            }

            CountComp newCountComp = new(p1, p2, c, c1, c2);
            convertedCodes.Add(newCountComp);

            Debug.Log($"Added new code: port1 = {p1}, port2 = {p2}");
        }
    }

    #endregion
}

#endregion

#region Count Comp

[Serializable]
public class CountComp
{
    private static int counter = 1;

    [SerializeField] private int id;
    public int ID
    {
        get => id;
    }

    [SerializeField] private int port1;
    public int Port1
    {
        get => port1;
    }

    [SerializeField] private int port2;
    public int Port2
    {
        get => port2;
    }

    [SerializeField] private float count;
    public float Count
    {
        get => count;
    }

    [SerializeField] private List<int> conToPort1 = new();
    public List<int> ConToPort1
    {
        get => conToPort1;
    }

    [SerializeField] private List<int> conToPort2 = new();
    public List<int> ConToPort2
    {
        get => conToPort2;
    }

    public CountComp(int _port1, int _port2, float _count, List<int> _conToPort1, List<int> _conToPort2)
    {
        id = counter;
        counter++;

        port1 = _port1;
        port2 = _port2;
        count = _count;

        foreach (var i in _conToPort1)
        {
            conToPort1.Add(i);
        }

        foreach (var x in _conToPort2)
        {
            conToPort2.Add(x);
        }
    }
}

#endregion