using UnityEngine;

public class Port : MonoBehaviour, IInteractable
{
    private static int counter = 1;

    [Header("Identificator")]
    [SerializeField] private int id;

    private CircuitComponent parentComponent;
    private int portNumber;

    public int ID
    {
        get => id;
    }

    private void Awake ()
    {
        id = counter;

        counter ++;

        parentComponent = transform.parent.GetComponent<CircuitComponent>();

        switch (this.gameObject.name)
        {
            case "Port1":
                portNumber = 1;
                break;
            case "Port2":
                portNumber = 2;
                break;
            case "Unique":
                portNumber = 3;
                break;
        }
    }

    public void Interact()
    {
        LineManager.Instance.ButtonIsPressed(this.gameObject);
    }

    public void AddNewPort (int _portID)
    {
        switch (portNumber)
        {
            case 1:
                parentComponent.AddNewPortTo1P(_portID);
                break;
            case 2:
                parentComponent.AddNewPortTo2P(_portID);
                break;
        }
    }
}
