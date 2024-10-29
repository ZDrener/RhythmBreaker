using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class ComboDisplay : MonoBehaviour
{
	protected Animator m_Animator;

	[Header("REFERENCE")]
	[Space]
	[SerializeField] Image m_BackgroundImage;
	[SerializeField] Image m_OutlineImage;
	[SerializeField] protected TextMeshProUGUI m_ComboText;

	protected const string m_APPEAR_TRIGGER = "Appear";
	protected const string m_DISAPPEAR_TRIGGER = "Disappear";
	protected const string m_SHAKE_FLOAT = "Shake";

	protected int CurrentCombo
	{
		get { return m_CurrentCombo; }
		set 
		{
			m_CurrentCombo = value;
			UpdateCombo();
		}
	}
	protected int m_CurrentCombo = 0;

	protected void OnKill()
	{
		CurrentCombo++;
	}

	protected void UpdateCombo()
	{
		if (CurrentCombo > 1)
		{
			m_ComboText.text = m_CurrentCombo.ToString();
			m_Animator.SetTrigger(m_APPEAR_TRIGGER);
			//m_Animator.SetFloat(m_SHAKE_FLOAT, 69);
		}

	}

	protected void ResetCombo()
	{
		if (CurrentCombo > 0)
		{
			CurrentCombo = 0;
			m_Animator.SetTrigger(m_DISAPPEAR_TRIGGER);
		}
	}

}
