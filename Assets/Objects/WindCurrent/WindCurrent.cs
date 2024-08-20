using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindCurrent : MonoBehaviour
{
    // Components
    private BoxCollider2D _collider;
    [SerializeField]
    private WindParticleSystem _particleSystem;

    // Settings
    public GravityDirection _direction;
    public float _absoluteForce;

    // Objects under the influence of wind current
    private List<Rigidbody2D> affectedObjects = new List<Rigidbody2D>();


    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        _particleSystem.Setup(_collider.bounds, _direction);
    }

    /* Adds an object to a list of objects under the influence of the wind current*/
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // EDGE: Don't consider Ground objects
        if (collision.gameObject.tag == "Ground")
            return;

        Rigidbody2D _rb = collision.GetComponent<Rigidbody2D>();

        // Add new objects to list
        if (_rb && !affectedObjects.Contains(_rb))
            affectedObjects.Add(_rb);
        
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        Rigidbody2D _rb = collision.GetComponent<Rigidbody2D>();

        // Remove seen objects to list
        if (_rb && affectedObjects.Contains(_rb))
            affectedObjects.Remove(_rb);
    }

    /* Applies a directional force to each object inside _affectedObjects given that the wind current is not blocked at a given object's position*/
    private void FixedUpdate()
    {
        // Calculate force to be applied
        Vector2 unit = GravityTool._instance.getVectorFromDirection(_direction);
        Vector2 force = unit * _absoluteForce;

        // Apply force to each object
        foreach (Rigidbody2D _rb in affectedObjects)
        {
            if (_rb)
            {
                BoxCollider2D subjectCollider = _rb.GetComponent<BoxCollider2D>();
                Bounds subjectBounds = subjectCollider.bounds;

                // Check if the wind current is blocked
                if (!isWindCurrentBlocked(_rb.gameObject, subjectBounds)) { 
                    // Apply force
                    _rb.AddForce(force);
                }
            }
        }

    }

    /* Algorithm: Checks if the wind current is being blocked by another object by:
     * 1. Shooting 3 raycasts FROM the center and 2 sides of an affected game object AGAINST the direction of the wind current
     * 2. If all 3 raycasts hit an object, that means the wind current is being blocked by some other object
     */
    private bool isWindCurrentBlocked(GameObject go, Bounds subjectBounds)
    {
        Bounds bounds = _collider.bounds;

        // Create origin points for each of the 3 raycasts
        List<Vector2> origins = new List<Vector2>(3) { subjectBounds.min, subjectBounds.max, subjectBounds.center };

        // Ignore certain objects
        int layerToIgnore = LayerMask.NameToLayer("IgnoreLayer");
        int layerMask = ~(1 << layerToIgnore); // Invert to ignore this layer

        int numHits = 0;

        // Set up raycasts for each origin
        foreach (Vector2 origin in origins)
        {
            Vector2 landingPoint;

            /* The landing point of each raycast is where the raycast would hypothetically leave the boundaries of the wind current (see the provided function description)
             * Calculating the landing point is dependent on the wind current direction
             */
            switch (_direction)
            {
                case GravityDirection.NORTH:
                    landingPoint = new Vector2(origin.x, bounds.min.y);
                    break;
                case GravityDirection.SOUTH:
                    landingPoint = new Vector2(origin.x, bounds.max.y);
                    break;
                case GravityDirection.WEST:
                    landingPoint = new Vector2(bounds.max.x, origin.y);
                    break;
                case GravityDirection.EAST:
                    landingPoint = new Vector2(bounds.min.x, origin.y);
                    break;
                default:
                    landingPoint = new Vector2(bounds.min.x, origin.y);
                    break;
            }


            // Send ONE raycast
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, landingPoint - origin);

            // Iterate through all objects that have been hit by the raycast
            foreach (RaycastHit2D hit in hits)
            {
                // Only collisions with BoxKeys are considered hits
                if (hit.collider != null && hit.collider.gameObject != go && hit.collider.gameObject.tag == "BoxKey")
                    numHits += 1;
            }
        }

        if (numHits == 3)
            return true;

        return false;
    }
}
