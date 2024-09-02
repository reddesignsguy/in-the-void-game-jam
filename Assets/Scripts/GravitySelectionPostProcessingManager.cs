using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GravitySelectionPostProcessingManager : MonoBehaviour
{

    [SerializeField] private Volume _postProcessingVolume;
    [SerializeField] private float _desaturationValue = -35f;
    [SerializeField] private float _fadeDuration = 3.5f;

    private ColorAdjustments _colorAdjustments;

    void Start()
    {
        _postProcessingVolume.profile.TryGet(out _colorAdjustments);
    }

    public void Desaturate()
    {
        StartCoroutine(IntepolateSaturation(0, _desaturationValue));
    }

    public void ResetSaturation()
    {
        print("Desaturating");
        StartCoroutine(IntepolateSaturation(_desaturationValue, 0));
    }

    private IEnumerator IntepolateSaturation(float start, float end)
    {
        float timeElapsed = 0f;

        while (timeElapsed < _fadeDuration)
        {
            timeElapsed += Time.unscaledDeltaTime;
            float t = timeElapsed / _fadeDuration;
            float lerpedValue = Mathf.Lerp(start, end, t);
            _colorAdjustments.saturation.value = lerpedValue;

            yield return null;
        }
    }
}
