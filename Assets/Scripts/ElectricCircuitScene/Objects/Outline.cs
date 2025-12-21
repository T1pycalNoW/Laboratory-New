using UnityEngine;

public class Outline : MonoBehaviour
{
    private MeshRenderer ms;

    private Color defaultColor;

    private void Awake ()
    {
        ms = GetComponent<MeshRenderer>();

        defaultColor = ms.material.color;
    }

    private void OnEnable ()
    {
        ms.material.color = Color.green;
    }

    private void OnDisable ()
    {
        ms.material.color = defaultColor;
    }
}

