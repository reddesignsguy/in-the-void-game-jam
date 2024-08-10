using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;
    private Rigidbody2D _rb;
    private BoxCollider2D _collider;

    private bool _beingControlled;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        EventsManager.instance.onInteract += giveControl;
        EventsManager.instance.onCancelInteract += removeControl;
    }


    public void giveControl(Vector2 mousePos)
    {
        if (_collider.OverlapPoint(mousePos))
        {
            _beingControlled = true;
        }
    }

    public void removeControl()
    {
           _beingControlled = false;
    }

    private void FixedUpdate()
    {
        if (_beingControlled)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            _rb.MovePosition(mousePos);
        }
    }
}
