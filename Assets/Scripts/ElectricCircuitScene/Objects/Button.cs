using UnityEngine;

[RequireComponent(typeof(Outline))]
public class Button : MonoBehaviour, IInteractable
{
    private Outline outline;

    private void Start()
    {
        outline = GetComponent<Outline>();    
    }

    public void Interact()
    {
        LineManager.Instance.ButtonIsPressed(this.gameObject);
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
