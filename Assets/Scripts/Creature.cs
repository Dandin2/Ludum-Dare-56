using UnityEngine;

public class Creature : MonoBehaviour
{
    public string Name;

    internal int HitPoints;
    internal int MaxHitPoints;

    internal int Defence;
    internal int MaxDefence;

    internal int Attack;
    internal int MaxAttack;

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
    private float nextMoveTime = 0;
    private float nextMoveTimeIncrement = 15; // time in seconds to make a new move
    private float nextMoveTimeIncrementAdjust = 5;
    private Vector3? targetPosition;
    private float maxXRange = 3.19f;
    private float maxYRange = 3.79f;

    // Start is called before the first frame update
    void Start()
    {
        var startingStats = ScriptableObjectFinder.FindScriptableObjectByName<CreatureStats>(Name);
        HitPoints = startingStats.HitPoints;
        MaxHitPoints = startingStats.HitPoints; 
        Defence = startingStats.Defence;
        MaxDefence = startingStats.Defence;
        Attack = startingStats.Attack;
        MaxAttack = startingStats.Attack;
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
        nextMoveTimeIncrement = Random.Range(nextMoveTimeIncrement - nextMoveTimeIncrementAdjust, nextMoveTimeIncrement + nextMoveTimeIncrementAdjust);
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

        if (targetPosition.HasValue && Vector3.Distance(transform.position, targetPosition.Value) < 0.1f && rigidBody.velocity != Vector2.zero)
        {
            animator.SetBool("isWalking", false);
            rigidBody.velocity = Vector2.zero;
            targetPosition = null;
        }
    }

    private Vector3 GetNextRandomPosition()
    {
        var randomX = Random.Range(-maxXRange, maxXRange);
        var randomY = Random.Range(-maxYRange, maxYRange);
        return new Vector3(randomX, randomY, 0);
    }
}
