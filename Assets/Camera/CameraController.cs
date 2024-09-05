using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

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


    private void Start()
    {
        CameraShaker.Instance.ShakeOnce(1f, 0.3f, .1f, 3f);
    }

    private void Update()
    {
        if (Time.fixedTime > 5 && Time.fixedTime < 6)
        {
            CameraShaker.Instance.ShakeOnce(1f, 0.7f, .1f, 3f);
        }
    }

    void FixedUpdate()
    {
        transform.position = player.transform.position + offset;
        
    }
}
