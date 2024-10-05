using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CareManager : MonoBehaviour
{
    // UI elements needed
    public GameObject CreaturePopupPanel; // Assign the popup panel in the Inspector
    public Image CreatureImage; // Assign the Sprite component for the creature image in the Inspector.
    public Text NameText; // Assign the Text component for the name
    public Text InfoText; // Assign the Text component for additional info
    public LineRenderer LineRenderer; // Assign the LineRenderer in the Inspector
    public RectTransform linePosition; // Assign the position where the line rendere attaches to this panel in the Inspector.
    public Camera UiCamera; // Assign the camera rendering the UI

    public Texture2D mainCursor;
    public Texture2D clickCursor;
    internal Vector2 hotSpot = new Vector2(5, 5);
    internal CursorMode cursorMode = CursorMode.Auto;

    // List of all tiny creatures you own
    internal List<GameObject> CreaturesOwned = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(mainCursor, hotSpot, cursorMode);

        // TODO: Instantiate creatures that the game state has saved.
        var allPossibleCreatureStats = ScriptableObjectFinder.GetAllCreatureStats();
        for(int i = 0; i< 10; i++)
        {
            var randomCreatureIndex = Random.Range(0, allPossibleCreatureStats.Count);
            CreaturesOwned.Add(Instantiate(allPossibleCreatureStats.ElementAt(randomCreatureIndex).CreaturePrefab));
        }

        // TODO: Calculate Eggs Spawned based on all creatures and their current stats.

        // TODO: Instantiate the spawned eggs at random locations around the fenced in area.
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 0 is for the left mouse button
        {
            if (!IsPointerOverUIElement())
            {
                Cursor.SetCursor(clickCursor, hotSpot, cursorMode);
            }
        }
        if (Input.GetMouseButtonUp(0)) // 0 is for the left mouse button
        {
            if (!IsPointerOverUIElement())
            {
                Cursor.SetCursor(mainCursor, hotSpot, cursorMode);
            }
        }
    }

    public void CloseCreatureInfoPanel()
    {
        Cursor.SetCursor(mainCursor, hotSpot, cursorMode);
        CreaturePopupPanel.SetActive(false);
        LineRenderer.enabled = false;
    }

    public void ToggleFoodMenu()
    {

    }

    public void ToggleToyMenu() 
    {

    }

    public void ToggleScrubBrush()
    {

    }
    private bool IsPointerOverUIElement()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
