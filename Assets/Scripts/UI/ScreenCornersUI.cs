using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCornersUI : MonoBehaviour
{
	[SerializeField] protected Animator m_Animator;
	protected const string m_SLOW_MOTION_FLOAT = "SlowMotion";

    protected void Awake()
    {
        if (m_Animator == null) m_Animator = GetComponent<Animator>();

		BeatmapManager.SlowdownEffectEvent.AddListener(OnSlowMotionUpdate);
    }

	protected void OnSlowMotionUpdate(float pSlowMotionFactor, float pRatio)
	{
		m_Animator.SetFloat(m_SLOW_MOTION_FLOAT, pRatio);
	}
}
