using UnityEngine;

public class Food : MonoBehaviour
{
    public string Name;

    internal int HungerRestore;

    internal float AttractionRadius;

    public int NumberOfUses;

    internal string Description;

    private CareManager manager;


    void Awake()
    {
        var startingStats = ScriptableObjectFinder.FindScriptableObjectByName<FoodStats>(Name);
        HungerRestore = startingStats.HungerRestore;
        AttractionRadius = startingStats.AttractionRadius;
        NumberOfUses = startingStats.NumberOfUses;
        Description = startingStats.Description;
        GetComponent<EffectCircleRenderer>().Radius = AttractionRadius;
    }

    private void Start()
    {
        manager = FindObjectOfType<CareManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(NumberOfUses <= 0)
        {
            SetAllCreaturesNotUsingItem();
            Destroy(gameObject);
            return;
        }

        AttractCreaturesWithinRadius();
    }

    private void SetAllCreaturesNotUsingItem()
    {
        foreach (var creatureGo in manager.CreaturesOwned)
        {
            var creature = creatureGo.GetComponent<Creature>();
            if (creature.FoodUsing?.gameObject.GetInstanceID() == gameObject.GetInstanceID())
            {
                creature.FoodUsing = null;
            }
        }
    }

    private void AttractCreaturesWithinRadius()
    {
        // Loop through and have any creature within the radius of this item use it.
        foreach (var creatureGo in manager.CreaturesOwned)
        {
            var d = Vector3.Distance(creatureGo.transform.position, transform.position);
            if (d < AttractionRadius)
            {
                var creature = creatureGo.GetComponent<Creature>();
                if (creature.FoodUsing == null && creature.Hunger < creature.MaxHunger)
                {
                    creature.FoodUsing = this;
                    creature.StopMovement();
                    creature.targetPosition = transform.position;
                }
                else if (creature.FoodUsing != null && creature.useItemAvailable && NumberOfUses > 0)
                {
                    creature.useItemAvailable = false;
                    NumberOfUses--;
                    creature.Hunger += HungerRestore;
                    if (creature.Hunger >= creature.MaxHunger)
                    {
                        creature.Hunger = creature.MaxHunger;
                        creature.FoodUsing = null;
                    }
                    manager.UpdateCreatureInfo(creature);
                }
            }
        }
    }
}
