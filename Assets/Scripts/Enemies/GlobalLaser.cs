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
	private Vector3 _laserOffsetFromPivot;

	public void Init(float pCreationOffset, float ptriggerNoteOffset)
	{
		_camera = Camera.main;
		_animator = GetComponent<Animator>();
		_CreationOffset = pCreationOffset;
		_triggerNoteOffset = ptriggerNoteOffset;

		int lRandom = UnityEngine.Random.Range(0, 3);
		switch (lRandom)
		{
			case 0:
				_laserOffsetFromPivot = Vector3.right * 4;
				_laserTransform.rotation = Quaternion.identity;
				break;
			case 1:
				_laserOffsetFromPivot = Vector3.left * 4;
				_laserTransform.rotation = Quaternion.identity;
				break;
			case 2:
				_laserOffsetFromPivot = Vector3.up * 9.5f;
				_laserTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
				break;
			case 3:
				_laserOffsetFromPivot = Vector3.down * 9.5f;
				_laserTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
				break;
		}
		_preview.rotation = Quaternion.identity;

		transform.position = Player.Instance.transform.position;
		Debug.LogWarning(_laserOffsetFromPivot);
		_laserTransform.position = transform.position +_laserOffsetFromPivot;
		_preview.position = transform.position + _laserOffsetFromPivot;
	}
	void Update()
	{
		transform.position = Player.Instance.transform.position;
		_laserTransform.position = transform.position + _laserOffsetFromPivot;
		_preview.position = transform.position + _laserOffsetFromPivot;
		if (_isTriggered) return;
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
			_laserOffsetFromPivot = Vector3.Lerp(lStartPos, lEndPos, _movementCurve.Evaluate(lT / _movementDuration));
			_laserTransform.position = transform.position + _laserOffsetFromPivot;
			lT += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		Destroy(gameObject, 1);
	}
}
