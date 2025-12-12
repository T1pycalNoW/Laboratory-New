using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] private Outline outline;

    private void OnMouseEnter()
    {
        if (outline != null)
        {
            outline.enabled = true;
        }
    }

    private void OnMouseExit()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }
}
