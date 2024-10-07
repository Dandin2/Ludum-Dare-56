using System;
using System.Linq;
using UnityEngine;

public class Toy : MonoBehaviour
{
    public string Name;

    internal int EntertainmentRestore;

    internal int NumberOfUses;

    internal float AttractionRadius;

    internal string Description;

    private CareManager manager;


    void Awake()
    {
        var startingStats = WorldManager.instance.ToyBases.First(x => x.name == Name);
        EntertainmentRestore = startingStats.EntertainmentRestore;
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
            if (creature.ToyUsing?.gameObject.GetInstanceID() == gameObject.GetInstanceID())
            {
                creature.useItemAvailable = false;
                creature.closeEnoughToItem = false;
                creature.ToyUsing = null;
            }
        }
    }

    private void AttractCreaturesWithinRadius()
    {
        // Loop through and have any creature within the radius of this item use it.
        foreach (var creatureGo in manager.CreaturesOwned)
        {
            if(Vector3.Distance(creatureGo.transform.position, transform.position) < AttractionRadius)
            {
                var creature = creatureGo.GetComponent<Creature>();
                if (creature.ToyUsing == null && creature.Entertainment < creature.MaxEntertainment)
                {
                    creature.ToyUsing = this;
                    creature.useItemAvailable = true;
                    creature.StopMovement();
                    creature.targetPosition = transform.position;
                }
                else if (creature.ToyUsing != null && creature.useItemAvailable && NumberOfUses > 0)
                {
                    creature.useItemAvailable = false;
                    creature.closeEnoughToItem = false;
                    NumberOfUses--;
                    creature.Entertainment += EntertainmentRestore;
                    if (creature.Entertainment >= creature.MaxEntertainment)
                    {
                        creature.Entertainment = creature.MaxEntertainment;
                        creature.ToyUsing = null;
                    }
                    manager.UpdateCreatureInfo(creature);
                }
            }
        }
    }
}
