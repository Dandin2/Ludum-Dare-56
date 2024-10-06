using System.Linq;
using UnityEngine;

public class Egg : MonoBehaviour
{
    public string Name;
    public float destructionTime = 10f;

    private float hatchTime;
    private bool isHatching;
    private CreatureStats[] possibleCreatures;

    private CareManager manager;
    private Animator animator;

    private Creature creatureSpwaned;

    void Awake()
    {
        var startingStats = ScriptableObjectFinder.FindScriptableObjectByName<EggStats>(Name);
        hatchTime = Random.Range(startingStats.MinHatchTime, startingStats.MaxHatchTime);
        var allPossibleCreatureStats = ScriptableObjectFinder.GetAllCreatureStats();
        possibleCreatures = allPossibleCreatureStats.Where(x => x.CreatureType == startingStats.CreatureType).ToArray();
    }

    private void Start()
    {
        hatchTime += Time.time;
        manager = FindObjectOfType<CareManager>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isHatching && Time.time >= hatchTime)
        {
            isHatching = true;
            animator.SetTrigger("Hatch");
            var randomCreature = possibleCreatures[Random.Range(0, possibleCreatures.Length)];
            var go = Instantiate(randomCreature.CreaturePrefab, transform.position, Quaternion.identity);
            creatureSpwaned = go.GetComponent<Creature>(); 
            manager.CreaturesOwned.Add(go);
            Destroy(gameObject, destructionTime);
        }

        if (creatureSpwaned != null && creatureSpwaned.isInitialized)
        {
            creatureSpwaned.StopMovement();
        }
    }
}
