using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GravitySelectionSoundManager : MonoBehaviour
{
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
    public float _fadeDuration = 0.35f;  // Duration in seconds

    private List<Coroutine> _runningSoundInterpolations;

    private void Awake()
    {
        _runningSoundInterpolations = new List<Coroutine>();
    }

    public void DrownSounds()
    {
        ClearSoundInterpolations();

        // IM SORRY DUDE!
        _runningSoundInterpolations.Add(StartCoroutine(AudioMixerHelper.Instance.InterpolateSoundSettings("Volume", _defaultVolume, _volume, _fadeDuration)));
        _runningSoundInterpolations.Add(StartCoroutine(AudioMixerHelper.Instance.InterpolateSoundSettings("Distortion", _defaultDistortion, _distortion, _fadeDuration)));
        _runningSoundInterpolations.Add(StartCoroutine(AudioMixerHelper.Instance.InterpolateSoundSettings("Bass", _defaultBass, _bass, _fadeDuration)));
        _runningSoundInterpolations.Add(StartCoroutine(AudioMixerHelper.Instance.InterpolateSoundSettings("Mids", _defaultMids, _mids, _fadeDuration)));
        _runningSoundInterpolations.Add(StartCoroutine(AudioMixerHelper.Instance.InterpolateSoundSettings("Highs", _defaultHighs, _highs, _fadeDuration)));
    }

    public void Reset()
    {
        ClearSoundInterpolations();

        var mixerHelper = AudioMixerHelper.Instance;

        // AGAIN I AM SORRY
        _runningSoundInterpolations.Add(StartCoroutine(mixerHelper.InterpolateSoundSettings("Volume", _volume, _defaultVolume, _fadeDuration)));
        _runningSoundInterpolations.Add(StartCoroutine(mixerHelper.InterpolateSoundSettings("Distortion", _distortion, _defaultDistortion, _fadeDuration)));
        _runningSoundInterpolations.Add(StartCoroutine(mixerHelper.InterpolateSoundSettings("Bass", _bass, _defaultBass, _fadeDuration)));
        _runningSoundInterpolations.Add(StartCoroutine(mixerHelper.InterpolateSoundSettings("Mids", _mids, _defaultMids, _fadeDuration)));
        _runningSoundInterpolations.Add(StartCoroutine(mixerHelper.InterpolateSoundSettings("Highs", _highs, _defaultHighs, _fadeDuration)));
    }

    private void ClearSoundInterpolations()
    {
        foreach (Coroutine c in _runningSoundInterpolations)
        {
            if (c == null)
            {
                StopCoroutine(c);
            }
        }

        _runningSoundInterpolations.Clear();
    }
}
