using UnityEngine;

public class FramesUncapper : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 60;
        Destroy(gameObject);
    }
}
