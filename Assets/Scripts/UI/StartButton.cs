using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class StartButton : MonoBehaviour
{
	public void ButtonPressed()
	{
		BeatmapManager.Instance.StartSong();

		Destroy(gameObject);
	}
}
