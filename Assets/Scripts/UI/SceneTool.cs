using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTool : MonoBehaviour
{
	public void LoadSceneByName(string pName)
	{
		SceneManager.LoadScene(pName);
	}
	public void ReloadCurrentScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
