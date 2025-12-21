using Unity.VisualScripting;
using UnityEngine;

public class Button : MonoBehaviour
{
    private Outline outline;

    private void Awake ()
    {
        outline = GetComponent<Outline>();
    }

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
