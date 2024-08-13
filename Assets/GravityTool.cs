using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityTool : MonoBehaviour
{
    // Public static instance of the GravityTool
    public static GravityTool _instance;

    // Awake method to enforce singleton pattern
    private void Awake()
    {
        // If an instance already exists and it's not this one, destroy the new one
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public Vector2 getVectorFromDirection(GravityDirection direction)
    {
        Vector2 unit = new Vector2(0, 0);
        switch (direction)
        {
            case GravityDirection.NORTH:
                unit = new Vector2(0, 1);
                break;
            case GravityDirection.SOUTH:
                unit = new Vector2(0, -1);
                break;
            case GravityDirection.EAST:
                unit = new Vector2(1, 0);
                break;
            case GravityDirection.WEST:
                unit = new Vector2(-1, 0);
                break;
        }

        return unit;
    }


}
