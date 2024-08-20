using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedDoorPlate : DoorTrigger
{

    [SerializeField]
    private int _activationForce = 10;
    [SerializeField]
    private Color _offColor;
    [SerializeField]
    private Color _onColor;


    private SpriteRenderer _childRenderer;

    private void Awake()
    {
        _childRenderer = transform.Find("Base")?.GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "BoxKey")
        {
            OpenDoor();
            _childRenderer.color = _onColor;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "BoxKey")
        {
            CloseDoor();
            _childRenderer.color = _offColor;
        }
    }

}
