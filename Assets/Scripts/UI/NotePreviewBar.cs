using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotePreviewBar : MonoBehaviour
{
	[SerializeField] private Image _image;
	private Vector3 _startPosition;
	private float _offset;
	private float _startSampledTime;
	private RectTransform _rect => (RectTransform)transform;
	private float lRatio;

	public void Init(Vector3 pStartPos, float pOffset, Color pColor) {
		_rect.localPosition = _startPosition = pStartPos;
		_offset = pOffset;
		_startSampledTime = BeatmapManager.SampledTime;
		_image.color = pColor;
	}
	private void Update() {
		transform.position += Vector3.left * 200 * Time.deltaTime;
		lRatio = (BeatmapManager.SampledTime - _startSampledTime) / _offset;
		_rect.localPosition = Vector3.LerpUnclamped(
			_startPosition, Vector3.zero,
			lRatio);

		_image.color = new Color(_image.color.r, _image.color.g, _image.color.b, lRatio * 3);

		if (BeatmapManager.SampledTime > _startSampledTime + _offset) Destroy(gameObject);
	}
}
