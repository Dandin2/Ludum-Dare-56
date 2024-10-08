using System;
using System.Linq;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public string Name;
    public float ThoughtTime = 5f; // How long a thought is shown for. should probably be less than the min thought time incrament.

    // threshholds for a creature caring about each stat being low
    public float HungerThreshold = .5f;
    public float EntertainmentThreshold = .5f;
    public float HygieneThreshold = .5f;

    internal string Type;
    internal CreatureType CreatureType;

    internal int HitPoints;
    internal int MaxHitPoints;

    internal int Defence;

    internal int Attack;

    internal int Hunger;
    internal int MaxHunger;

    internal int Entertainment;
    internal int MaxEntertainment;

    internal int Hygiene;
    internal int MaxHygiene;
    internal float partialHygiene;

    internal bool IsExhausted;

    internal float Speed;

    internal Vector3? targetPosition; // the target position the creature is trying to get to.
    internal Toy ToyUsing;
    internal Food FoodUsing;
    internal bool useItemAvailable;
    internal bool closeEnoughToItem;

    internal bool isInitialized = false;

    private Rigidbody2D rigidBody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float nextMoveTime = 0; // tracks when the next move will be made.
    private float nextMoveTimeIncrement = 5; // time in seconds to make a new move
    private float nextMoveTimeIncrementAdjust = 5; // adjusts next move time so not all creatures move in the same increment.
    private CareManager manager;

    private float nextThoughtTime = 0;
    private float nextMinThoughtTimeIncrement = 10; // min time in seconds to make a new thought
    private float nextMaxThoughtTimeIncrement = 50; // max time in seconds to make a new thought

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<CareManager>();
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        nextMoveTime = Time.time + nextMoveTimeIncrementAdjust;
        nextMoveTimeIncrement = UnityEngine.Random.Range(nextMoveTimeIncrement - nextMoveTimeIncrementAdjust, nextMoveTimeIncrement + nextMoveTimeIncrementAdjust);

        nextThoughtTime += UnityEngine.Random.Range(0, nextMaxThoughtTimeIncrement);
        isInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (ToyUsing != null || FoodUsing != null)
        {
            if (Time.time >= nextMoveTime && closeEnoughToItem)
            {
                nextMoveTime += nextMoveTimeIncrement;
                useItemAvailable = true;
            }

            if (targetPosition.HasValue && rigidBody.velocity == Vector2.zero)
            {
                StartMovement();
            }

            if (targetPosition.HasValue && Vector3.Distance(transform.position, targetPosition.Value) < 0.2f && rigidBody.velocity != Vector2.zero)
            {
                StopMovement();
                useItemAvailable = true;
                closeEnoughToItem = true;
            }
        }
        else
        {
            if (Time.time >= nextMoveTime)
            {
                nextMoveTime += nextMoveTimeIncrement;
                targetPosition = GetNextRandomPosition();
            }

            if (targetPosition.HasValue && rigidBody.velocity == Vector2.zero)
            {
                StartMovement();
            }

            if (targetPosition.HasValue && Vector3.Distance(transform.position, targetPosition.Value) < 0.2f && rigidBody.velocity != Vector2.zero)
            {
                StopMovement();
            }
        }

        if (Time.time >= nextThoughtTime)
        {
            nextThoughtTime += UnityEngine.Random.Range(nextMinThoughtTimeIncrement, nextMaxThoughtTimeIncrement);
            ShowNextThought();
        }        
    }

    public void SetStats(ActiveCreatureStats currentCreatureStats)
    {
        var startingStats = WorldManager.instance.CreatureBases.First(x => x.CreatureName == Name);
        if (currentCreatureStats == null)
        {
            CreatureType = startingStats.CreatureType;
            Type = Enum.GetName(typeof(CreatureType), startingStats.CreatureType);
            HitPoints = startingStats.HitPoints;
            MaxHitPoints = startingStats.HitPoints;
            Defence = startingStats.Defence;
            Attack = startingStats.Attack;
            Hunger = startingStats.Hunger - UnityEngine.Random.Range(0, startingStats.Hunger);
            MaxHunger = startingStats.Hunger;
            Entertainment = startingStats.Entertainment - UnityEngine.Random.Range(0, startingStats.Entertainment);
            MaxEntertainment = startingStats.Entertainment;
            Hygiene = startingStats.Hygiene - UnityEngine.Random.Range(0, startingStats.Hygiene);
            MaxHygiene = startingStats.Hygiene;
            Speed = startingStats.Speed;
        }
        else
        {
            CreatureType = currentCreatureStats.myType;
            Type = Enum.GetName(typeof(CreatureType), currentCreatureStats.myType);
            HitPoints = currentCreatureStats.health;
            MaxHitPoints = startingStats.HitPoints;
            Defence = currentCreatureStats.block;
            Attack = currentCreatureStats.damage;
            Hunger = currentCreatureStats.hunger;
            MaxHunger = startingStats.Hunger;
            Entertainment = currentCreatureStats.entertainment;
            MaxEntertainment = startingStats.Entertainment;
            Hygiene = currentCreatureStats.hygene;
            MaxHygiene = startingStats.Hygiene;
            Speed = startingStats.Speed;
        }
    }

    /// <summary>
    /// Updates the variables to properly make the creature move.
    /// </summary>
    private void StartMovement()
    {
        animator.SetBool("isWalking", true);
        var direction = targetPosition.Value - transform.position;
        rigidBody.AddRelativeForce(direction.normalized * Speed, ForceMode2D.Impulse);

        if (rigidBody.velocity.x >= 0)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }

    /// <summary>
    /// Updates all the variables to properly make the creature stop moving.
    /// </summary>
    internal void StopMovement()
    {
        animator.SetBool("isWalking", false);
        rigidBody.velocity = Vector2.zero;
        targetPosition = null;
        spriteRenderer.flipX = false;
    }

    /// <summary>
    /// Generates a random position within the range.
    /// </summary>
    /// <returns>A Vector3 representing the new position for the creature to move towards.</returns>
    private Vector3 GetNextRandomPosition()
    {
        var randomX = UnityEngine.Random.Range(-manager.maxXRange, manager.maxXRange);
        var randomY = UnityEngine.Random.Range(-manager.maxYRange, manager.maxYRange);
        return new Vector3(randomX, randomY, 0);
    }

    private void ShowNextThought()
    {
        var hungerPercent = Hunger / (float)MaxHunger;
        var hygienePercent = Hygiene / (float)MaxHygiene;
        var entertainmentPercent = Entertainment / (float)MaxEntertainment;

        // Find the lowest percentage
        float lowestPercent = Mathf.Min(hungerPercent, hygienePercent, entertainmentPercent);

        // Show the corresponding thought
        if (lowestPercent == hungerPercent && hungerPercent < HungerThreshold)
        {
            Destroy(Instantiate(manager.HungryPrefab, transform), ThoughtTime);
        }
        else if (lowestPercent == hygienePercent && hygienePercent < HygieneThreshold)
        {
            Destroy(Instantiate(manager.DirtyPrefab, transform), ThoughtTime);
        }
        else if (lowestPercent == entertainmentPercent && entertainmentPercent < EntertainmentThreshold)
        {
            Destroy(Instantiate(manager.BoredPrefab, transform), ThoughtTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StopMovement();
    }
}
