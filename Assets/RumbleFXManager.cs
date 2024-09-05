using System.Collections;
using UnityEngine;
using EZCameraShake;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;

public class RumbleFXManager : MonoBehaviour
{

    private GameObject[] _backgroundLights;
    public AudioMixer _mixer;
    [SerializeField] private AudioSource _rumbleAudio;
    [SerializeField] private AudioSource _creakAudio;
    [SerializeField] private AudioSource _flickerAudio;
    [SerializeField] private float _duration = 0;
    [SerializeField] private float _shakeStartFade = 0.4f;
    [SerializeField] private float _shakeEndFade = 5f;
    [SerializeField] private float _timeTillNextRumbleMin = 10;
    [SerializeField] private float _timeTillNextRumbleMax = 15;

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
            float randTime = Random.Range(_timeTillNextRumbleMin, _timeTillNextRumbleMax);
            _fxStartTime = randTime;
        }
    }

    private IEnumerator StartRumble()
    {
        CameraShakeInstance shake = CameraShaker.Instance.StartShake(3f, 1.3f, _shakeStartFade);


        StartRumbleAudioSequence();

        float rumbleTimer = 0;

        while (rumbleTimer < _duration)
        {
            rumbleTimer += Time.unscaledDeltaTime;

            FlickerLights();
            yield return null;
        }

        EndRumbleAudioSequence();
        ResetLights();

        shake.StartFadeOut(_shakeEndFade);
    }

    private void StartRumbleAudioSequence()
    {
        StartCoroutine(AudioMixerHelper.Instance.InterpolateSoundSettings("Music_Volume", 0, -20f, 0.35f));
        PlayRumbleSounds();
    }

    private void EndRumbleAudioSequence()
    {
        StartCoroutine(AudioMixerHelper.Instance.InterpolateSoundSettings("Music_Volume", -20f, 0, 0.1f));
        StopRumbleSounds();
    }

    private void PlayRumbleSounds()
    {
        _rumbleAudio.Play();
        _creakAudio.Play();
        _flickerAudio.Play();
    }

    private void StopRumbleSounds()
    {
        StartCoroutine(AudioMixerHelper.Instance.InterpolateSoundSettings("Rumble_Volume", 0, -20f, 0.55f));
        StartCoroutine(AudioMixerHelper.Instance.InterpolateSoundSettings("Creak_Volume", 0, -20f, 0.55f));
        StartCoroutine(AudioMixerHelper.Instance.InterpolateSoundSettings("Flicker_Volume", 0, -20f, 0.55f));
    }

    /* Flickers lights by changing their color randomly between 2 options: black or white*/
    private void FlickerLights()
    {
        foreach (GameObject light in _backgroundLights)
        {
            float rand = Random.Range(-1, 5);
            Color newColor = rand < 0 ? new Color(1,1,1): new Color(0, 0, 0);

            Light2D settings = light.GetComponent<Light2D>();
            settings.color = newColor;
        }
    }

    /* Resets lights back to white */
    private void ResetLights()
    {
        foreach (GameObject light in _backgroundLights)
        {
            Light2D settings = light.GetComponent<Light2D>();
            settings.color = new Color(1, 1, 1);
        }
    }
}
