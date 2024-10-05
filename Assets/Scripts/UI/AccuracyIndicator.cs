using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccuracyIndicator : MonoBehaviour
{
	public Image image;
	[SerializeField] private float _lifetime;

	private void Start() {
		StartCoroutine(DisappearCoroutine());
	}

	private IEnumerator DisappearCoroutine() {
		float lT = 0;
		Color lColor = image.color;
		while(lT < _lifetime) {
			lColor.a = 1 - lT / _lifetime;
			image.color = lColor;

			lT += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		Destroy(this);
	}
}
