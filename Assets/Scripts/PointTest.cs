using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverTest : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Button Hovered");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Button Hover Ended");
    }
}