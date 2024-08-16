using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public AudioSource _gravitySound;
    public AudioSource _pickupSound;

    // UI for selecting gravity
    public GameObject _gravitySelector;
    private Animator _selectorAnimator;

    private Coroutine SelectGravityCoroutine;

    private bool _beingControlled;

    private GravityDirection _tempGravityDirection;  // only used in SelectGravityCoroutine
    [SerializeField]
    private GravityDirection _gravityDirection;
    private Vector2 _spawnPosition;

    // TODO -- Refactor force and put it in some global SO
    public float _forceMagnitude = 10;
    // TODO  -- refactor
    private Vector2 velocity = Vector2.zero; // Internal velocity used by SmoothDamp

    private void Awake()
    {
        _spawnPosition = transform.position;
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

        if (_gravityDirection == null)
            _gravityDirection = GravityDirection.NONE;

        _gravitySelector.SetActive(false);
        _selectorAnimator = _gravitySelector.GetComponent<Animator>();

        initializeMass();
    }

    private void Update()
    {
            if (Input.GetKeyDown(KeyCode.P))
            {
                print("restarting");
                ResetState();
            }
    }

    public void OnChangeGravity(Vector2 mousePos)
    {
        // Mouse isn't over this object
        if (!_collider.OverlapPoint(mousePos))
            return;

        // Initialize gravity selector
        _gravitySelector.SetActive(true);
        Vector2 thisObjPos = transform.position;

        // Annoying edge case: Just ignore
        thisObjPos.y += _renderer.size.y / 2;
        _gravitySelector.transform.position = thisObjPos;
        thisObjPos.y -= _renderer.size.y / 2;

        // Handle gravity selection separately
        SelectGravityCoroutine = StartCoroutine(SelectGravity(thisObjPos));

        // Pause physics system
        pauseTime(true);

        // SFX
        _pickupSound.Play();
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
        _gravitySelector.SetActive(false);

        // SFX
        _gravitySound.Play();

        pauseTime(false);
    }

    public void giveControl(Vector2 mousePos)
    {
        _rb.velocity *= 0;

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
            GravityDirection direction = vectorToSelectedDirection(directionVector);
            setSelectedGravity(direction);


            yield return new WaitForEndOfFrame();
        }
    }

    private GravityDirection vectorToSelectedDirection(Vector2 vector)
    {
        // Edge: Vector not large enough
        if (Mathf.Abs(vector.x) < 0.45 && Mathf.Abs(vector.y) < 0.45)
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
        // If a new dir is selected, play appropriate anims
        if (direction != _tempGravityDirection)
        {
            setSelectionAnimation(direction);
        }

        _tempGravityDirection = direction;

    }

    private  void setSelectionAnimation (GravityDirection direction)
    {
        switch (direction)
        {
            case GravityDirection.NORTH:
                _selectorAnimator.SetTrigger("North");
                print("receiving north signal");
                return;
            case GravityDirection.SOUTH:
                _selectorAnimator.SetTrigger("South");
                return;
            case GravityDirection.EAST:
                _selectorAnimator.SetTrigger("East");
                return;
            case GravityDirection.WEST:
                _selectorAnimator.SetTrigger("West");
                return;
            default:
                _selectorAnimator.SetTrigger("None");
                return;

        }
    }


    private void FixedUpdate()
    {
        if (_beingControlled)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            // Smoothly interpolate the Rigidbody's position towards the target
            Vector2 targetPos = Vector2.SmoothDamp(_rb.position, mousePos, ref velocity, 0.12f);

            _rb.MovePosition(targetPos);
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

        return force * _forceMagnitude;
    }

    // Mass is based on scale
    private void initializeMass()
    {
        Vector3 scale = transform.localScale;
        _rb.mass = scale.x * scale.y * 2;
    }

    private void pauseTime(bool pause)
    {
        if (pause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    public void ResetState()
    {
        transform.position = _spawnPosition;
        _gravityDirection = GravityDirection.NONE;
    }
}
