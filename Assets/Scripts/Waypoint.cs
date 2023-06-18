using UnityEngine;

public class Waypoint : MonoBehaviour
{
    private Camera cam;
    private bool isDragging;
    private float distance;

    private LayerMask raycastLayers; // The layers the raycast can hit

    private void Start()
    {
        cam = Camera.main;

        // Set up the raycast to hit the Waypoint layer only
        raycastLayers = LayerMask.GetMask("Waypoint");  
    }

    private void Update()
    {
        if (!isDragging)
        {
            return;
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 newPosition = ray.GetPoint(distance);
        transform.position = newPosition;
    }

    private void OnMouseDown()
    {
        isDragging = true;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // Use the layer mask in the Raycast
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastLayers))
        {
            distance = hit.distance;
        }
        Debug.Log("Dragging");
    }

    private void OnMouseUp()
    {
        isDragging = false;
        Debug.Log("Not dragging");
    }
}
