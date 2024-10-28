using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class GlobalLaser : MonoBehaviour
{
	[Header("REFERENCES")]
	[SerializeField] private Transform _laserTransform;
	[SerializeField] private Transform _preview;
	[SerializeField] private Image _previewFill;

	[Space]
	[Header("PROPERTIES")]
	[SerializeField] private AnimationCurve _movementCurve;
	[SerializeField] private float _movementDuration;

	private Animator _animator;
	private float _triggerNoteOffset;
	private float _CreationOffset;
	private Camera _camera;
	private bool _isTriggered;

	public void Init(float pCreationOffset, float ptriggerNoteOffset)
	{
		_camera = Camera.main;
		_animator = GetComponent<Animator>();
		_CreationOffset = pCreationOffset;
		_triggerNoteOffset = ptriggerNoteOffset;

		int lRandom = UnityEngine.Random.Range(0, 3);
		transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90) * lRandom);
		Vector3 lPosition = (lRandom % 2 == 0) ? Vector3.right * 4 : Vector3.right * 9.5f;
		_preview.rotation = Quaternion.identity;
		_laserTransform.localPosition = lPosition;
		_preview.localPosition = lPosition;
	}
	void Update()
	{
		if (_isTriggered) return;
		transform.position = _camera.transform.position;
		_previewFill.fillAmount = (BeatmapManager.SampledTime - _CreationOffset) / (_triggerNoteOffset - _CreationOffset);
		if (BeatmapManager.SampledTime >= _triggerNoteOffset) TriggerLaser();
	}
	public void TriggerLaser()
	{
		_isTriggered = true;
		_animator.SetBool("Triggered", true);
		StartCoroutine(LaserMovementCoroutine());
	}

	private IEnumerator LaserMovementCoroutine()
	{
		float lT = 0;
		Vector3 lStartPos = _laserTransform.localPosition;
		Vector3 lEndPos = _laserTransform.localPosition * -1;
		while (lT < _movementDuration)
		{
			transform.position = _camera.transform.position;
			_laserTransform.localPosition = Vector3.Lerp(lStartPos, lEndPos, _movementCurve.Evaluate(lT / _movementDuration));
			lT += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		Destroy(gameObject, 1);
	}
}
