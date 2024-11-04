using UnityEngine;

public class FramesUncapper : MonoBehaviour
{
    private void Awake()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Application.targetFrameRate = 60;
#endif
        Destroy(gameObject);
    }
}
