using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MixLevels : MonoBehaviour
{
    // SINGLETON
    public static MixLevels _instance;

    // References
    public AudioMixer _mixer;

    // EQ Settings
    public float _volume = -20f;
    public float _distortion = 0.21f;
    public float _bass = 2.91f;
    public float _mids = 0.34f;
    public float _highs = 0.41f;

    public float _defaultVolume = 0f;       // Default to 0
    public float _defaultDistortion = 0f;   // Default to 0
    public float _defaultBass = 1f;         // Default to 1
    public float _defaultMids = 1f;         // Default to 1
    public float _defaultHighs = 1f;        // Default to 1

    // Lerp settings for sound transitions
    public float _fadeDuration = 3f;  // Duration in seconds

    private List<Coroutine> _runningSoundInterpolations;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _runningSoundInterpolations = new List<Coroutine>();
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    public void DrownSounds()
    {
        ClearSoundInterpolations();

        _runningSoundInterpolations.Add(StartCoroutine(InterpolateSoundSettings("Volume", _defaultVolume, _volume)));
        _runningSoundInterpolations.Add(StartCoroutine(InterpolateSoundSettings("Distortion", _defaultDistortion, _distortion)));
        _runningSoundInterpolations.Add(StartCoroutine(InterpolateSoundSettings("Bass", _defaultBass, _bass)));
        _runningSoundInterpolations.Add(StartCoroutine(InterpolateSoundSettings("Mids", _defaultMids, _mids)));
        _runningSoundInterpolations.Add(StartCoroutine(InterpolateSoundSettings("Highs", _defaultHighs, _highs)));
    }

    public void Reset()
    {
        ClearSoundInterpolations();

        _runningSoundInterpolations.Add(StartCoroutine(InterpolateSoundSettings("Volume", _volume, _defaultVolume)));
        _runningSoundInterpolations.Add(StartCoroutine(InterpolateSoundSettings("Distortion", _distortion, _defaultDistortion)));
        _runningSoundInterpolations.Add(StartCoroutine(InterpolateSoundSettings("Bass", _bass, _defaultBass)));
        _runningSoundInterpolations.Add(StartCoroutine(InterpolateSoundSettings("Mids", _mids, _defaultMids)));
        _runningSoundInterpolations.Add(StartCoroutine(InterpolateSoundSettings("Highs", _highs, _defaultHighs)));
    }

    private void ClearSoundInterpolations()
    {
        if (_runningSoundInterpolations.Count > 0)
        {
            foreach (Coroutine c in _runningSoundInterpolations)
            {
                StopCoroutine(c);
            }
        }

        _runningSoundInterpolations.Clear();
    }

    private IEnumerator InterpolateSoundSettings(string param, float start, float end)
    {
        float timeElapsed = 0f;

        while (timeElapsed < _fadeDuration)
        {
            print(timeElapsed);
            timeElapsed += Time.unscaledDeltaTime;
            float t = timeElapsed / _fadeDuration;
            float lerpedValue = Mathf.Lerp(start, end, t);
            _mixer.SetFloat(param, lerpedValue);

            yield return null;
        }
}
}
