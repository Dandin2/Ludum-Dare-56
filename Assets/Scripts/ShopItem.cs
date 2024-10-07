using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ItemType
{
    Food,
    Toy,
    Egg
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
                var foodStats = WorldManager.instance.FoodBases.First(x => x.name == ItemName);
                var go = Instantiate(foodStats.FoodPrefab);
                go.SetActive(false);
                manager.FoodOwned.Add(go);
            }
            else if (ItemType == ItemType.Toy)
            {
                var toyStats = WorldManager.instance.ToyBases.First(x => x.name == ItemName);
                var go = Instantiate(toyStats.ToyPrefab);
                go.SetActive(false);
                manager.ToysOwned.Add(go);
            }
            else if (ItemType == ItemType.Egg)
            {
                var eggStats = WorldManager.instance.EggBases.First(x => x.name == ItemName);
                Instantiate(eggStats.EggPrefab, manager.GetNextRandomPosition(), Quaternion.identity);
            }
            manager.SetupInventoryImages();
            manager.GoldAmountText.text = WorldManager.instance.GoldAmount.ToString();
        }
    }
}
