using System.Collections;
using UnityEngine;

public class ChaoticMover : MonoBehaviour
{
    public RectTransform bounds; //limit to where you're allowed to move to
    public float magnitude; //how much you move
    public float frequency; //how often you move

    private float curTime = 0;
    private float frequencyModified = -1;
    private float magnitudeModified = -1;
    private bool moving = true;


    private float minX;
    private float maxX;
    private float minY;
    private float maxY;
    private float distMultiplier;
    private float mySize;


    public void SetBounds(RectTransform rt)
    {
        bounds = rt;
        MoveDone();
        mySize = (transform as RectTransform).sizeDelta.x * 0.5f; //assuming a circle for now
        minX = bounds.position.x - (bounds.rect.width * 0.5f);
        maxX = bounds.position.x + (bounds.rect.width * 0.5f);
        minY = bounds.position.y - (bounds.rect.height * 0.5f);
        maxY = bounds.position.y + (bounds.rect.height * 0.5f);
        distMultiplier = (maxX - minX) * 0.25f;
    }

    void Update()
    {
        if (!moving)
        {
            curTime += Time.deltaTime;
            if (curTime > frequencyModified)
            {
                StartCoroutine(MoveInDirection());
            }
        }
    }

    private void MoveDone()
    {
        moving = false;
        curTime = 0;
        frequencyModified = (frequency * 0.8f) + (frequency * 0.4f * UnityEngine.Random.value); //between 0.8 and 1.2
        magnitudeModified = (magnitude * 0.8f) + (magnitude * 0.4f * UnityEngine.Random.value); //between 0.8 and 1.2
    }

    private IEnumerator MoveInDirection()
    {
        moving = true;

        float x = (2 * Mathf.PI) * UnityEngine.Random.value;
        Vector3 moveVector = new Vector3(Mathf.Cos(x) * distMultiplier, Mathf.Sin(x) * distMultiplier, 0);

        if (transform.position.x - mySize + moveVector.x < minX)
            moveVector.x = transform.position.x - mySize - minX;
        else if (transform.position.x + mySize + moveVector.x > maxX)
            moveVector.x = maxX - (transform.position.x + mySize);
        if (transform.position.y - mySize + moveVector.y < minY)
            moveVector.y = transform.position.y - mySize - minY;
        else if (transform.position.y + mySize + moveVector.y > maxY)
            moveVector.y = maxY - (transform.position.y + mySize);

        float cur = 0;
        while(cur < 0.5f)
        {
            transform.position += moveVector * Time.deltaTime;
            yield return new WaitForEndOfFrame();
            cur += Time.deltaTime;
        }

        MoveDone();
        yield break;
    }
}
