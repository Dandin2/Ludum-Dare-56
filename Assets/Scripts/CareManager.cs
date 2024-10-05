using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CareManager : MonoBehaviour
{
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
}
