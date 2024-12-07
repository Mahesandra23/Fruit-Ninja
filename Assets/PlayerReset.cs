using UnityEngine;

public class PlayerReset : MonoBehaviour
{
    void Awake()
{
    Debug.Log("PlayerReset script Awake called.");
    startingPosition = transform.position;
        Debug.Log("Starting position saved as: " + startingPosition);
}

    private Vector3 startingPosition;

    void Start()
    {
        // Save the starting position once at the beginning of the game
        startingPosition = transform.position;
        Debug.Log("Starting position saved as: " + startingPosition);
    }
}
