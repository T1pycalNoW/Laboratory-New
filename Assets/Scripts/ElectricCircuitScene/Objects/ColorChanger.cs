using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    /// <summary>
    /// Меняет цвет объекта, путём замены всех материалов в его детях.
    /// </summary>
    /// <param name="changeObject">Transform изменяемого объекта.</param>
    /// <param name="type">Цвет: "Green" - заленый, "Red" - красный, "White" - белый, "Default" - серый, "Purple" - фиолетовый.</param>
    public void ChangeMaterial(Transform changeObject, string type)
    {
        Debug.Log($"Changing... Change on {type}");
        Color newColor = Color.gray;

        switch (type)
        {
            case "Green":
                newColor = Color.green;
                break;
            case "Red":
                newColor = Color.red;
                break;
            case "Purple":
                newColor = Color.magenta;
                break;
            case "White":
                newColor = Color.white;
                break;
        }

        for (int i = 0; i < changeObject.childCount; i++)
        {
            if (changeObject.GetChild(i).TryGetComponent(out MeshRenderer mc))
            {
                //mc.material = changeMaterial;
                mc.material.color = newColor;
            }
        }

        for (int i = 0; i < changeObject.childCount; i++)
        {
            if (changeObject.GetChild(0).GetChild(i).TryGetComponent(out MeshRenderer mc))
            {
                //mc.material = changeMaterial;
                mc.material.color = newColor;
            }
        }
    }
}
