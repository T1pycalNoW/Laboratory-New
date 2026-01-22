using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammeter : CircuitComponent
{
    [Header("Amperage Count")]
    public float Amperage;
    
    [Header("Pattern Settings")]
    [SerializeField] private Port port1Comp;
    [SerializeField] private Port port2Comp;

    public void UpdateAmperageCount()
    {
        if(port1Comp.AmperageCount != 0)
        {
            Amperage = port1Comp.AmperageCount;
            
            PortTracker.Instance.DeleteAmmeter();
            
            Debug.Log("Succesfull");
        }
        else if (port2Comp.AmperageCount != 0)
        {
            Amperage = port2Comp.AmperageCount;
            
            PortTracker.Instance.DeleteAmmeter();
            
            Debug.Log("Succesfull");
        }
        else
        {
            Debug.Log("Mistake!");
        }
    }
}
