using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Charge : MonoBehaviour, IInteractable
{
    [SerializeField] private ChargeStat chargeSetting;
    public ChargeStat ChargeSettings
    {
        get => chargeSetting;
    }

    [SerializeField] private float count;
    public float Count
    {
        get => count;
    }

    private void Awake()
    {
        switch (this.gameObject.name)
        {
            case "Proton":
                chargeSetting = ChargeStat.Plus;
                break;
            case "Neuron":
                chargeSetting = ChargeStat.Minus;
                break;
        }
    }

    public void Interact()
    {
        ChargeForceManager.Instance.ChargeIsPressed(this);
    }
}

public enum ChargeStat
{
    Plus,
    Minus
}