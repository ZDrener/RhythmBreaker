using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AccuracyDisplay : MonoBehaviour
{
	public static UnityEvent<float> DisplayAccuracyEvent = new UnityEvent<float>();

	[SerializeField] private GameObject _AccuracyIndicator;
	[SerializeField] private RectTransform _container;
	[SerializeField] private Gradient _gradient;

	private void Start() {
		DisplayAccuracyEvent.AddListener(AddIndicator);
	}

	private void AddIndicator(float pAcc) {
		Debug.Log($"Accuracy = {pAcc}");

		AccuracyIndicator lIndicator = Instantiate(_AccuracyIndicator, _container, false).GetComponent<AccuracyIndicator>(); ;
		lIndicator.transform.localPosition = Vector3.left * pAcc * 150;
		lIndicator.image.color = _gradient.Evaluate(Mathf.Abs(pAcc));
	}
}
