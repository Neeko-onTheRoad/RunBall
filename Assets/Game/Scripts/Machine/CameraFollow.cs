using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	//======================================================================| Fields	

	[Header("Follow")]
	public Transform PlayerTransform;
	public float LerpLevel;

	[Header("Rotating")]
	public float MouseSensivity;
	public float ViewXMin;
	public float ViewXMax;

	//======================================================================| Methods

	private void FixedUpdate() {

		transform.position = Vector3.Lerp(
			transform.position,
			PlayerTransform.position,
			LerpLevel * Time.fixedDeltaTime
		);

	}

	private void Update() {
		
		Cursor.lockState = CursorLockMode.Locked;

		Vector3 newAngle = transform.eulerAngles;
		newAngle.y += Input.mousePositionDelta.x * MouseSensivity;
		newAngle.x -= Input.mousePositionDelta.y * MouseSensivity;

		var (clampMin, clampMax) = newAngle.x > 180f
			? (360f + ViewXMin, 361f)
			: (-1, ViewXMax);

		newAngle.x = Mathf.Clamp(newAngle.x, clampMin, clampMax);

		transform.eulerAngles = newAngle;

	}

}
