using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region PROJECTILE LINE CLASS
[Serializable]
public class ProjectileLine
{
	public LineRenderer line;
	public Gradient gradient;
	public AnimationCurve widthCurve;
	public float speedMultiplier;
	public List<int> colorIDs;
}
#endregion

public class LineProjectile : Projectile
{
	[Header("LINES")]
	[SerializeField] protected List<ProjectileLine> _lines;
	[SerializeField] protected GameObject _impactPrefab;
	[Space]
	[SerializeField] protected WeaponStatSO _stats;

	public override void ProjectileInit(Color pColor)
	{
		base.ProjectileInit(pColor);

		// Init Lines
		RaycastHit2D[] lHits = Physics2D.RaycastAll(transform.position, transform.right, _stats.range);
		Vector3 lEndPoint;

		// Pierce through all targets
		if (lHits.Length < _stats.piercing + 1 || _stats.piercing == -1)
			lEndPoint = Vector3.right * _stats.range;
		// Pierce a fix number of targets
		else
		{
			Array.Sort(lHits, (x, y) => x.distance.CompareTo(y.distance));
			lEndPoint = transform.InverseTransformPoint(lHits[_stats.piercing].point);
			Debug.DrawLine(transform.position, lHits[_stats.piercing].point, Color.green, 2);
		}

		// Set Lines colors and positions
		foreach (ProjectileLine lLine in _lines) SetLine(pColor, lEndPoint, lLine);		

		// Create Impacts
		GameObject lImpact;
		ParticleRecolor lPr;
		for (int i = 0; i < Mathf.Min(lHits.Length, _stats.piercing + 1); i++)
		{
			lImpact = Instantiate(_impactPrefab);
			lImpact.transform.position = lHits[i].point;
			lImpact.transform.rotation = transform.rotation;
			if(lImpact.TryGetComponent(out lPr)) lPr.InitRecolor(pColor);			
		}

		// On end init
		StartCoroutine(LifetimeCoroutine());
	}

	protected virtual void SetLine(Color pColor, Vector3 lEndPoint, ProjectileLine lLine)
	{
		// Set positions
		lLine.line.positionCount = 2;
		lLine.line.SetPosition(0, Vector3.zero);
		lLine.line.SetPosition(1, lEndPoint);

		// Get the current color keys
		GradientColorKey[] colorKeys = lLine.gradient.colorKeys;

		// Set colors
		foreach (int lID in lLine.colorIDs) colorKeys[lID].color = pColor;

		// Create a new Gradient object with the modified color keys
		Gradient newGradient = new Gradient();
		newGradient.SetKeys(colorKeys, lLine.gradient.alphaKeys);

		// Assign the new gradient to the LineRenderer
		lLine.gradient = newGradient;
	}

	public virtual IEnumerator LifetimeCoroutine()
	{
		float lTime = 0;
		float lRatio1 = 0;

		while (lRatio1 < 1)
		{
			lRatio1 = lTime / _stats.lifetime;

			foreach (ProjectileLine lLine in _lines)
			{
				// Directly use the colorGradient instead of startColor and endColor
				// Line color
				lLine.line.startColor = lLine.line.endColor = lLine.gradient.Evaluate(lRatio1);

				// line Width
				lLine.line.startWidth = lLine.line.endWidth = lLine.widthCurve.Evaluate(lRatio1);

				// Line progression
				if (_stats.speed * lLine.speedMultiplier > 0)
				{
					lLine.line.SetPosition(0, Vector3.Lerp(
						Vector3.zero,
						lLine.line.GetPosition(1),
						lTime / (lLine.line.GetPosition(1).magnitude / (_stats.speed * lLine.speedMultiplier))));
				}
			}

			lTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		Destroy(gameObject);
	}
}
