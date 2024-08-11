using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
    public static EventsManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject); // If an instance already exists, destroy this one
        }
        else
        {
            instance = this;
        }

    }

    public event Action<Vector2> onInteract;

    public void Interact(Vector2 mousePos)
    {
        if (onInteract != null)
        {
            onInteract(mousePos);
        }
    }

    public event Action onCancelInteract;


    public void CancelInteract()
    {
        if (onCancelInteract != null)
        {
            onCancelInteract();
        }
    }

    public event Action <Vector2> onChangeGravity;


    public void ChangeGravity(Vector2 mousePos)
    {
        if (onChangeGravity != null)
        {
            onChangeGravity(mousePos);
        }
    }

    public event Action onCancelChangeGravity;


    public void CancelChangeGravity()
    {
        if (onCancelChangeGravity != null)
        {
            onCancelChangeGravity();
        }
    }
}
