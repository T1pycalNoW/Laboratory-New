using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammeter : CircuitComponent
{
    [Header("Amperage Count")]
    public float Amperage;
    
    [Header("Pattern Settings")]
    [SerializeField] private Port port1Comp;

    public void UpdateAmperageCount()
    {
        Amperage = port1Comp.AmperageCount;
    }
}
