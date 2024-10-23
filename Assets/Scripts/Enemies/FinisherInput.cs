using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class FinisherInput : MonoBehaviour
{
	protected Animator m_Animator;
	protected const string m_HIT_BOOL = "Hit";
	protected DashDirection m_Direction;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void SetupInput(DashDirection pDirection)
	{
		m_Direction = pDirection;
		float lRotation = 0f;

        // Top tier coding right there
        if ((int)m_Direction == 0)
			lRotation = 180f;
		else if ((int)m_Direction == 2)
			lRotation = -90f;
		else if ((int)m_Direction == 3)
			lRotation = 90f;

		transform.rotation *= Quaternion.AngleAxis(lRotation, Vector3.forward);
    }

	public bool CheckInput(DashDirection pDirection)
	{
		if (pDirection == m_Direction)
		{
			InputSuccess();
			return true;
		}
		else
			return false;
	}

	protected void InputSuccess()
	{
		m_Animator.SetBool(m_HIT_BOOL, true);
	}

	public void ResetInput()
	{
        m_Animator.SetBool(m_HIT_BOOL, false);
    }
}
