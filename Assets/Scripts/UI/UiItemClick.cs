using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class UiItemClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    internal string ItemName;
    internal string ItemDescription;

    private CareManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<CareManager>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!manager.isUsingScrubBrush)
        {
            manager.SetClickCursor();
            if (manager.UiToGameObjectDictionary.TryGetValue(gameObject.GetInstanceID(), out var item))
            {
                GetComponent<Image>().enabled = false;
                manager.DraggingItem = item;
                manager.DraggingItem.SetActive(true);
                item.GetComponent<LineRenderer>().enabled = true;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (manager.DraggingItem != null)
        {
            if (manager.IsDroppedWithinFence())
            {
                manager.DraggingItem.GetComponent<LineRenderer>().enabled = false;
                var food = manager.DraggingItem.GetComponent<Food>();
                if(food != null)
                {
                    food.enabled = true;
                    manager.FoodOwned.Remove(manager.DraggingItem);
                }
                var toy = manager.DraggingItem.GetComponent<Toy>();
                if(toy != null)
                {
                    toy.enabled = true;
                    manager.ToysOwned.Remove(manager.DraggingItem);
                }
                manager.DraggingItem = null;
                manager.ItemPopupPanel.SetActive(false);
                manager.SetMainCursor();
                manager.UiToGameObjectDictionary.Remove(gameObject.GetInstanceID());
                manager.UiToUiGameObjectDictionary.Remove(gameObject.GetInstanceID());
                Destroy(this.gameObject);
            }
            else
            {
                manager.DraggingItem.GetComponent<LineRenderer>().enabled = false;
                GetComponent<Image>().enabled = true;
                manager.DraggingItem.SetActive(false);
                manager.DraggingItem = null;
                manager.ItemPopupPanel.SetActive(false);
                manager.SetMainCursor();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        manager.ItemImage.sprite = GetComponent<Image>().sprite;
        manager.ItemNameText.text = ItemName;
        manager.ItemInfoText.text = ItemDescription;
        manager.ItemPopupPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (manager.DraggingItem == null)
        {
            manager.ItemPopupPanel.SetActive(false);
        }
    }
}
