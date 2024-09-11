using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualizer : MonoBehaviour
{
    [SerializeField] private GameObject[] _bars;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private int _numSamples = 64; // Number of frequency bands
    [SerializeField] private float _strength = 5;
    [SerializeField] private float _lerpSpeed = 5f; // Lerp speed for smooth transition

    private float[] _spectrumData;

    void Start() {
        _spectrumData = new float[_numSamples];
        foreach (GameObject lBar in _bars) lBar.SetActive(true);
    }

    void Update() {
        _audioSource.GetSpectrumData(_spectrumData, 0, FFTWindow.BlackmanHarris);

        for (int i = 0; i < _bars.Length; i++) {
            // Target y-scale based on spectrum data
            float targetYScale = Mathf.Clamp(_spectrumData[i] * _strength, .25f, 1);

            // Current y-scale of the bar
            float currentYScale = _bars[i].transform.localScale.y;

            // Smoothly interpolate between the current and target scale using Lerp
            float newYScale = Mathf.Lerp(currentYScale, targetYScale, _lerpSpeed * Time.deltaTime);

            // Set the new scale with Lerp applied
            _bars[i].transform.localScale = new Vector3(_bars[i].transform.localScale.x, newYScale, _bars[i].transform.localScale.z);
        }
    }

}
