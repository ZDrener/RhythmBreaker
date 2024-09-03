using UnityEngine;

public class NotePreview : MonoBehaviour
{
	private void Update() {
		transform.position += Vector3.left * 200 * Time.deltaTime;
	}
}