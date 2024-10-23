using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(SphereCollider))]
public class SpikeTrap : EntityFollowingBeat
{
	protected Animator m_Animator;
    protected SphereCollider m_Collider;

	protected bool m_IsTriggered = false;
    protected bool m_IsActive = false;
    protected bool m_IsPlayerOnTop = false;
	protected const string m_SPIKE_O_CLOCK_TRIGGER = "SpikeOClock";

    override protected void Awake()
    {
        base.Awake();

        m_Animator = GetComponent<Animator>();
        m_Collider = GetComponent<SphereCollider>();
    }

    protected override void OnNotePlayed(NoteType pNote)
    {
        if (m_IsTriggered)
            return;

        base.OnNotePlayed(pNote);
    }

    protected override void PlayBeatAction()
    {
        base.PlayBeatAction();

        m_Animator.SetTrigger(m_SPIKE_O_CLOCK_TRIGGER);
        m_IsTriggered = true;
    }

    private void OnTriggerEnter(Collider pOther)
    {
        if (m_IsActive && pOther.gameObject.GetComponentInParent<Player>())
        {
            m_IsPlayerOnTop = true;
            Player.Instance.TakeDamage(1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponentInParent<Player>())
        {
            m_IsPlayerOnTop = false;
        }
    }

    protected void TriggerTrap()
	{
        m_IsActive = true;
        if (m_IsPlayerOnTop)
        {
            Player.Instance.TakeDamage(1);
        }
    }

	protected void ResetTrap()
	{
        m_IsActive = false;
        m_IsTriggered = false;
    }
}
