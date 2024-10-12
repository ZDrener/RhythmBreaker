using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
	public static HealthBar Instance { get; protected set; }

	[SerializeField] protected TextMeshProUGUI m_HealthText;

	protected int m_Health;
	protected int m_MaxHealth;

	private void Awake()
	{
		if (Instance != null)
			Destroy(gameObject);

		Instance = this;
		Player.PlayerHealthChange += OnPayerHealthChange;
	}

	public void InitHealth(int pHealth, int pMaxHealth)
	{
		m_Health = pHealth;
		m_MaxHealth = pHealth;
		UpdateHealthText();
	}

	protected void OnPayerHealthChange(int pNewHealth)
	{
		m_Health = pNewHealth; 
		UpdateHealthText();
	}

	protected void UpdateHealthText()
	{
		m_HealthText.text = m_Health + "/" + m_MaxHealth;
	}

    private void OnDestroy()
    {
        Player.PlayerHealthChange -= OnPayerHealthChange;
    }
}
