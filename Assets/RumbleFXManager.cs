using System.Collections;
using UnityEngine;
using EZCameraShake;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;

public class RumbleFXManager : MonoBehaviour
{

    private GameObject[] _backgroundLights;
    private PlayerController _player;
    public AudioMixer _mixer;
    [SerializeField] private AudioSource _rumbleAudio;
    [SerializeField] private AudioSource _creakAudio;
    [SerializeField] private float _duration = 0;
    [SerializeField] private float _shakeStartFade = 0.4f;
    [SerializeField] private float _shakeEndFade = 5f;

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

    /* Flickers lights by changing their color randomly between 2 options: black or white*/
    private void FlickerLights()
    {
        foreach (GameObject light in _backgroundLights)
        {
            float rand = Random.Range(-1, 1);
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



    private void EmitParticles(float duration)
    {

    }
}
