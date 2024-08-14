using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindCurrent : MonoBehaviour
{
    private BoxCollider2D _collider;
    [SerializeField]
    private WindParticleSystem _particleSystem;
    public GravityDirection _direction;
    public float _absoluteForce;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();

        // Set up particle system
        Vector2 position = new Vector2();
    }

    private void Start()
    {
        _particleSystem.Setup(_collider.bounds, _direction);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
            return;

        GameObject obj = collision.gameObject;

        // Check if force can be appleid to this object or not
        BoxCollider2D subjectCollider = obj.GetComponent<BoxCollider2D>();
        Bounds subjectBounds = subjectCollider.bounds;
        if (isWindCurrentBlocked(collision.gameObject, subjectBounds))
            return;

        // Calculate force
        Vector2 unit = GravityTool._instance.getVectorFromDirection(_direction);
        Vector2 force = unit * _absoluteForce;

        Rigidbody2D _rb = obj.GetComponent<Rigidbody2D>();

        // Apply force
        if (_rb) _rb.AddForce(force);
    }

    // Check if the wind current is being blocked by another object
    private bool isWindCurrentBlocked(GameObject go, Bounds subjectBounds)
    {
        Bounds bounds = _collider.bounds;

        List<Vector2> origins = new List<Vector2>(3) { subjectBounds.min, subjectBounds.max, subjectBounds.center };

        int layerToIgnore = LayerMask.NameToLayer("IgnoreLayer");
        int layerMask = ~(1 << layerToIgnore); // Invert to ignore this layer

        int numHits = 0;
        foreach (Vector2 origin in origins)
        {
            Vector2 landingPoint;
            // Determine landing point of raycast
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


            // Send raycast
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, landingPoint - origin);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.collider.gameObject != go && hit.collider.gameObject.tag == "BoxKey")
                    numHits += 1;
            }
        }

        if (numHits == 3)
            return true;

        return false;
    }
}
