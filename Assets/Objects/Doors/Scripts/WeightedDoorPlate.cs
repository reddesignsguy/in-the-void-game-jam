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

    
    protected override void OpenDoor()
    {
        
    }

    protected override void CloseDoor()
    {
        Debug.LogError("Close door not implemented");
    }
}
