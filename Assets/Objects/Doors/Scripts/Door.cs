using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    protected enum DoorState { OPEN, OPENING, CLOSED, CLOSING}

    private Animator _animator;
    private BoxCollider2D _collider;

    public AudioSource _gateOpenSound;
    public AudioSource _gateCloseSound;

    private DoorState _doorState;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider2D>();

        _doorState = DoorState.CLOSED;
    }

    public void OpenDoor()
    {
        _gateOpenSound.Play();
            _animator.SetBool("openDoor", true);
    }

    public void CloseDoor()
    {
        _gateCloseSound.Play();
            _animator.SetBool("openDoor", false);
    }
}
