using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GravityDirection
{
    NORTH,
    SOUTH,
    WEST,
    EAST,
    NONE,
}

public class Interactable : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;
    private Rigidbody2D _rb;
    private BoxCollider2D _collider;
    private SpriteRenderer _renderer;

    // UI for selecting gravity
    public SharedReferences _sharedReferences;
    private GameObject _gravitySelector;

    private Coroutine SelectGravityCoroutine;

    private bool _beingControlled;

    private GravityDirection _tempGravityDirection;  // only used in SelectGravityCoroutine
    private GravityDirection _gravityDirection;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        EventsManager.instance.onInteract += giveControl;
        EventsManager.instance.onCancelInteract += removeControl;
        EventsManager.instance.onChangeGravity += OnChangeGravity;
        EventsManager.instance.onCancelChangeGravity += OnCancelChangeGravity;

        //_gravitySelector = _sharedReferences._gravitySelector;
        _gravityDirection = GravityDirection.NONE;

        initializeMass();
    }

    public void OnChangeGravity(Vector2 mousePos)
    {
        // Mouse isn't over this object
        if (!_collider.OverlapPoint(mousePos))
            return;

        // Initialize gravity selector
        //_gravitySelector.SetActive(true);
        Vector2 thisObjPos = transform.position;
        //_gravitySelector.transform.position = thisObjPos;
        //print(_gravitySelector.transform.position);

        SelectGravityCoroutine = StartCoroutine(SelectGravity(thisObjPos));

    }

    public void OnCancelChangeGravity()
    {
        if (SelectGravityCoroutine == null)
            return;

        // Stop the coroutine if it's running
        print("ending coroutine");

        // Change to "default state" logic

        // Coroutine logic
        StopCoroutine(SelectGravityCoroutine);
        SelectGravityCoroutine = null;

        // Gravity logic
        _gravityDirection = _tempGravityDirection;
        _tempGravityDirection = GravityDirection.NONE;

        // Finetuning: Reset velocity for predictable behavior
        _rb.velocity *= 0;

        // Gravity selector logic
        //_gravitySelector.SetActive(false);
    }

    public void giveControl(Vector2 mousePos)
    {
        // Mouse is over this object
        if (_collider.OverlapPoint(mousePos))
        {
            _beingControlled = true;
        }
    }

    public void removeControl()
    {
           _beingControlled = false;
    }

    private IEnumerator SelectGravity(Vector2 centerOfSelector)
    {
        while (true)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            // Check which cardinal direction the player wants to select
            Vector2 directionVector = (Vector2) mousePos - centerOfSelector;
            print(directionVector);
            GravityDirection direction = vectorToSelectedDirection(directionVector);
            print( direction);
            setSelectedGravity(direction);


            yield return new WaitForEndOfFrame();
        }
    }

    private GravityDirection vectorToSelectedDirection(Vector2 vector)
    {
        // TODO -- not yet implemented

        // Edge: Vector not large enough
        if (Mathf.Abs(vector.x) < 0.4 && Mathf.Abs(vector.y) < 0.4)
            return GravityDirection.NONE;

        // The direction is in the axis that has the greatest value
        float absoluteX = Mathf.Abs(vector.x);
        float absoluteY = Mathf.Abs(vector.y);

        bool isAHorizontalDirection = absoluteX > absoluteY;
        if (isAHorizontalDirection)
            return vector.x > 0 ? GravityDirection.EAST : GravityDirection.WEST; // Determine which horizontal dir
        else
            return vector.y > 0 ? GravityDirection.NORTH : GravityDirection.SOUTH; // Determine which vertical dir
    }

    /* Updates gravity selection UI and the temporary gravity variable*/
    private void setSelectedGravity(GravityDirection direction)
    {
        _tempGravityDirection = direction;

    }


    private void FixedUpdate()
    {
        if (_beingControlled)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            _rb.MovePosition(mousePos);
        }

        Vector2 force = getForceFromGravityDirection(_gravityDirection);
        _rb.AddForce(force);
    }

    private Vector2 getForceFromGravityDirection(GravityDirection direction)
    {
        Vector2 force = new Vector2(0, 0);
        switch (direction) {
            case GravityDirection.NORTH:
                force = new Vector2(0, 1);
                break;
            case GravityDirection.SOUTH:
                force = new Vector2(0, -1);
                break;
            case GravityDirection.EAST:
                force = new Vector2(1, 0);
                break;
            case GravityDirection.WEST:
                force = new Vector2(-1, 0);
                break;
        }

        return force;
    }

    private void initializeMass()
    {
        // TODO -- Mass is based on widht/height of sprite
        //_rb.mass = 
    }
}
