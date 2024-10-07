using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CareManager : MonoBehaviour
{
    // UI elements needed
    public GameObject CreaturePopupPanel; // Assign the popup panel in the Inspector
    public GameObject HelpPopupPanel; // Assign the popup panel in the Inspector
    public GameObject[] HelpListPages; // Assign the list of pages in the Inspector
    public GameObject ShopPopupPanel; // Assign the popup panel in the Inspector
    public GameObject ToyShopPanel; // Assign the panel in the Inspector
    public GameObject FoodShopPanel; // Assign the panel in the Inspector
    public GameObject EggsShopPanel; // Assign the panel in the Inspector
    public Transform ToyShopInventory; // Assign the inventory object in the Inspector
    public Transform FoodShopInventory; // Assign the inventory object in the Inspector
    public Transform EggsShopInventory; // Assign the inventory object in the Inspector
    public GameObject ItemPopupPanel; // Assign the popup panel in the Inspector
    public GameObject FoodPopupPanel; // Assign the popup panel in the Inspector
    public Transform FoodInventory; // Assign the inventory object in the Inspector
    public GameObject PlayPopupPanel; // Assign the popup panel in the Inspector
    public Transform PlayInventory; // Assign the inventory object in the Inspector
    public Image CreatureImage; // Assign the Sprite component for the creature image in the Inspector.
    public Text CreatureNameText; // Assign the Text component for the name
    public Text CreatureInfoText; // Assign the Text component for additional info
    public Image ItemImage; // Assign the Sprite component for the item image in the Inspector.
    public Text ItemNameText; // Assign the Text component for the name
    public Text GoldAmountText; // Assign the Text component for the gold amount;
    public Text ItemInfoText; // Assign the Text component for additional info
    public LineRenderer LineRenderer; // Assign the LineRenderer in the Inspector
    public RectTransform linePosition; // Assign the position where the line renderer attaches to the creature panel in the Inspector.
    public Camera UiCamera; // Assign the camera rendering the UI
    public GameObject UiItemPrefab;
    public GameObject UiShopItemPrefab;
    public Vector2 grabOffset = new Vector2(-15, 5);

    public GameObject BoredPrefab;
    public GameObject HungryPrefab;
    public GameObject DirtyPrefab;
    public GameObject bubbleParticles;

    public Texture2D mainCursor;
    public Texture2D clickCursor;
    public Texture2D spongeCursor;
    internal Vector2 hotSpot = new Vector2(5, 5);
    internal CursorMode cursorMode = CursorMode.Auto;

    internal float minXRange = -3f; // min x position inside the fence
    internal float minYRange = -3.5f; // min y position inside the fence
    internal float maxXRange = 3f; // max x position inside the fence
    internal float maxYRange = 3.5f; // max y position inside the fence

    internal bool isUsingScrubBrush = false;

    // List of all tiny creatures you own
    internal List<GameObject> CreaturesOwned = new List<GameObject>();
    internal List<GameObject> FoodOwned = new List<GameObject>();
    internal List<GameObject> ToysOwned = new List<GameObject>();

    internal Dictionary<int, GameObject> UiToGameObjectDictionary = new Dictionary<int, GameObject>();
    internal Dictionary<int, GameObject> UiToUiGameObjectDictionary = new Dictionary<int, GameObject>();
    internal GameObject DraggingItem;

    private int currentHelpPage = 0;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(mainCursor, hotSpot, cursorMode);
        GoldAmountText.text = WorldManager.instance.GoldAmount.ToString();
        var creaturesOwned = WorldManager.instance.activeCreatureStats.ToArray();
        for (int i = 0; i < creaturesOwned.Length; i++)
        {
            var currentCreatureStats = creaturesOwned[i];
            var creatureStats = WorldManager.instance.CreatureBases.First(x => x.CreatureName == currentCreatureStats.name);
            var creatureInstance = Instantiate(creatureStats.CreaturePrefab, GetNextRandomPosition(), Quaternion.identity);
            var creature = creatureInstance.GetComponent<Creature>();
            creature.SetStats(currentCreatureStats);
            CreaturesOwned.Add(creatureInstance);
        }

        var allPossibleEggs = WorldManager.instance.EggBases.ToArray();
        for (int i = 0; i < allPossibleEggs.Count(); i++)
        {
            var currentEgg = allPossibleEggs[i];
            var go = Instantiate(UiShopItemPrefab, EggsShopInventory);
            var shopItem = go.GetComponent<ShopItem>();
            shopItem.ItemImage.sprite = currentEgg.ShopImage;
            shopItem.GoldAmount = currentEgg.Cost;
            shopItem.ItemName = currentEgg.name;
            shopItem.ItemType = ItemType.Egg;
            shopItem.ItemDescription = $"Type: {Enum.GetName(typeof(CreatureType), currentEgg.CreatureType)}\n" +
                                       $"The egg will spawn in your tiny creature area.";
        }

        var allPossibleFoods = WorldManager.instance.FoodBases.ToArray();
        for (int i = 0; i < allPossibleFoods.Count(); i++)
        {
            var currentFood = allPossibleFoods[i];
            var go = Instantiate(UiShopItemPrefab, FoodShopInventory);
            var shopItem = go.GetComponent<ShopItem>();
            shopItem.ItemImage.sprite = currentFood.ShopImage;
            shopItem.GoldAmount = currentFood.Cost;
            shopItem.ItemName = currentFood.name;
            shopItem.ItemType = ItemType.Food;
            shopItem.ItemDescription = $"Food: {currentFood.HungerRestore}\n" +
                                       $"Uses: {currentFood.NumberOfUses}\n" +
                                       $"{currentFood.Description}";
        }

        var allPossibleToys = WorldManager.instance.ToyBases.ToArray();
        for (int i = 0; i < allPossibleToys.Count(); i++)
        {
            var currentToy = allPossibleToys[i];
            var go = Instantiate(UiShopItemPrefab, ToyShopInventory);
            var shopItem = go.GetComponent<ShopItem>();
            shopItem.ItemImage.sprite = currentToy.ShopImage;
            shopItem.GoldAmount = currentToy.Cost;
            shopItem.ItemName = currentToy.name;
            shopItem.ItemType = ItemType.Toy;
            shopItem.ItemDescription = $"Entertainment: {currentToy.EntertainmentRestore}\n" +
                                       $"Uses: {currentToy.NumberOfUses}\n" +
                                       $"{currentToy.Description}";
        }


        var foodOwned = WorldManager.instance.foodInventory.ToArray();
        for (int i = 0; i < foodOwned.Length; i++)
        {
            var currentFood = foodOwned[i];
            var foodStats = WorldManager.instance.FoodBases.First(x => x.name == currentFood.name);
            var go = Instantiate(foodStats.FoodPrefab);
            go.SetActive(false);
            FoodOwned.Add(go);
        }

        var toysOwned = WorldManager.instance.toysInventory.ToArray();
        for (int i = 0; i < toysOwned.Length; i++)
        {
            var currentToy = toysOwned[i];
            var toyStats = WorldManager.instance.ToyBases.First(x => x.name == currentToy.name);
            var go = Instantiate(toyStats.ToyPrefab);
            go.SetActive(false);
            ToysOwned.Add(go);
        }

        SetupInventoryImages();

        CalculateEggSpawns();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 0 is for the left mouse button
        {
            if (isUsingScrubBrush)
            {

            }
            else
            {
                if (!IsPointerOverUIElement())
                {
                    Cursor.SetCursor(clickCursor, hotSpot, cursorMode);
                }
            }

        }
        if (Input.GetMouseButtonUp(0)) // 0 is for the left mouse button
        {
            if (isUsingScrubBrush)
            {

            }
            else
            {
                if (!IsPointerOverUIElement())
                {
                    Cursor.SetCursor(mainCursor, hotSpot, cursorMode);
                }
            }
        }

        if (DraggingItem != null)
        {
            var mousePosition = Input.mousePosition;
            mousePosition.x += grabOffset.x;
            mousePosition.y += grabOffset.y;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            worldPosition.z = 0f;
            DraggingItem.transform.position = worldPosition;
        }
    }

    public void SetupInventoryImages()
    {
        // Destroy all existing data to rebuild it.
        var array = UiToGameObjectDictionary.ToArray();
        for (int i = 0; i < array.Length; i++)
        {
            UiToGameObjectDictionary.Remove(array[i].Key);
            Destroy(UiToUiGameObjectDictionary[array[i].Key]);
            UiToUiGameObjectDictionary.Remove(array[i].Key);
        }

        foreach (var food in FoodOwned)
        {
            var foodUiInstance = Instantiate(UiItemPrefab, FoodInventory);
            var uiItem = foodUiInstance.GetComponent<UiItemClick>();
            var worldItem = food.GetComponent<Food>();
            uiItem.GetComponent<Image>().sprite = worldItem.GetComponent<SpriteRenderer>().sprite;
            uiItem.ItemName = worldItem.Name;
            uiItem.ItemDescription = $"Food: {worldItem.HungerRestore}\n" +
                                     $"Uses: {worldItem.NumberOfUses}\n" +
                                     $"{worldItem.Description}";
            UiToGameObjectDictionary.Add(foodUiInstance.GetInstanceID(), food);
            UiToUiGameObjectDictionary.Add(foodUiInstance.GetInstanceID(), foodUiInstance);
        }
        foreach (var toy in ToysOwned)
        {
            var toyUiInstance = Instantiate(UiItemPrefab, PlayInventory);
            var uiItem = toyUiInstance.GetComponent<UiItemClick>();
            var worldItem = toy.GetComponent<Toy>();
            uiItem.GetComponent<Image>().sprite = worldItem.GetComponent<SpriteRenderer>().sprite;
            uiItem.ItemName = worldItem.Name;
            uiItem.ItemDescription = $"Entertainment: {worldItem.EntertainmentRestore}\n" +
                                     $"Uses: {worldItem.NumberOfUses}\n" +
                                     $"{worldItem.Description}";
            UiToGameObjectDictionary.Add(toyUiInstance.GetInstanceID(), toy);
            UiToUiGameObjectDictionary.Add(toyUiInstance.GetInstanceID(), toyUiInstance);
        }
    }

    public void CloseCreatureInfoPanel()
    {
        if(!isUsingScrubBrush)
        {
            Cursor.SetCursor(mainCursor, hotSpot, cursorMode);
            CreaturePopupPanel.SetActive(false);
            LineRenderer.enabled = false;
        }        
    }

    public void ToggleFoodMenu()
    {
        FoodPopupPanel.SetActive(!FoodPopupPanel.activeSelf);
    }

    public void TogglePlayMenu()
    {
        PlayPopupPanel.SetActive(!PlayPopupPanel.activeSelf);
    }

    public void ToggleShopMenu()
    {
        ShopPopupPanel.SetActive(!ShopPopupPanel.activeSelf);
    }

    public void ToggleScrubBrush()
    {
        isUsingScrubBrush = !isUsingScrubBrush;
        if (isUsingScrubBrush)
        {
            UseScrubBrush();
        }
        else
        {
            CancelScrubBrush();
        }
    }

    public void UseScrubBrush()
    {
        isUsingScrubBrush = true;
        Cursor.SetCursor(spongeCursor, hotSpot, cursorMode);
    }

    public void CancelScrubBrush()
    {
        isUsingScrubBrush = false;
        SetMainCursor();
    }

    public void CloseFoodMenu()
    {
        FoodPopupPanel.SetActive(false);
    }

    public void SelectToyShop()
    {
        ToyShopPanel.SetActive(true);
        FoodShopPanel.SetActive(false);
        EggsShopPanel.SetActive(false);
    }

    public void SelectFoodShop()
    {
        ToyShopPanel.SetActive(false);
        FoodShopPanel.SetActive(true);
        EggsShopPanel.SetActive(false);
    }
    public void SelectEggsShop()
    {
        ToyShopPanel.SetActive(false);
        FoodShopPanel.SetActive(false);
        EggsShopPanel.SetActive(true);
    }

    public void SetMainCursor()
    {
        if (DraggingItem == null && !isUsingScrubBrush)
        {
            Cursor.SetCursor(mainCursor, hotSpot, cursorMode);
        }
    }

    public void SetClickCursor()
    {
        if (!isUsingScrubBrush)
        {
            Cursor.SetCursor(clickCursor, hotSpot, cursorMode);
        }
    }

    public void ClosePlayMenu()
    {
        PlayPopupPanel.SetActive(false);
    }

    public void CloseShopMenu()
    {
        ShopPopupPanel.SetActive(false);
    }

    public void EnterBattle()
    {
        CancelScrubBrush();
        SceneManager.LoadScene(2);
        SaveCreatureValues();
        SaveItemValues();
    }

    public void ToggleHelpPanel()
    {
        HelpPopupPanel.SetActive(!HelpPopupPanel.activeSelf);
    }

    public void CloseHelpPanel()
    {
        HelpPopupPanel.SetActive(false);
    }

    public void NextHelpPage()
    {
        HelpListPages[currentHelpPage].SetActive(false);
        currentHelpPage++;
        if(currentHelpPage >= HelpListPages.Length)
        {
            currentHelpPage = 0;
        }
        HelpListPages[currentHelpPage].SetActive(true);
    }

    public void PreviousHelpPage()
    {
        HelpListPages[currentHelpPage].SetActive(false);
        currentHelpPage--;
        if (currentHelpPage < 0)
        {
            currentHelpPage = HelpListPages.Length - 1;
        }
        HelpListPages[currentHelpPage].SetActive(true);
    }

    internal void UpdateCreatureInfo(Creature creature)
    {
        var currentCreatureSelected = FindObjectOfType<CreatureClickHandler>().currentCreature;
        if (currentCreatureSelected != null && currentCreatureSelected.gameObject.GetInstanceID() == creature.gameObject.GetInstanceID())
        {
            CreatureInfoText.text = $"Type: {creature.Type}\n" +
                            $"HP: {creature.HitPoints}/{creature.MaxHitPoints}\n" +
                            $"Attack: {creature.Attack}\n" +
                            $"Defence: {creature.Defence}\n" +
                            $"Exhausted: {creature.IsExhausted}\n" +
                            $"Hunger: {creature.Hunger}/{creature.MaxHunger}\n" +
                            $"Entertainment: {creature.Entertainment}/{creature.MaxEntertainment}\n" +
                            $"Hygiene: {creature.Hygiene}/{creature.MaxHygiene}";
        }
    }

    /// <summary>
    /// Generates a random position within the range.
    /// </summary>
    /// <returns>A Vector3 representing the new position for the egg to be spawned at.</returns>
    internal Vector3 GetNextRandomPosition()
    {
        var randomX = UnityEngine.Random.Range(minXRange, maxXRange);
        var randomY = UnityEngine.Random.Range(minYRange, maxYRange);
        return new Vector3(randomX, randomY, 0);
    }

    internal bool IsDroppedWithinFence()
    {
        var mousePosition = Input.mousePosition;
        mousePosition.x += grabOffset.x;
        mousePosition.y += grabOffset.y;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        return worldPosition.x < maxXRange
            && worldPosition.x > -maxXRange
            && worldPosition.y < maxYRange
            && worldPosition.y > -maxYRange;
    }

    private void SaveCreatureValues()
    {
        // Reset the list so we dont have to check for what was already existing.
        WorldManager.instance.activeCreatureStats = new List<ActiveCreatureStats>();
        foreach (var creatureGo in CreaturesOwned)
        {
            var creature = creatureGo.GetComponent<Creature>();
            WorldManager.instance.activeCreatureStats.Add(new ActiveCreatureStats()
            {
                name = creature.Name,
                health = creature.HitPoints,
                maxHealth = creature.MaxHitPoints,
                damage = creature.Attack,
                block = creature.Defence,
                hunger = creature.Hunger,
                entertainment = creature.Entertainment,
                hygene = creature.Hygiene,
                exhausted = creature.IsExhausted,
                myType = creature.CreatureType
            });
        }
    }

    private void SaveItemValues()
    {
        foreach(var food in FoodOwned)
        {
            WorldManager.instance.foodInventory.Add(new ActiveItemStats()
            {
                name = food.GetComponent<Food>().Name,
            });
        }

        foreach (var toy in ToysOwned)
        {
            WorldManager.instance.foodInventory.Add(new ActiveItemStats()
            {
                name = toy.GetComponent<Toy>().Name,
            });
        }
    }

    private bool IsPointerOverUIElement()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void CalculateEggSpawns()
    {
        var groupedCreatureTypes = CreaturesOwned.Select(x => x.GetComponent<Creature>()).GroupBy(x => x.Type);
        foreach(var creatureGroup in groupedCreatureTypes) 
        {
            var groupType = creatureGroup.Key;
            var pairCount = creatureGroup.Count() / 2;

            // Calculate average hygiene, hunger, and entertainment values for the group
            float averageHygiene = (float)creatureGroup.Average(x => x.Hygiene);
            float averageHunger = (float)creatureGroup.Average(x => x.Hunger);
            float averageEntertainment = (float)creatureGroup.Average(x => x.Entertainment);
            // Calculate a multiplier based on the average values
            float averageValue = (averageHygiene + averageHunger + averageEntertainment) / 3.0f;
            float multiplier = averageValue / 100.0f; // Assuming values are between 0 and 100
            int randomVariation;
            if (averageValue > 66.6f) // High average
            {
                randomVariation = UnityEngine.Random.Range(0, 2); // More likely to be +1
            }
            else if (averageValue < 33.3f) // Low average
            {
                randomVariation = UnityEngine.Random.Range(-1, 1); // More likely to be -1
            }
            else // Around 50%
            {
                randomVariation = 0;
            }

            var eggAmount = Mathf.Max(0, (int)((pairCount + randomVariation) * multiplier)); // Ensure eggAmount is not negative
            for (int i = 0; i < eggAmount; i++)
            {
                SpawnEgg(groupType);
            }
        }
    }

    private void SpawnEgg(string type)
    {
        var eggScriptableObject = WorldManager.instance.EggBases.First(x => x.name == type+"Egg");
        Instantiate(eggScriptableObject.EggPrefab, GetNextRandomPosition(), Quaternion.identity);
    }

    private GameObject FindObjectByInstanceID(int instanceID)
    {
        UnityEngine.Object obj = EditorUtility.InstanceIDToObject(instanceID);
        if (obj is GameObject)
        {
            return (GameObject)obj;
        }
        return null;
    }
}
