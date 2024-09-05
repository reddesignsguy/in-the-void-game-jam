using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using UnityEngine.Audio;

public class RumbleFXManager : MonoBehaviour
{

    private GameObject[] _backgroundLights;
    private PlayerController _player;
    public AudioMixer _mixer;
    [SerializeField] private AudioSource _rumbleAudio;
    [SerializeField] private AudioSource _creakAudio;
    [SerializeField] private float _duration = 0;
    [SerializeField, Range(0,1)] private float _shakeEndFadePercent = 0.75f;

    private float _timer = 0;
    private float _fxStartTime = 10f;

    private void Awake()
    {
        _backgroundLights = GameObject.FindGameObjectsWithTag("Background Light");
    }


    // Update is called once per frame
    void Update()
    {
        _timer += Time.unscaledDeltaTime;

        if (_timer >= _fxStartTime)
        {
            StartCoroutine(StartRumble());
            _timer = 0;
            float randTime = Random.Range(300, 420);
            _fxStartTime = randTime;
        }
    }

    private IEnumerator StartRumble()
    {
        float startEndFade = 1 - _shakeEndFadePercent;
        CameraShaker.Instance.ShakeOnce(0.8f, 0.8f, startEndFade, _shakeEndFadePercent);

        StartRumbleAudioSequence();

        float rumbleTimer = 0;

        while (rumbleTimer < _duration)
        {
            rumbleTimer += Time.unscaledDeltaTime;

            yield return null;
        }

        EndRumbleAudioSequence();
    }

    private void StartRumbleAudioSequence()
    {
        StartCoroutine(AudioMixerHelper.Instance.InterpolateSoundSettings("Volume", 0, -20f, 0.35f));
        PlayRumbleSounds();
    }

    private void EndRumbleAudioSequence()
    {
        StartCoroutine(AudioMixerHelper.Instance.InterpolateSoundSettings("Volume", -20f, 0, 0.35f));
        StopRumbleSounds();
    }

    private void PlayRumbleSounds()
    {
        _rumbleAudio.Play();
        _creakAudio.Play();
    }

    private void StopRumbleSounds()
    {
        _rumbleAudio.Stop();
        _creakAudio.Stop();
    }

    private void FlickerLights(float duration)
    {

    }



    private void EmitParticles(float duration)
    {

    }
}
