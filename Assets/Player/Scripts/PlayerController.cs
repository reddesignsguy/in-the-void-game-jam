using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float _jumpForce = 2500;
    [SerializeField]
    private float _runSpeed = 5;
    [SerializeField]
    private float _interactionRadius = 2.8f;
    [SerializeField]
    private bool _gravityOn = true;

    private DefaultPlayerActions _playerActions;
    private Rigidbody2D _rb;
    private BoxCollider2D _collider;
    private SpriteRenderer _sprite;
    private ParticleSystem _particleSystem;

    private bool _playedAirBurstIntro = false;
    public AudioSource _airBurstLoop;
    public AudioSource _airBurstStart;
    public AudioSource _airBurstEnd;

    private InputAction _move;
    private InputAction _jump;
    private InputAction _interact;
    private InputAction _look;
    private InputAction _changeGravity;


    private Vector2 _spawnPoint;


    void Awake()
    {
        _spawnPoint = transform.position;
        _playerActions = new DefaultPlayerActions();
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        _particleSystem.Stop();
    }

    private void OnEnable()
    {
        _move = _playerActions.Player.Move;
        _move.Enable();
        _jump = _playerActions.Player.Jump;
        _jump.Enable();
        _interact = _playerActions.Player.Interact;
        _interact.Enable();
        _look = _playerActions.Player.Look;
        _look.Enable();
        _changeGravity = _playerActions.Player.ChangeGravity;
        _changeGravity.Enable();

        _move.started += OnStartMove;
        _move.canceled += OnCancelMove;
        _jump.performed += OnJump;
        _look.performed += OnLooking;
        _interact.performed += OnInteract;
        _interact.canceled += OnCancelInteract;
        _changeGravity.performed += OnChangeGravity;
        _changeGravity.canceled += OnStopChangeGravity;

    }

    private void OnCancelMove(InputAction.CallbackContext context)
    {
        _airBurstEnd.Play();
        _airBurstStart.Stop();
        _airBurstLoop.Stop();

        _particleSystem.Stop();
    }

    private void OnStartMove(InputAction.CallbackContext context)
    {
        _airBurstStart.Play();
        _airBurstEnd.Stop();
        _airBurstLoop.Stop();

        _particleSystem.Play();
    }

    private void OnDisable()
    {
        _move.Disable();
        _jump.Disable();
        _interact.Disable();
        _look.Disable();
        _interact.Disable();
    }

    private void Update()
    {
        // TODO - refactor resetting the level
        if (Input.GetKeyDown(KeyCode.P))
        {
            _rb.velocity *= 0;
            transform.position = _spawnPoint;
        }

        // Cancel any interactions with objects if the cursor is far away from player
        if (_interact.IsPressed() && !mouseWithinRadius())
        {
            EventsManager.instance.CancelInteract();
        }
    }

    private void FixedUpdate()
    {
        _gravityOn = _rb.gravityScale != 0;

        // ==============================
        // Handle player movement
        // ==============================

        Vector2 inputDirection = _move.ReadValue<Vector2>();

        
        //  Horizontal velocity is manipulated
        if (_gravityOn)
        {
            Vector2 newVel = _rb.velocity;
            newVel.x = inputDirection.x * _runSpeed;

            _rb.velocity = newVel;
        }
        // Forces are manipulated
        else
        {
            bool isMoving = inputDirection.magnitude != 0;
            if (isMoving && !_airBurstStart.isPlaying && !_airBurstLoop.isPlaying)
            {
                 _airBurstLoop.Play();
            }
            else if (!isMoving)
            {
                _airBurstLoop.Stop();
            }

            Vector2 force = inputDirection * 2000 * Time.deltaTime;
            _rb.AddForce(force);
        }
    }

    private void OnJump(InputAction.CallbackContext obj)
    {
        if (_gravityOn && IsGrounded())
            _rb.AddForce(new Vector2(0, _jumpForce));
    }

    private bool IsGrounded()
    {
        RaycastHit hit;
        float raycastDistance = _collider.size.y / 2 + 0.12f;

        // Make raycast origin the bottom at the bottom of player
        Vector2 raycastOrigin = transform.position;

        // Check only for the ground
        int mask = 1 << LayerMask.NameToLayer("Ground");

        // Racyast downwards
        if (Physics2D.Raycast(raycastOrigin, Vector2.down, raycastDistance, mask))
        {
            return true;

        }
        return false;

    }

    private void OnLooking(InputAction.CallbackContext obj)
    {
        // Make sprite look at mouse
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bool shouldLookRight = mouseWorldPos.x - transform.position.x > 0;

        if (shouldLookRight)
            _sprite.flipX = true;
        else
            _sprite.flipX = false;
    }

    /* Invokes the Interact event if the mouse position is within a certain radius of the playuer
     * 
     */
    private void OnInteract(InputAction.CallbackContext obj)
    {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            float distToPlayer = (mousePos - transform.position).magnitude;

            bool mousePosIsWithinPlayerRadius = distToPlayer < _interactionRadius;

            if (mousePosIsWithinPlayerRadius)
            {
                EventsManager.instance.Interact(mousePos);
            }
        
    }


    private void OnChangeGravity(InputAction.CallbackContext context)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        float distToPlayer = (mousePos - transform.position).magnitude;

        bool mousePosIsWithinPlayerRadius = distToPlayer < _interactionRadius;

        if (mousePosIsWithinPlayerRadius)
        {
            EventsManager.instance.ChangeGravity(mousePos);
        }
    }

    private void OnStopChangeGravity(InputAction.CallbackContext context) => EventsManager.instance.CancelChangeGravity();

    private void OnCancelInteract(InputAction.CallbackContext obj) => EventsManager.instance.CancelInteract();

    private bool mouseWithinRadius()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        float distToPlayer = (mousePos - transform.position).magnitude;

        return distToPlayer < _interactionRadius;
    }

    private void enableGravity()
    {
        _rb.gravityScale = 1;
        _rb.drag = 0.0f;
        _gravityOn = true;
    }

    private void disableGravity()
    {
        _rb.gravityScale = 0;
        _rb.drag = 4.1f;
        _gravityOn = false;
    }

    public void setSpawnPoint(Vector2 pos)
    {
        _spawnPoint = pos;
    }

    public void moveToSpawnPoint()
    {
        _rb.velocity *= 0;
        transform.position = _spawnPoint;
    }

}
