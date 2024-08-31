using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

[Serializable]
public class RecoloredParticle
{
	public ParticleSystem ps;
	public List<int> colorIDs;
}
public class ParticleRecolor : MonoBehaviour
{
	[SerializeField] protected List<RecoloredParticle> _RecoloredParticles;


	public void InitRecolor(Color pColor)
	{
		ColorOverLifetimeModule colorModule;
		foreach (RecoloredParticle lRp in _RecoloredParticles)
		{
			foreach (int lID in lRp.colorIDs)
			{
				colorModule = lRp.ps.colorOverLifetime;
				colorModule.color.gradient.colorKeys[lID].color = pColor;

                colorModule.enabled = true;

                Gradient gradient = colorModule.color.gradient;
                GradientColorKey[] colorKeys = gradient.colorKeys;
                GradientAlphaKey[] alphaKeys = gradient.alphaKeys;

                if (lID >= 0 && lID < colorKeys.Length) colorKeys[lID].color = pColor;
                
                Gradient newGradient = new Gradient();
                newGradient.SetKeys(colorKeys, alphaKeys);

                colorModule.color = new ParticleSystem.MinMaxGradient(newGradient);
            }
		}
	}
}
