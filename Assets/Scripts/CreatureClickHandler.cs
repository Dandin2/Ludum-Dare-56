using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CreatureClickHandler : MonoBehaviour
{
    internal Creature currentCreature;
    private SpriteRenderer currentCreatureSpriteRenderer;
    private CareManager manager;
    private float lastTimeSpawned;

    private void Start()
    {
        manager = FindObjectOfType<CareManager>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero);

            if (hit.collider != null)
            {
                Creature creature = hit.collider.GetComponent<Creature>();
                if (creature != null)
                {
                    currentCreature = creature;
                    currentCreatureSpriteRenderer = creature.GetComponent<SpriteRenderer>();
                    ShowPopup(creature);
                    if(manager.isUsingScrubBrush && creature.Hygiene != creature.MaxHygiene && creature.gameObject.GetInstanceID() == gameObject.GetInstanceID())
                    {
                        if(Time.time > lastTimeSpawned + .5f)
                        {
                            lastTimeSpawned = Time.time;
                            var bubbles = Instantiate(manager.bubbleParticles, creature.transform);
                            Destroy(bubbles, .5f);
                        }
                        creature.partialHygiene += .5f;
                        if(creature.partialHygiene >= 1)
                        {
                            creature.Hygiene += 1;
                            creature.partialHygiene -= 1;
                        }
                        if (creature.Hygiene > creature.MaxHygiene)
                        {
                            creature.Hygiene = creature.MaxHygiene;
                        }
                        manager.UpdateCreatureInfo(creature);
                    }
                }
            }
        }

        if (currentCreature != null)
        {
            UpdateLineRenderer();
        }
    }

    void ShowPopup(Creature creature)
    {
        manager.CreatureImage.sprite = currentCreatureSpriteRenderer.sprite;
        manager.CreatureNameText.text = creature.Name;
        manager.CreatureInfoText.text = $"Type: {creature.Type}\n" +
                        $"HP: {creature.HitPoints}/{creature.MaxHitPoints}\n" +
                        $"Attack: {creature.Attack}\n" +
                        $"Defence: {creature.Defence}\n" +
                        $"Exhausted: {creature.IsExhausted}\n" +
                        $"Hunger: {creature.Hunger}/{creature.MaxHunger}\n" +
                        $"Entertainment: {creature.Entertainment}/{creature.MaxEntertainment}\n" +
                        $"Hygiene: {creature.Hygiene}/{creature.MaxHygiene}";
        manager.CreaturePopupPanel.SetActive(true);
        manager.LineRenderer.enabled = true;
    }

    void UpdateLineRenderer()
    {
        manager.CreatureImage.sprite = currentCreatureSpriteRenderer.sprite;
        Vector3 creaturePosition = currentCreature.transform.position;
        manager.LineRenderer.SetPosition(0, creaturePosition);
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(manager.linePosition, manager.linePosition.position, manager.UiCamera, out var worldPoint))
        {
            manager.LineRenderer.positionCount = 40;
            for (int i = 1; i < manager.LineRenderer.positionCount - 1; i++)
            {
                manager.LineRenderer.SetPosition(i, Vector2.Lerp(creaturePosition, worldPoint, (float)i/manager.LineRenderer.positionCount));
            }
            manager.LineRenderer.SetPosition(manager.LineRenderer.positionCount - 1, worldPoint);
        }

    }
}
