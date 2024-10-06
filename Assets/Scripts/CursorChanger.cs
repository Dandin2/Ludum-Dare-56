using UnityEngine;
using UnityEngine.EventSystems;

public class CursorChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Texture2D pointerCursor;
    internal Vector2 hotSpot = new Vector2(5,0);
    internal CursorMode cursorMode = CursorMode.Auto;

    public void OnPointerEnter(PointerEventData eventData)
    {
        var manager = FindObjectOfType<CareManager>();
        if (manager.DraggingItem == null && !manager.isUsingScrubBrush)
        {
            Cursor.SetCursor(pointerCursor, hotSpot, cursorMode);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var manager = FindObjectOfType<CareManager>();
        if (!manager.isUsingScrubBrush)
        {
            manager.SetMainCursor();
        }
    }
}
