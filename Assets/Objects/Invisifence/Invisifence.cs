using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invisifence : MonoBehaviour
{
    public PlayerController player;

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player.moveToSpawnPoint();
        }
    }
}
