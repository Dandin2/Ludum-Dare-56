using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

    // List all possible creatures prefabs
    public List<GameObject> allPosibleCreatures;
    // List of all tiny creatures you own

    // Start is called before the first frame update
    void Start()
    {
        // TODO: Instantiate creatures that the game state has saved.
        for(int i = 0; i< 10; i++)
        {
            var randomCreatureIndex = Random.Range(0, allPosibleCreatures.Count);
            Instantiate(allPosibleCreatures.ElementAt(randomCreatureIndex));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseCreatureInfoPanel()
    {
        CreaturePopupPanel.SetActive(false);
        LineRenderer.enabled = false;
    }
}
