using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCircleRenderer : MonoBehaviour
{
    internal float Radius;

    private int segments = 100;
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1;
        lineRenderer.useWorldSpace = true;
        DrawCircle();
    }

    private void Update()
    {
        DrawCircle();
    }

    void DrawCircle()
    {
        float angle = 0f;
        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * Radius;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * Radius;

            // Add the object's transform position to the calculated x and y
            Vector3 point = new Vector3(x, y, 0f) + transform.position;
            lineRenderer.SetPosition(i, point);

            angle += 360f / segments;
        }
    }
}
