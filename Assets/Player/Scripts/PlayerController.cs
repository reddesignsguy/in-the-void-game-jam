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

    private bool _gravityOn = true;

    private DefaultPlayerActions _playerActions;
    private Rigidbody2D _rb;
    private BoxCollider2D _collider;
    private SpriteRenderer _sprite;

    private InputAction _move;
    private InputAction _jump;
    private InputAction _interact;
    private InputAction _look;


    void Awake()
    {
        _playerActions = new DefaultPlayerActions();
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _sprite = GetComponent<SpriteRenderer>();
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

        _jump.performed += OnJump;
        _look.performed += OnLooking;
    }

    private void OnDisable()
    {
        _move.Disable();
        _jump.Disable();
        _interact.Disable();
        _look.Disable();
        _interact.Disable();
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
        bool shouldLookAhead = mouseWorldPos.x - transform.position.x > 0;
        if (shouldLookAhead)
            _sprite.flipX = true;
        else
            _sprite.flipX = false;

    }


    private void FixedUpdate()
    {
        // Handle player movement
        if (_gravityOn)
        {
            Vector2 dir = _move.ReadValue<Vector2>();

            Vector2 newVel = _rb.velocity;
            newVel.x = dir.x * _runSpeed;

            _rb.velocity = newVel;
        }
    }




}
