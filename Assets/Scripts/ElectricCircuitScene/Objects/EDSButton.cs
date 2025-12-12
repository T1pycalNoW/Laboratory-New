using UnityEngine;

public class EDSButton : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        CircuitManager.Instance.StartCircuitByEDC(this.gameObject.transform.parent.GetComponent<VoltageSource>());
    }
}
