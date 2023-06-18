using UnityEngine;

public class Waypoint : MonoBehaviour
{
    private Camera cam;
    private bool isDragging;
    private float distance;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (!isDragging)
            return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 newPosition = ray.GetPoint(distance);
        transform.position = newPosition;
    }

    private void OnMouseDown()
    {
        isDragging = true;
        distance = Vector3.Distance(transform.position, cam.transform.position);
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }
}
