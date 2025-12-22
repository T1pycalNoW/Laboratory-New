using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [Header("Canvas Elements")]
    [SerializeField] private GameObject nPanel;
    [SerializeField] private GameObject bPanel;

    private bool nPanelIsOpen;
    private bool bPanelIsOpen;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            ChangeNPanelState();

            if (nPanelIsOpen && bPanelIsOpen)
            {
                ChangeBPanelState();
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ChangeBPanelState();

            if (nPanelIsOpen && bPanelIsOpen)
            {
                ChangeNPanelState();
            }
        }
    }

    private void ChangeBPanelState()
    {
        bPanelIsOpen = !bPanelIsOpen;

        bPanel.GetComponent<Animator>().SetBool("isOpen", bPanelIsOpen);
    }

    private void ChangeNPanelState()
    {
        nPanelIsOpen = !nPanelIsOpen;

        nPanel.GetComponent<Animator>().SetBool("isOpen", nPanelIsOpen);

        ChangeObjectInformation.Instance.enabled = nPanelIsOpen;

        if (nPanelIsOpen)
        {
            ObjectManager.Instance.State = ColliderState.Default;
        }
        else
        {
            ChangeObjectInformation.Instance.StopMoving();
            ObjectManager.Instance.State = ColliderState.Small;
        }
    }
}
