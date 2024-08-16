using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalAudio : MonoBehaviour
{
    public AudioSource _oblivion;
    public AudioSource _voidCircles;

    public BoxCollider2D _collider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !_voidCircles.isPlaying)
        {
        _oblivion.Stop();
        _voidCircles.Play();
        }

    }
}
