using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedDoorPlate : DoorTrigger
{

    [SerializeField]
    private int _activationForce = 10;

    private Animator _animator;
    private BoxCollider2D _collider;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "BoxKey")
            OpenDoor();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "BoxKey")
            CloseDoor();
    }

}
