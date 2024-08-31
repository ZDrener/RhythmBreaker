using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorAim : MonoBehaviour
{
	[SerializeField] private Camera _camera;

	private void OnEnable() { Cursor.visible = false; }
	private void OnDisable() { Cursor.visible = true; }

	void Update()
	{
		Vector3 lPos = _camera.ScreenToWorldPoint(Input.mousePosition);
		lPos.z = 0;
		transform.position = lPos;

#if UNITY_EDITOR
		Cursor.visible = false;
#endif
	}
}
