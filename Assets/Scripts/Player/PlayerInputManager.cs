using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInputManager : MonoBehaviour
{
    // Events
    public static UnityEvent<Vector2> ON_Movement = new UnityEvent<Vector2>();
    public static UnityEvent ON_DashKeyPressed = new UnityEvent();
    public static UnityEvent ON_MainFireKeyPressed = new UnityEvent();
    public static UnityEvent ON_SecondaryFireKeyPressed = new UnityEvent();

    // Bools (for buffering)
    public static bool MainFireKeyHold;
    public static bool SecondaryFireKeyHold;
    public static bool DashFireKeyHold;

    // Singleton
    public static PlayerInputManager Instance;

    private void Start()
    {
        if (Instance != null) throw new System.Exception("Two instance of a singleton exists at the same time. Go fix it dumbass.");
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) 
        {
            ON_Movement.Invoke(Input.GetAxis("Horizontal") * Vector2.right + Input.GetAxis("Vertical") * Vector2.up);
        }
        if (Input.GetKeyDown(KeyCode.Space)) ON_DashKeyPressed.Invoke();
        if (Input.GetMouseButtonDown(0)) ON_MainFireKeyPressed.Invoke();
        if (Input.GetMouseButtonDown(1)) ON_SecondaryFireKeyPressed.Invoke();

        MainFireKeyHold = Input.GetMouseButton(0);
        SecondaryFireKeyHold = Input.GetMouseButton(1);
        DashFireKeyHold = Input.GetKey(KeyCode.Space);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
