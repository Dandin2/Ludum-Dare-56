using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ItemType
{
    Food,
    Toy
}

public class ShopItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image ItemImage;

    public Text GoldAmountText;

    internal int GoldAmount;

    internal string ItemName;

    internal string ItemDescription;

    internal ItemType ItemType;

    private CareManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<CareManager>();
        GoldAmountText.text = GoldAmount + "G";
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

    public void BuyItemClick()
    {
        if (WorldManager.instance.HasEnoughGold(GoldAmount))
        {
            WorldManager.instance.RemoveGold(GoldAmount);
            if (ItemType == ItemType.Food)
            {
                var foodStats = ScriptableObjectFinder.FindScriptableObjectByName<FoodStats>(ItemName);
                manager.FoodOwned.Add(Instantiate(foodStats.FoodPrefab));
            }
            else if (ItemType == ItemType.Toy)
            {
                var toyStats = ScriptableObjectFinder.FindScriptableObjectByName<ToyStats>(ItemName);
                manager.ToysOwned.Add(Instantiate(toyStats.ToyPrefab));
            }
            manager.SetupInventoryImages();
        }
    }
}
