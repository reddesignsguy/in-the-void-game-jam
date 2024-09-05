using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerHelper : MonoBehaviour
{
    public static AudioMixerHelper Instance { get; private set; }

    public AudioMixer _mixer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // If you want it to persist across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    
    public IEnumerator InterpolateSoundSettings(string param, float start, float end, float duration)
    {
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.unscaledDeltaTime;
            float t = timeElapsed / duration;
            float lerpedValue = Mathf.Lerp(start, end, t);
            _mixer.SetFloat(param, lerpedValue);

            yield return null;
        }
    }
}
