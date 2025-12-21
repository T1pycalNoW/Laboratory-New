using System;
using UnityEngine;

public class ElectricChargeManager : MonoBehaviour
{
    public static ElectricChargeManager Instance;

    [SerializeField] private double kStat = 9; // (10^9)

    private void Awake ()
    {
        Instance = this;
    }

    public void FindForceBetweenTwoCharges (Charge firstCharge, Charge secondCharge)
    {
        float distance = Vector3.Distance(firstCharge.gameObject.transform.position, secondCharge.gameObject.transform.position);

        double chargeProduct = Math.Round((double)Math.Abs(firstCharge.Count * secondCharge.Count), 2);
        double semiProduct = chargeProduct / Math.Pow(distance, 2);

        float answer = (float)(kStat * semiProduct);

        Debug.Log($"Сила Кулона равна: {answer} * 10^9");
    }
}