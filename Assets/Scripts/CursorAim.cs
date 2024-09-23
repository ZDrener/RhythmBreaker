using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorAim : MonoBehaviour
{
	public static CursorAim Instance;
	public static Vector3 Target;

	[SerializeField] private Camera _camera;

	private void OnEnable() { Cursor.visible = false; }
	private void OnDisable() { Cursor.visible = true; }

	private List<DemoDummy> _enemiesInRange = new List<DemoDummy>();

	private void Awake() {
		if (Instance != null) throw new Exception("Two instances of a singleton exist at the same time");
		Instance = this;
	}

	void Update() {
		Vector3 lPos = _camera.ScreenToWorldPoint(Input.mousePosition);
		lPos.z = 0;
		transform.position = lPos;

		// Set target
		if (_enemiesInRange.Count == 0) Target = transform.position;
		else {
			_enemiesInRange.Sort(delegate (DemoDummy n1, DemoDummy n2) {
				return Vector3.Distance(transform.position, n1.transform.position).CompareTo(Vector3.Distance(transform.position, n2.transform.position));
			});
			Target = _enemiesInRange[0].transform.position;
		}

		
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		DemoDummy lEnemy;
		if (collision.gameObject.TryGetComponent(out lEnemy)) {
			_enemiesInRange.Add(lEnemy);
		}
	}

	private void OnTriggerExit2D(Collider2D collision) {
		DemoDummy lEnemy;
		if (collision.gameObject.TryGetComponent(out lEnemy)) {
			_enemiesInRange.Remove(lEnemy);
		}
	}

	private void OnDestroy() {
		if (Instance == this) Instance = null;
	}
}
