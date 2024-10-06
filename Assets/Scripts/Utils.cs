using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static Vector2 ScreenToWorld(Camera pCamera, Vector3 pPosition) {
        // Check if the screen position is within the screen bounds
        if (pPosition.x < 0 || pPosition.x > Screen.width || pPosition.y < 0 || pPosition.y > Screen.height) {
            return Vector2.zero;        // Finger is outside the screen
        }

        pPosition.z = 0;
        pPosition = pCamera.ScreenToWorldPoint(pPosition);
        return pPosition;
    }

}
