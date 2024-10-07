using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class UICamEvent : MonoBehaviour
{
	[SerializeField] private float _targetOrthoSize = 8;
	[SerializeField] private float _zoomOrthoSize = 7;
	[SerializeField] private float _lerpForce = 100;

	private CinemachineVirtualCamera _virtualCamera;

	private void Start() {
		_virtualCamera = GetComponent<CinemachineVirtualCamera>();
		BeatmapManager.ON_TriggerNote.AddListener(ZoomOnHit);
	}

	public void ZoomOnHit(NoteType pType) {
		_virtualCamera.m_Lens.OrthographicSize = _zoomOrthoSize;
	}

	private void Update() {
		if (_virtualCamera.m_Lens.OrthographicSize != _targetOrthoSize)
			_virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(
				_virtualCamera.m_Lens.OrthographicSize, 
				_targetOrthoSize, 
				_lerpForce * Time.deltaTime);
	}
}
