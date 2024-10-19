using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimatorSpeedScaler : MonoBehaviour
{
	public static UnityEvent<float> ON_SetAnimatorSpeedScales = new UnityEvent<float>();
	[SerializeField] protected Animator _animator;

	protected virtual void Start() {
		Metronome.Trigger.AddListener(PlayAnim);
	}

	protected virtual void PlayAnim() {
		_animator.SetBool("0", true);
	}
}
