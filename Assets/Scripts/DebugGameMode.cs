using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugGameMode : MonoBehaviour
{
    // Singleton
    public static DebugGameMode Instance;

    public bool MobileInputs;
    public bool AllowHold;
    public ScreenOrientation Orientation;

    private void Start() {
        if (Instance != null) throw new System.Exception("Two instance of a singleton exists at the same time. Go fix it dumbass.");
        Instance = this;

        Screen.orientation = Orientation;
    }

    public void ReloadScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
