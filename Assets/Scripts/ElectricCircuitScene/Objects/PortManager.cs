using UnityEngine;
using System.Collections.Generic;

public class PortManager : MonoBehaviour
{
    [SerializeField] private Port port1;
    [SerializeField] private Port port2;

    private void Start()
    {
        if (!CheckPorts())
        {
            throw new System.Exception("Ports are not initialized");
        }
    }

    public Port GetOppositePort(Port port)
    {
        if(port != port1 && port != port2) throw new System.Exception("Not valid enter information");

        return port == port1 ? port2 : port1;
    }
    
    private bool CheckPorts()
    {
        return (port1 && port2) ? true : false ;
    }
}
