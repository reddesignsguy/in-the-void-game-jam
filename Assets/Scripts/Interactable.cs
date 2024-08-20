using System.Collections;
using System.Drawing;
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
    // Components
    [SerializeField]
    private PlayerController player;
    private Rigidbody2D _rb;
    private BoxCollider2D _collider;
    private SpriteRenderer _renderer;

    // Audios
    public AudioSource _gravitySound;
    public AudioSource _pickupSound;

    // UI for selecting gravity
    //public GameObject _gravitySelector;
    //private Animator _selectorAnimator;

    // Gets run when gravity is being manipulated
    //private Coroutine SelectGravityCoroutine;
    //private GravityDirection _tempGravityDirection;  // only used in SelectGravityCoroutine

    private bool _beingControlled;

    [SerializeField]
    private GravityDirection _gravityDirection;
    private Vector2 _spawnPosition;

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
    }

    private void Start()
    {
        EventsManager.instance.onInteract += giveControl;
        EventsManager.instance.onCancelInteract += removeControl;
        EventsManager.instance.onChangeGravity += OnChangeGravity;
        EventsManager.instance.onCancelChangeGravity += OnCancelChangeGravity;

        initializeMass();
    }

    private void Update()
    {
            if (Input.GetKeyDown(KeyCode.P))
            {
                ResetState();
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

        Vector2 force = GravityTool._instance.getVectorFromDirection(_gravityDirection) * _forceMagnitude;
        _rb.AddForce(force);
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

    public void giveControl(Vector2 mousePos)
    {
        _rb.velocity *= 0;

        // Mouse is over this object
        if (_collider.OverlapPoint(mousePos))
            _beingControlled = true;
    }

    public void removeControl()
    {
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
        transform.position = _spawnPosition;
        _gravityDirection = GravityDirection.NONE;
    }
}
