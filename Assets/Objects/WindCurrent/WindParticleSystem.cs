using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindParticleSystem : MonoBehaviour
{

    private ParticleSystem _sys;

    private void Awake()
    {
        _sys = GetComponentInChildren<ParticleSystem>();
    }

    // Sets up the transformation of the particle emitter
    public void Setup(Bounds colliderBounds, GravityDirection direction)
    {
        // Position
        transform.position = GetOptimalPosition(colliderBounds, direction);

        // Rotation
        ParticleSystem.ShapeModule newShape = _sys.shape;
        var rotation = newShape.rotation;
        rotation.z = GetOptimalRotation(direction);
        newShape.rotation = rotation;

        // Size
        newShape.radius = GetOptimalRadius(colliderBounds, direction);

    }

    /* The optimal position is the center of the side of a BoxCollider that is opposite to the given gravity direction*/
    private Vector2 GetOptimalPosition(Bounds bounds, GravityDirection direction)
    {
        Vector2 position = new Vector2(0, 0);
        switch (direction)
        {
            // Eg 1) Must shoot from the bottom of collider
            case GravityDirection.NORTH:
                position = new Vector2((bounds.min.x + bounds.max.x) / 2.0f, bounds.min.y);
                break;
            // Eg 2) Must shoot from the top of collider
            case GravityDirection.SOUTH:
                position = new Vector2((bounds.min.x + bounds.max.x) / 2.0f, bounds.max.y);
                break;
            case GravityDirection.EAST:
                position = new Vector2(bounds.min.x, (bounds.min.y + bounds.max.y) / 2.0f);
                break;
            case GravityDirection.WEST:
                position = new Vector2(bounds.max.x, (bounds.min.y + bounds.max.y) / 2.0f);
                break;
        }

        return position;
    }

    /* The optimal rotation is one where the emitter points in the given direction */
    private float GetOptimalRotation(GravityDirection direction)
    {
        float rotation = 0;
        switch (direction)
        {
            // Counter clockwise rotation
            case GravityDirection.NORTH:
                rotation = 0;
                break;
            case GravityDirection.SOUTH:
                rotation = 180;
                break;
            case GravityDirection.EAST:
                rotation = 270;
                break;
            case GravityDirection.WEST:
                rotation = 90;
                break;
        }

        return rotation;
    }

    /* The optimal radius is half the length of the side of the BoxCollider that the emitter is positioned at*/
    private float GetOptimalRadius(Bounds bounds, GravityDirection direction)
    {
        float radius;

        // Radius is half of the horizontal walls
        if (direction == GravityDirection.NORTH || direction == GravityDirection.SOUTH)
            radius = (bounds.max.x - bounds.min.x) / 2.0f;

        else // Radius is half of the vertical walls
            radius = (bounds.max.y - bounds.min.y) / 2.0f;

        return radius;
    }


}
