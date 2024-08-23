using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public PlayerController player;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the player has never entered the checkpoint until now...
        if (collision.gameObject.name == "Player" && !player.hasRegisteredCheckpoint(transform.position))
        {
            player.setSpawnPoint(transform.position);
            EventsManager.instance.ResetLevels();
        }
    }
}
