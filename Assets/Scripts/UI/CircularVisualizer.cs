using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CircularVisualizer : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private int _numSamples = 64; // Number of frequency bands
    [SerializeField] private float _strength = 5f; // Strength of the audio effect
    [SerializeField] private float _radius = 10f; // Radius of the circle
    [SerializeField] private float _lerpSpeed = 5f; // Speed of lerp
    [SerializeField] private int _numPoints = 128; // Number of points on the circle
    [SerializeField] private float _maxScale = 128; // Number of points on the circle

    private LineRenderer _lineRenderer;
    private float[] _spectrumData;
    private Vector3[] _positions;

    void Start() {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = _numPoints + 1; // +1 to close the circle
        _spectrumData = new float[_numSamples];
        _positions = new Vector3[_numPoints + 1]; // +1 for closing the loop
    }

    void Update() {
        // Get audio spectrum data
        _audioSource.GetSpectrumData(_spectrumData, 0, FFTWindow.BlackmanHarris);

        // Calculate the circle points with audio data affecting the radius
        for (int i = 0; i < _numPoints; i++) {
            float angle = (i / (float)_numPoints) * Mathf.PI * 2f; // Angle for each point on the circle

            // Get the audio spectrum data value, scaled to the strength
            float spectrumValue = Mathf.Clamp(_spectrumData[i % _numSamples] * _strength, 0.5f, _maxScale); // Limit the distance scaling

            // Lerp the spectrum value for smooth transition
            float radius = Mathf.Lerp(_radius, _radius * spectrumValue, _lerpSpeed * Time.deltaTime);

            // Calculate the x, y positions based on the angle and radius
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            // Set the new position in the array
            _positions[i] = new Vector3(x, y, 0);
        }

        // Close the circle by setting the last position equal to the first
        _positions[_numPoints] = _positions[0];

        // Apply the positions to the LineRenderer
        _lineRenderer.SetPositions(_positions);

        transform.rotation *= Quaternion.Euler(Vector3.forward * 90 * Time.deltaTime);
    }
}
