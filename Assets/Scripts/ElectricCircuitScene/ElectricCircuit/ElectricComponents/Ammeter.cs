using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ammeter : CircuitComponent
{
    [Header("Amperage Count")]
    public float Amperage;
    
    [Header("Pattern Settings")]
    [SerializeField] private Port port1Comp;
    [SerializeField] private Port port2Comp;

    [SerializeField] private TextMeshProUGUI uiAmmText;

    public void UpdateAmperageCount()
    {
        if(port1Comp.AmperageCount != 0)
        {
            Amperage = port1Comp.AmperageCount;
            
            UpdateUI(Amperage);
            
            PortTracker.Instance.DeleteAmmeter();
            
            Debug.Log("Succesfull");
        }
        else if (port2Comp.AmperageCount != 0)
        {
            Amperage = port2Comp.AmperageCount;
            
            UpdateUI(Amperage);
            
            PortTracker.Instance.DeleteAmmeter();
            
            Debug.Log("Succesfull");
        }
        else
        {
            Debug.Log("Mistake!");
        }
    }

    private void UpdateUI(float num)
    {
        uiAmmText.text = num.ToString();
    }
}
