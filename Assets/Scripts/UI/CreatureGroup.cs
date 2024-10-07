using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreatureGroup : MonoBehaviour
{
    public GameObject creaturePrefab;

    public GameObject Screen2;
    public ScreenFader Fade;

    private List<CombatCreature> myCreatures = new List<CombatCreature>();
    private bool firstLoad = true;
    private void Awake()
    { 
        UIClick c = GetComponentInChildren<UIClick>();
        c.SetClickAction(() => { StartCoroutine(OnClick()); });
        c.SetHoverAction(() => { myCreatures.ForEach(x => x.SetPreview()); });
        c.SetUnhoverAction(() => { myCreatures.ForEach(x => x.UnPreview()); });
    }

    private void Update()
    {
        if(WorldManager.instance != null && firstLoad)
        {
            firstLoad = false;

            RectTransform rt = transform as RectTransform;
            float minX = -rt.sizeDelta.x * 0.5f + 40;
            float maxX = rt.sizeDelta.x * 0.5f - 40;
            float minY = -rt.sizeDelta.y * 0.5f + 60;
            float maxY = rt.sizeDelta.y * 0.5f - 60;

            //Add some variance.  Pick one element to pick more often, and one to pick less often
            List<int> possibleChoices = new List<int>();
            int priority = UnityEngine.Random.Range(0, 5);
            int hate = priority;
            while (hate != priority)
            {
                hate = UnityEngine.Random.Range(0, 5);
            }
            for (int j = 0; j < 5; j++)
            {
                if (j == priority)
                    possibleChoices.AddRange(Enumerable.Repeat(j, 3));
                else if (j == hate)
                    possibleChoices.AddRange(Enumerable.Repeat(j, 1));
                else
                    possibleChoices.AddRange(Enumerable.Repeat(j, 2));
            }

            for (int i = 0; i < 40; i++)
            {
                GameObject go = Instantiate(creaturePrefab);
                CombatCreature cc = go.GetComponent<CombatCreature>();
                myCreatures.Add(cc);

                CreatureType ct = (CreatureType)possibleChoices[UnityEngine.Random.Range(0, possibleChoices.Count())];
                cc.SetType(new ActiveCreatureStats(WorldManager.instance.CreatureBases.Where(x => x.CreatureType == ct).OrderBy(x => UnityEngine.Random.value).FirstOrDefault()));
                cc.transform.parent = transform;
                cc.transform.localScale = new Vector3(1, 1, 1);
                cc.transform.localPosition = new Vector3(minX + ((maxX - minX) * UnityEngine.Random.value),
                                                    minY + ((maxY - minY) * UnityEngine.Random.value),
                                                    transform.position.z - UnityEngine.Random.value);
                cc.GetComponent<ChaoticMover>().SetBounds(transform as RectTransform);
            }
        }
    }
    private IEnumerator OnClick()
    {
        WorldManager.instance.activeCreatureStats = myCreatures.Select(x => x.myStats).ToList();
        Fade.transform.parent.gameObject.SetActive(false);
        Screen2.SetActive(true);
        yield return new WaitForSeconds(4);
        Fade.transform.parent.gameObject.SetActive(true);
        Fade.FadeToBlack(2, null);
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(2);
        yield break;
    }
}
