using System;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public string Name;

    public string Type;

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

    internal float Speed;

    private Rigidbody2D rigidBody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float nextMoveTime = 0; // tracks when the next move will be made.
    private float nextMoveTimeIncrement = 15; // time in seconds to make a new move
    private float nextMoveTimeIncrementAdjust = 5; // adjusts next move time so not all creatures move in the same increment.
    private Vector3? targetPosition; // the target position the creature is trying to get to.
    private float maxXRange = 3.18f; // max x position of the random generated position
    private float maxYRange = 3.78f; // max y position of the random generated position

    // Start is called before the first frame update
    void Start()
    {
        var startingStats = ScriptableObjectFinder.FindScriptableObjectByName<CreatureStats>(Name);
        Type = Enum.GetName(typeof(Type), startingStats.CreatureType);
        HitPoints = startingStats.HitPoints;
        MaxHitPoints = startingStats.HitPoints; 
        Defence = startingStats.Defence;
        Attack = startingStats.Attack;
        Hunger = startingStats.Hunger;
        MaxHunger = startingStats.Hunger;
        Entertainment = startingStats.Entertainment;
        MaxEntertainment = startingStats.Entertainment;
        Hygiene = startingStats.Hygiene;
        MaxHygiene = startingStats.Hygiene;
        Speed = startingStats.Speed;
        transform.position = GetNextRandomPosition();
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        nextMoveTime = nextMoveTimeIncrementAdjust * 2 - nextMoveTimeIncrement;
        nextMoveTimeIncrement = UnityEngine.Random.Range(nextMoveTimeIncrement - nextMoveTimeIncrementAdjust, nextMoveTimeIncrement + nextMoveTimeIncrementAdjust);

        // TODO: Check if hunger is low enough to spawn the hunger effect around creature.
        // TODO: Check if entertainment is low enough to spawn the bordom effect around creature.
        // TODO: Check if hygiene is low enough to spawn the dirty effect around creature.
    }

    // Update is called once per frame
    void Update()
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

        if (targetPosition.HasValue && Vector3.Distance(transform.position, targetPosition.Value) < 0.1f && rigidBody.velocity != Vector2.zero)
        {
            StopMovement();
        }
    }

    public void ScrubCreature()
    {
        Hygiene += 10;
        // TODO: spwan soap suds
        // TODO: check for hygiene past threshhold to despawn dirty effect and destroy it if so.
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
    private void StopMovement()
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
        var randomX = UnityEngine.Random.Range(-maxXRange, maxXRange);
        var randomY = UnityEngine.Random.Range(-maxYRange, maxYRange);
        return new Vector3(randomX, randomY, 0);
    }
}
