using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortTracker : MonoBehaviour
{
    public static PortTracker Instance;

    private List<Port> usedPorts = new();

    private int ammetersNumber = 0;

    public void AddNewAmmeter()
    {
        ammetersNumber++;
    }

    public void DeleteAmmeter()
    {
        ammetersNumber--;
    }

    public bool CheckAmmeterNum()
    {
        if (ammetersNumber == 0)
        {
            Debug.Log("No ammeters found");
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Awake ()
    {
        Instance = this;
    }

    public void ClearPorts ()
    {
        usedPorts.Clear();
        ammetersNumber = 0;
    }

    public void AddNewPort (Port port)
    {
        usedPorts.Add(port);
    }

    public bool CheckPort (Port _port)
    {
        return usedPorts.Contains(_port) ? true : false;
    }
}
