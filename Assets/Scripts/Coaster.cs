using UnityEngine;
using MyBox; 

[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class Coaster : MonoBehaviour
{
    [SerializeField, AutoProperty] private Transform[] waypoints = null;
    [SerializeField] private float width = 0.1f;
    [SerializeField] private int resolution = 100;

    private LineRenderer lineRenderer = null;
    private Vector3[] lastPositions; // Used to check if the waypoints have moved
    private bool isDragging = false;

    private void OnValidate()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;

        lastPositions = new Vector3[waypoints.Length];

        for (int i = 0; i < waypoints.Length; i++)
            lastPositions[i] = waypoints[i].position;

        if (!isDragging)
        {
            UpdateLineRenderer();
        }
    }

    private void Update()
    {
        isDragging = false;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (Vector3.Distance(lastPositions[i], waypoints[i].position) > 0.01f)
            {
                isDragging = true;
                lastPositions[i] = waypoints[i].position;
                break;
            }
        }

        if (!isDragging)
        {
            UpdateLineRenderer();
        }
    }

    private void UpdateLineRenderer()
    {
        lineRenderer.positionCount = resolution + 1;
        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            lineRenderer.SetPosition(i, BezierCurve(t, waypoints));
        }
    }

    private Vector3 BezierCurve(float t, Transform[] points)
    {
        Vector3[] pointPositions = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            pointPositions[i] = points[i].position;
        }

        return BezierCurveRecursive(t, pointPositions);
    }

    private Vector3 BezierCurveRecursive(float t, Vector3[] pointPositions)
    {
        if (pointPositions.Length == 1)
        {
            return pointPositions[0];
        }
        else
        {
            Vector3[] newPoints = new Vector3[pointPositions.Length - 1];
            for (int i = 0; i < newPoints.Length; i++)
            {
                newPoints[i] = Vector3.Lerp(pointPositions[i], pointPositions[i + 1], t);
            }
            return BezierCurveRecursive(t, newPoints);
        }
    }
}
