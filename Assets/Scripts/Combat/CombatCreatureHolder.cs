using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombatCreatureHolder : MonoBehaviour
{
    public GameObject CreaturePrefab;

    private List<CombatCreature> myCreatures = new List<CombatCreature>();

    private float minX;
    private float maxX;
    private float minY;
    private float maxY;
    private void Awake()
    {
        RectTransform bounds = (transform as RectTransform);
        minX = bounds.position.x - (bounds.rect.width * 0.5f) + .1f;
        maxX = bounds.position.x + (bounds.rect.width * 0.5f) - .1f;
        minY = bounds.position.y - (bounds.rect.height * 0.5f) + .1f;
        maxY = bounds.position.y + (bounds.rect.height * 0.5f) - .1f;

        SetInitial();
    }

    public void SetInitial()
    {
        for (int i = 0; i < 80; i++)
        {
            GameObject go = Instantiate(CreaturePrefab);
            CombatCreature cc = go.GetComponent<CombatCreature>();
            myCreatures.Add(cc);
            cc.SetType((CreatureType)UnityEngine.Random.Range(0, 5));
            cc.transform.parent = transform;
            cc.transform.position = new Vector3(minX + ((maxX - minX) * UnityEngine.Random.value), 
                                                minY + ((maxY - minY) * UnityEngine.Random.value), 
                                                transform.position.z - UnityEngine.Random.value);
            cc.GetComponent<ChaoticMover>().SetBounds(transform as RectTransform);
        }
    }
}
