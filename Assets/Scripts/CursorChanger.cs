using UnityEngine;
using UnityEngine.EventSystems;

public class CursorChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Texture2D pointerCursor;
    internal Vector2 hotSpot = new Vector2(5,0);
    internal CursorMode cursorMode = CursorMode.Auto;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(pointerCursor, hotSpot, cursorMode);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var manager = FindObjectOfType<CareManager>();
        Cursor.SetCursor(manager.mainCursor, manager.hotSpot, cursorMode);
    }
}
