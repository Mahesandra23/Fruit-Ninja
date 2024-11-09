using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleFollowCursor : MonoBehaviour
{
    public Transform rectangle;
    public Camera mainCamera;
    public Vector3 offset = new Vector3(-1, 1, 5); // Offset for the bottom-left corner in world space

    void Start()
    {

    }

    void Update()
    {
        // Get the mouse position in world space
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = mainCamera.nearClipPlane + offset.z;
        Vector3 worldMousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

        // Make the rectangle point toward the mouse
        Vector3 directionToMouse = (worldMousePosition - rectangle.position).normalized;
        rectangle.rotation = Quaternion.LookRotation(directionToMouse);
    }
}

