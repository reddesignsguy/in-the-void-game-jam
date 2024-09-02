using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;
    [SerializeField]
    private float zOffset;

    private Vector3 offset;

    private void Awake()
    {
        offset = new Vector3(0,0,zOffset);
    }

    void FixedUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}
