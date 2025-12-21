using UnityEngine;

public class ChargeForceManager : MonoBehaviour
{
    public static ChargeForceManager Instance;

    private bool isCounting = false;

    private Charge firstCharge;
    private Charge secondCharge;

    private void Awake ()
    {
        Instance = this;
    }

    public void ChargeIsPressed (Charge charge)
    {
        if(!isCounting)
        {
            firstCharge = charge;

            isCounting = true;
        }
        else
        {
            secondCharge = charge;

            ElectricChargeManager.Instance.FindForceBetweenTwoCharges(firstCharge, secondCharge);
        }
    }
}
