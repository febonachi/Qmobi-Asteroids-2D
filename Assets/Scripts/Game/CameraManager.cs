﻿using Cinemachine;
using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
    #region editor
    [SerializeField] private Camera cam = default;
    [SerializeField] private CinemachineVirtualCamera vcam = default;
    #endregion

    #region public properties
    public Camera mainCamera => cam;
    public CinemachineVirtualCamera virtualCamera => vcam;
    #endregion

    private float cachedFieldOfView = 45f;

    private Coroutine shakeCoroutine;
    private CinemachineBasicMultiChannelPerlin vcamPerlin;

    #region private
    private void Awake() {
        cachedFieldOfView = vcam.m_Lens.FieldOfView;

        vcamPerlin = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        resetVcam();
    }

    private void resetVcam() {
        vcamPerlin.m_AmplitudeGain = 0f;
        vcamPerlin.m_FrequencyGain = 0f;
    }

    private IEnumerator _shake(float duration, float amplitude, float frequency, bool damping) {
        vcamPerlin.m_AmplitudeGain = amplitude;
        vcamPerlin.m_FrequencyGain = frequency;

        if (damping) {
            float elapsed = 0f;
            float elapsedStep = Time.deltaTime / duration;
            float localAmplitude = amplitude;
            float amplitudeStep = elapsedStep * amplitude;
            float localFrequency = frequency;
            float frequencyStep = elapsedStep * frequency;

            while(elapsed < 1f) {
                vcamPerlin.m_AmplitudeGain = localAmplitude;
                vcamPerlin.m_FrequencyGain = localFrequency;

                elapsed += elapsedStep;
                localAmplitude -=  amplitudeStep;
                localFrequency -= frequencyStep;

                yield return null;
            }
        }else yield return new WaitForSeconds(duration);

        resetVcam();
    }
    #endregion

    #region public
    public void shake(float duration, float amplitude, float frequency, bool damping = false) {
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(_shake(duration, amplitude, frequency, damping));
    }

    public void shakeOnce(float amplitude, float frequency, bool damping = false) {
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(_shake(.5f, amplitude, frequency, damping));
    }
    #endregion
}
