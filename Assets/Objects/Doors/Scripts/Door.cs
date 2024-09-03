using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    protected enum DoorState { OPEN, OPENING, CLOSED, CLOSING}

    private Animator _animator;
    private BoxCollider2D _collider;
    private ParticleSystem _particleSystem;

    public AudioSource _gateOpenSound;
    public AudioSource _gateCloseSound;

    private DoorState _doorState;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider2D>();
        _particleSystem = GetComponent<ParticleSystem>();

        _doorState = DoorState.CLOSED;
    }

    public void OpenDoor()
    {
        _gateOpenSound.Play();
        _animator.SetBool("openDoor", true);
        SetParticleSystemEnabled(false);
    }

    public void CloseDoor()
    {
        _gateCloseSound.Play();
        _animator.SetBool("openDoor", false);
        SetParticleSystemEnabled(true);
    }

    // Listens to animation event at the end of CloseDoor animation
    public void enableCollider()
    {
        _collider.enabled = true;
    }

    // Listens to animation event at the end of OpenDoor animation
    public void disableCollider()
    {
        _collider.enabled = false;
    }

    public void SetParticleSystemEnabled(bool enabled)
    {
        ParticleSystem.EmissionModule emission = _particleSystem.emission;
        emission.enabled = enabled;
    }
}
