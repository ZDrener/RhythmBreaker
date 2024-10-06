using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeDetection : MonoBehaviour
{
	//public static bool IsSwiping;

	//[SerializeField] private float _minimumDistance = .2f;
	//[SerializeField] private float _maximumTime = .5f;

	//private Vector2 _startPosition;
	//private float _startTime;
	//private Vector2 _endPosition;
	//private float _endTime;

	//private void OnEnable() {
	//	PlayerInputManager.Instance.OnStartTouch += SwipeStart;
	//	PlayerInputManager.Instance.OnEndTouch += SwipeEnd;
	//}
	//private void OnDisable() {
	//	PlayerInputManager.Instance.OnStartTouch -= SwipeStart;
	//	PlayerInputManager.Instance.OnEndTouch -= SwipeEnd;
	//}

	//private void SwipeStart(Vector2 pPosition, float pTime) {
	//	_startPosition = pPosition;
	//	_startTime = pTime;
	//}
	//private void SwipeEnd(Vector2 pPosition, float pTime) {
	//	_endPosition = pPosition;
	//	_endTime = pTime;
	//	DetectSwipe();
	//}

	//private void DetectSwipe() {
	//	if (Vector3.Distance(_startPosition, _endPosition) >= _minimumDistance &&
	//		_endTime - _startTime <= _maximumTime) {
	//		Debug.DrawLine(_startPosition, _endPosition, Color.red, 5f);

	//		Vector3 lDirection = _endPosition - _startPosition;
	//		Vector2 lDirection2D = new Vector2(lDirection.x, lDirection.y).normalized;
	//	}
	//}
}
