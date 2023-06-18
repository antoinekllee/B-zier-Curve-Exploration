using UnityEngine;
using MyBox;
using System.Collections.Generic;

[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class Coaster : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints = new List<Transform>();
    [SerializeField] private float width = 0.1f;
    [SerializeField] private int resolution = 100;
    [SerializeField] private float speed = 1f; 
    [SerializeField, MustBeAssigned] private Transform bezierObject = null; 

    [SerializeField, MustBeAssigned] private Waypoint waypointPrefab = null;

    private LineRenderer lineRenderer = null;
    private Vector2[] lastPositions; 
    private bool isDragging = false;
    private float currentPosition = 0;

    private void OnValidate()
    {
        lineRenderer = GetComponent<LineRenderer>();
        
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;

        lastPositions = new Vector2[waypoints.Count];

        for (int i = 0; i < waypoints.Count; i++)
            lastPositions[i] = waypoints[i].position;

        if (!isDragging)
        {
            UpdateLineRenderer();
        }
    }

    private void Update()
    {
        isDragging = false;
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (Vector2.Distance(lastPositions[i], waypoints[i].position) > 0.01f)
            {
                isDragging = true;
                lastPositions[i] = waypoints[i].position;
                break;
            }
        }

        if (!isDragging)
        {
            UpdateLineRenderer();
            UpdateBezierObject();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10f;
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            Waypoint newWaypoint = Instantiate(waypointPrefab, worldPosition, Quaternion.identity, transform);

            waypoints.Add(newWaypoint.transform);

            lastPositions = new Vector2[waypoints.Count];
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

    private void UpdateBezierObject()
    {
        currentPosition += Time.deltaTime * speed;
        if (currentPosition > 1)
        {
            currentPosition = 0;
        }

        if (Application.isPlaying)
        {
            Vector2[] points = new Vector2[waypoints.Count];
            for (int i = 0; i < waypoints.Count; i++)
            {
                points[i] = waypoints[i].position;
            }

            bezierObject.position = BezierCurve(currentPosition, waypoints);
            
            Vector2 tangent = BezierCurveTangent(currentPosition, points);

            float rotationZ = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0f, 0f, rotationZ);
            bezierObject.rotation = rotation; 
        }
    }


    private Vector2 BezierCurve(float t, List<Transform> points)
    {
        List<Vector2> pointPositions = new List<Vector2>();

        foreach (Transform point in points)
        {
            pointPositions.Add(point.position);
        }

        return BezierCurveRecursive(t, pointPositions);
    }

    private Vector2 BezierCurveRecursive(float t, List<Vector2> pointPositions)
    {
        if (pointPositions.Count == 1)
        {
            return pointPositions[0];
        }
        else
        {
            List<Vector2> newPoints = new List<Vector2>();
            for (int i = 0; i < pointPositions.Count - 1; i++)
            {
                newPoints.Add(Vector2.Lerp(pointPositions[i], pointPositions[i + 1], t));
            }
            
            return BezierCurveRecursive(t, newPoints);
        }
    }

    private Vector2 BezierCurveTangent(float t, Vector2[] pointPositions)
    {
        if (pointPositions.Length == 2)
        {
            return (pointPositions[1] - pointPositions[0]).normalized;
        }
        else
        {
            Vector2[] newPoints = new Vector2[pointPositions.Length - 1];
            for (int i = 0; i < newPoints.Length; i++)
            {
                newPoints[i] = Vector2.Lerp(pointPositions[i], pointPositions[i + 1], t);
            }
            return BezierCurveTangent(t, newPoints);
        }
    }
}
