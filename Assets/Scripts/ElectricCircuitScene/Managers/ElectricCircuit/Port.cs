using UnityEngine;

public class Port : MonoBehaviour
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

        if(this.gameObject.name == "Port1")
        {
            portNumber = 1;
        }
        else
        {
            portNumber = 2;
        }
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
