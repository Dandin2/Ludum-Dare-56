using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image ItemImage;

    public Text GoldAmountText;

    internal string ItemName;

    internal string ItemDescription;

    private CareManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<CareManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        manager.ItemImage.sprite = ItemImage.sprite;
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
