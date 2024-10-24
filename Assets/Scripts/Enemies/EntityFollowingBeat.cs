using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntityFollowingBeat : MonoBehaviour
{
	[Header("PATTERN")]
	[Space]
	[SerializeField] protected NoteType m_NoteAwaited;
	[SerializeField][Tooltip("The amount of notes needing to be played before the enemy's action")] protected int m_NotesForAction = 1;

	protected int m_CurrentNoteCount = 0;

	protected virtual void Awake()
	{
        BeatmapManager.ON_TriggerNote.AddListener(OnNotePlayed);
    }

	/// <summary>
	/// Called on every note event played by the BeatmapManager.
	/// </summary>
	/// <param name="pNote">The note that was played. Used to determine if the enemy should act or not.</param>
	protected virtual void OnNotePlayed(NoteType pNote)
	{
		if (pNote == m_NoteAwaited)
		{
			m_CurrentNoteCount++;

			if (m_CurrentNoteCount >= m_NotesForAction)
			{
				PlayBeatAction();
				m_CurrentNoteCount = 0;
			}
		}
	}

	/// <summary>
	/// Called after m_NotesAwaited. Manages the entities action, ie: enemies attack, traps go off.
	/// </summary>
	protected virtual void PlayBeatAction()
	{
		
	}


	protected virtual void OnDestroy()
	{
        BeatmapManager.ON_TriggerNote.RemoveListener(OnNotePlayed);
    }
}
