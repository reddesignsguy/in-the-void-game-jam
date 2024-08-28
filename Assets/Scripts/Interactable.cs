using System.Collections;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

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
    // Components
    [SerializeField]
    private PlayerController _player;
    private Rigidbody2D _rb;
    private BoxCollider2D _collider;
    private SpriteRenderer _renderer;
    private Material _material;
    private Animator _animator;

    // Audios
    public AudioSource _gravitySound;
    public AudioSource _pickupSound;

    private bool _beingControlled;

    [SerializeField]
    private GravityDirection _gravityDirection;
    private Vector2 _spawnPosition;
    [SerializeField, Range(0, 1)]
    private float _highlightValue;

    // TODO -- Refactor force and put it in some global SO
    public float _forceMagnitude = 10;
    // TODO  -- refactor
    private Vector2 velocity = Vector2.zero;

    private void Awake()
    {
        _spawnPosition = transform.position;
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _material = GetComponent<SpriteRenderer>().material;
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        EventsManager.instance.onInteract += giveControl;
        EventsManager.instance.onCancelInteract += removeControl;
        EventsManager.instance.onChangeGravity += OnChangeGravity;
        EventsManager.instance.onCancelChangeGravity += OnCancelChangeGravity;
        EventsManager.instance.onResetLevels += ResetState;

        initializeMass();
    }

    // TODO - remove
    Vector2 offset;

    private void FixedUpdate()
    {
        // The force magnitude applied to the interactable can be ZERO
        Vector2 force = GravityTool._instance.getVectorFromDirection(_gravityDirection) * _forceMagnitude;
        _rb.AddForce(force);

        if (!_beingControlled) return;

        Vector2 targetPos;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // TODO - remove
        if (offset == null)
            offset = _rb.position;

        if (_player.mouseWithinRadius())
        {
            // Smoothly interpolate the Rigidbody's position towards the target
            Vector2 tempTargetPos = (Vector2) mousePos + offset;

            targetPos = Vector2.SmoothDamp(_rb.position, tempTargetPos, ref velocity, 0.12f);
        } else
        {
            Vector2 playerToMouseUnit = (mousePos - _player.transform.position).normalized;

            // Target position is just on the border of the interaction radius
            Vector2 unit = playerToMouseUnit * _player._interactionRadius;
            Vector2 origin = _player.transform.position;
            Vector2 tempTargetPos = origin + unit;
            tempTargetPos += offset;

            targetPos = Vector2.SmoothDamp(_rb.position, tempTargetPos, ref velocity, 0.12f);
        }

        // TODO - remove
        //lastMousePos = mousePos;

        _rb.MovePosition(targetPos);
    }

    public void OnChangeGravity(Vector2 mousePos)
    {
        // Mouse isn't over this object
        if (!_collider.OverlapPoint(mousePos))
            return;

        // Initialize gravity selector
        Vector2 thisObjPos = transform.position;
        float halfOfObjHeight = _renderer.size.y / 2;

        // Initiate gravity selection
        GravitySelector._instance.StartSelection(gameObject.GetInstanceID(), thisObjPos, halfOfObjHeight);

        // SFX
        _pickupSound.Play();
    }


    public void OnCancelChangeGravity()
    {
        int objId = GravitySelector._instance._interactableInstanceID;

        if (objId == gameObject.GetInstanceID())
        {
            GravitySelector._instance.EndSelection();

            _gravityDirection = GravitySelector._instance._selectedGravityDirection;

            // Finetuning: Reset velocity for predictable behavior
            _rb.velocity *= 0;

            // SFX
            _gravitySound.Play();
        }
    }

    private void OnMouseOver()
    {
        if (!_player.mouseWithinRadius() && !_beingControlled) {
            disableHighlight();
            return;
        }

        enableHighlight();
    }

    private void OnMouseExit()
    {
        if (_beingControlled) return;

        disableHighlight();
    }

    public void giveControl(Vector2 mousePos)
    {
        // Mouse is over this object
        if (_collider.OverlapPoint(mousePos))
        {
            _rb.velocity *= 0;
            _beingControlled = true;
            enableOutline();

            offset = _rb.position - mousePos;
        }
    }

    public void removeControl()
    {
        disableOutline();
        _beingControlled = false;
    }

    // Mass is based on scale
    private void initializeMass()
    {
        Vector3 scale = transform.localScale;
        _rb.mass = scale.x * scale.y * 2;
    }


    public void ResetState()
    {
        _rb.velocity *= 0;
        transform.position = _spawnPosition;
        _gravityDirection = GravityDirection.NONE;
    }

    private void enableHighlight()
    {
        _material.SetFloat("_Whiteness", _highlightValue);
    }

    private void disableHighlight()
    {
        _material.SetFloat("_Whiteness", 0);
    }

    private void enableOutline()
    {
        _animator.SetBool("Outlined", true);
        _renderer.sortingOrder = 1; // Allows the outline of a block to overlap other sprites
    }

    private void disableOutline()
    {
        _animator.SetBool("Outlined", false);
    }

    // Outline of the block doesn't exist so no need to make it overlap w/ other sprites
    private void OnAnimationEventOutlineEnd()
    {
        _renderer.sortingOrder = 0;
    }
}
