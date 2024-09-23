using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimatorSpeedScaler : MonoBehaviour
{
	public static UnityEvent<float> ON_SetAnimatorSpeedScales = new UnityEvent<float>();
	[SerializeField] protected Animator _animator;

	private void Start() {
		Metronome.Trigger.AddListener(PlayAnim);
	}

	private void PlayAnim() {
		_animator.SetBool("0", true);
	}
}
