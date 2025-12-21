using UnityEngine;

public class Panel : MonoBehaviour
{
    public void PressButton (int index)
    {
        PlaceNewObjects.Instance.ButtonIsPressed(index);
    }
}
