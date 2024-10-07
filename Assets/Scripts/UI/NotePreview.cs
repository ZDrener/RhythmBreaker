using UnityEngine;

public class NotePreview : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private AnimationCurve _approachCurve;
    private Vector3 _startScale;
    private float _offset;
    private float _startSampledTime;
    private float _completionRatio;
    private Color _color;
    private int _segments = 32; // Number of segments to define the circle shape
    private float _startRadius = 1500; // Starting radius of the circle
    private float _endRadius = 150; // Final radius of the circle

    public void Init(Vector3 pStartScale, float pBeatOffset, Color pColor) {
        _startScale = pStartScale;
        _offset = pBeatOffset;
        _startSampledTime = BeatmapManager.SampledTime;
        _color = pColor;

        // Initialize the LineRenderer properties
        _lineRenderer.positionCount = _segments + 1; // +1 to close the circle
        _lineRenderer.useWorldSpace = false;

        UpdateCircle(_startRadius);
    }

    private void Update() {
        _completionRatio = (BeatmapManager.SampledTime - _startSampledTime) / _offset;
        float currentRadius = Mathf.Lerp(_startRadius, _endRadius, _approachCurve.Evaluate(_completionRatio));

        // Update circle size and color over time
        UpdateCircle(currentRadius);
        _lineRenderer.startColor = new Color(_color.r, _color.g, _color.b, Mathf.Lerp(0, .35f, _approachCurve.Evaluate(_completionRatio)));
        _lineRenderer.endColor = _lineRenderer.startColor;

        if (BeatmapManager.SampledTime > _startSampledTime + _offset) {
            Destroy(gameObject);
        }
    }

    private void UpdateCircle(float radius) {
        float angle = 0f;
        for (int i = 0; i <= _segments; i++) {
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            _lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
            angle += 360f / _segments;
        }
    }
}
