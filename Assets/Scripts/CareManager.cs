using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CareManager : MonoBehaviour
{
    // UI elements needed
    public GameObject CreaturePopupPanel; // Assign the popup panel in the Inspector
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
    public Text ItemInfoText; // Assign the Text component for additional info
    public LineRenderer LineRenderer; // Assign the LineRenderer in the Inspector
    public RectTransform linePosition; // Assign the position where the line renderer attaches to the creature panel in the Inspector.
    public Camera UiCamera; // Assign the camera rendering the UI
    public GameObject UiItemPrefab;
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

    internal float maxXRange = 3f; // max x position inside the fence
    internal float maxYRange = 3.5f; // max y position inside the fence

    internal bool isUsingScrubBrush = false;

    // List of all tiny creatures you own
    internal List<GameObject> CreaturesOwned = new List<GameObject>();
    internal List<GameObject> FoodOwned = new List<GameObject>();
    internal List<GameObject> ToysOwned = new List<GameObject>();

    internal Dictionary<int, GameObject> UiToGameObjectDictionary = new Dictionary<int, GameObject>();
    internal GameObject DraggingItem;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(mainCursor, hotSpot, cursorMode);

        // TODO: Instantiate creatures that the game state has saved.
        var allPossibleCreatureStats = ScriptableObjectFinder.GetAllCreatureStats();
        for (int i = 0; i < 10; i++)
        {
            var randomCreatureIndex = UnityEngine.Random.Range(0, allPossibleCreatureStats.Count);
            var creatureInstance = Instantiate(allPossibleCreatureStats.ElementAt(randomCreatureIndex).CreaturePrefab, GetNextRandomPosition(), Quaternion.identity);
            CreaturesOwned.Add(creatureInstance);
        }

        // TODO: Instatntiate the food you actually have instead of the mock inventory I am loading for testing.
        var allPossibleFoods = ScriptableObjectFinder.GetAllFoodStats();
        for (int i = 0; i < allPossibleFoods.Count(); i++)
        {
            var foodInstance = Instantiate(allPossibleFoods.ElementAt(i).FoodPrefab);
            foodInstance.SetActive(false);
            FoodOwned.Add(foodInstance);
            foodInstance = Instantiate(allPossibleFoods.ElementAt(i).FoodPrefab);
            foodInstance.SetActive(false);
            FoodOwned.Add(foodInstance);
        }
        // TODO: Instatntiate the toys you actually have instead of the mock inventory I am loading for testing.
        var allPossibleToys = ScriptableObjectFinder.GetAllToyStats();
        for (int i = 0; i < allPossibleToys.Count(); i++)
        {
            var toyInstance = Instantiate(allPossibleToys.ElementAt(i).ToyPrefab);
            toyInstance.SetActive(false);
            ToysOwned.Add(toyInstance);
            toyInstance = Instantiate(allPossibleToys.ElementAt(i).ToyPrefab);
            toyInstance.SetActive(false);
            ToysOwned.Add(toyInstance);
        }
        SetupInventoryImages();

        // TODO: Calculate Eggs Spawned based on all creatures and their current stats.
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

    internal void UpdateCreatureInfo(Creature creature)
    {
        var currentCreatureSelected = FindObjectOfType<CreatureClickHandler>().currentCreature;
        if (currentCreatureSelected != null && currentCreatureSelected.gameObject.GetInstanceID() == creature.gameObject.GetInstanceID())
        {
            CreatureInfoText.text = $"Type: {creature.Type}\n" +
                            $"HP: {creature.HitPoints}/{creature.MaxHitPoints}\n" +
                            $"Attack: {creature.Attack}\n" +
                            $"Defence: {creature.Defence}\n" +
                            $"Hunger: {creature.Hunger}/{creature.MaxHunger}\n" +
                            $"Entertainment: {creature.Entertainment}/{creature.MaxEntertainment}\n" +
                            $"Hygiene: {creature.Hygiene}/{creature.MaxHygiene}";
        }
    }

    internal bool IsDroppedWithinFence()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return worldPosition.x < maxXRange
            && worldPosition.x > -maxXRange
            && worldPosition.y < maxYRange
            && worldPosition.y > -maxYRange;
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
        var eggScriptableObject = ScriptableObjectFinder.FindScriptableObjectByName<EggStats>(type+"Egg");
        Instantiate(eggScriptableObject.EggPrefab, GetNextRandomPosition(), Quaternion.identity);
    }


    /// <summary>
    /// Generates a random position within the range.
    /// </summary>
    /// <returns>A Vector3 representing the new position for the egg to be spawned at.</returns>
    private Vector3 GetNextRandomPosition()
    {
        var randomX = UnityEngine.Random.Range(-maxXRange, maxXRange);
        var randomY = UnityEngine.Random.Range(-maxYRange, maxYRange);
        return new Vector3(randomX, randomY, 0);
    }
}
