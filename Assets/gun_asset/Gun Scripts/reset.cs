using UnityEngine;

public class Reset : MonoBehaviour
{
    public Transform teleportDestination;  // Destination to teleport to
    public GameObject player;  // Reference to the player

    void OnTriggerEnter(Collider other){
        if(other.CompareTag("Player")){
            player.SetActive(false);
            player.transform.position = teleportDestination.transform.position;
            player.SetActive(true);
        }
}
}
