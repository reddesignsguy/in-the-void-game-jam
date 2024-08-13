using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class WindParticleSystem : MonoBehaviour
{

    private ParticleSystem _sys;

    private void Awake()
    {
        _sys = GetComponent<ParticleSystem>();
    }

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

    private Vector2 GetOptimalPosition(Bounds bounds, GravityDirection direction)
    {
        Vector2 position = new Vector2(0, 0);
        switch (direction)
        {
            // Must shoot from the bottom of collider
            case GravityDirection.NORTH:
                position = new Vector2(0, 1);
                break;
            case GravityDirection.SOUTH:
                position = new Vector2(0, -1);
                break;
            case GravityDirection.EAST:
                position = new Vector2(1, 0);
                break;
            case GravityDirection.WEST:
                position = new Vector2(bounds.max.x, (bounds.min.y + bounds.max.y) / 2.0f);
                break;
        }

        return position;
    }

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
