using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FinisherManager : MonoBehaviour
{
	[SerializeField] protected GameObject m_FinisherInputPrefab;
	[SerializeField] protected Transform m_InputsParent;
	protected List<FinisherInput> m_FinisherInputs = new List<FinisherInput>();
	protected int m_CurrentInput = 0;

	public UnityEvent ResetInputState;
	public UnityEvent FinisherSuccess;
	public UnityEvent<Color, int> FinisherSlash;

	protected void OnPlayerDash(DashDirection pDirection)
	{
		print(pDirection.ToString());
		if (m_FinisherInputs[m_CurrentInput].CheckInput(pDirection))
		{
			// Success!

			FinisherSlash?.Invoke(m_FinisherInputs[m_CurrentInput].InputColor, (int)pDirection);
            m_CurrentInput++;

			if (m_CurrentInput == m_FinisherInputs.Count)
			{
				FinisherSuccess?.Invoke();
				PlayerMovement.ON_Dash.RemoveListener(OnPlayerDash);
                ResetFinisherInputs();
            }

		}
		else
		{
			ResetInputState?.Invoke();
			m_CurrentInput = 0;
			// Failure!
		}
	}

	public void SetupFinisher(int pInputCount = 4)
	{
		if (m_FinisherInputs.Count != 0)
			ResetFinisherInputs();
		else
			PlayerMovement.ON_Dash.AddListener(OnPlayerDash);

		FinisherInput lInput;
        m_CurrentInput = 0;

        for (int i = 0; i < pInputCount; i++)
		{
			lInput = Instantiate(m_FinisherInputPrefab, m_InputsParent).GetComponent<FinisherInput>();
			lInput.SetupInput((DashDirection)Random.Range(0,4));
			m_FinisherInputs.Add(lInput);
			ResetInputState.AddListener(lInput.ResetInput);
		}
	}

	protected void ResetFinisherInputs()
	{
		foreach (FinisherInput input in m_FinisherInputs)
			Destroy(input.gameObject);

		m_FinisherInputs.Clear();
	}

}
