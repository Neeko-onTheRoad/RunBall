using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	//======================================================================| Fields
	
	public Image Image;
	public Transform CameraFollow;
	public TrailRenderer TrailRenderer;

	[Header("Movement")]
	public float JumpPower;
	public float MoveLerpLevel;
	public float MoveSpeed;

	[Header("Teleport")]
	public float TeleportDuration;
	public Ease TeleportEase;

	[Header("Dash")]
	public float DashDuration;
	public float DashSpeed;
	public float DashCoolTime;

	//======================================================================| Properties

	public bool Dashing { get; private set; } = false;
	public bool Hitable { get; private set; } = true;

	//======================================================================| Members

	private bool canDash = true;
	private bool teleporting = false;
	private float dashCoolDown = 0f;

	private Rigidbody rigidBody;
	private Vector3 velocity = new();
	private Vector3 dashVelocity = new();

	//======================================================================| Methods

	private void Awake() {
		rigidBody = GetComponent<Rigidbody>();
		Enemy.Player = transform;
	}

	public void Update() {
		dashCoolDown -= Time.deltaTime;

		if (!teleporting) {
			if (!Dashing) {
				if (canDash) {
					if (dashCoolDown < 0f) {
						if (Input.GetKeyDown(KeyCode.Mouse1)) {
							StartCoroutine(Dash());
						}
					}
				}
			}
		}
	}

	private IEnumerator Dash() {
		
		Dashing = true;
		canDash = false;
		Hitable = false;
		dashCoolDown = DashCoolTime;
		TrailRenderer.gameObject.SetActive(true);
		
		float spent = 0f;

		Vector3 force = (Camera.main.transform.forward + CameraFollow.forward) * DashSpeed / 2f;

		while (spent < DashDuration) {
			spent += Time.deltaTime;
			rigidBody.linearVelocity = force * Time.deltaTime;

			if (teleporting) break;

			yield return null;
		}

		Hitable = true;
		Dashing = false;

		if (teleporting) yield break;

		float trailSpent = 0f;
		TrailRenderer trail = GetComponentInChildren<TrailRenderer>();

		while (trailSpent < trail.time) {
			trailSpent += Time.deltaTime;

			trail.widthCurve.keys[0].value = (trail.time - trailSpent) / trail.time;

			yield return null;
		}

		trail.widthCurve.keys[0].value = 1f;
		TrailRenderer.gameObject.SetActive(false);

	}

	public void FixedUpdate() {
		
		Vector3 offset = Vector3.zero;

		if (Input.GetKey(KeyCode.W)) offset.z += 1f;
		if (Input.GetKey(KeyCode.S)) offset.z -= 1f;
		if (Input.GetKey(KeyCode.D)) offset.x += 1f;
		if (Input.GetKey(KeyCode.A)) offset.x -= 1f;

        Vector3 forward = CameraFollow.forward;
        Vector3 right = CameraFollow.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * offset.z + right * offset.x).normalized;
        Vector3 newVelocity = new Vector3(0f, rigidBody.linearVelocity.y, 0f) + MoveSpeed * Time.fixedDeltaTime * moveDirection;
        velocity = Vector3.Lerp(rigidBody.linearVelocity, newVelocity, MoveLerpLevel * Time.fixedDeltaTime);

		dashVelocity = Vector3.Lerp(dashVelocity, Vector3.zero, MoveLerpLevel * Time.fixedDeltaTime);

        if (moveDirection != Vector3.zero) {
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                Quaternion.LookRotation(moveDirection), 
                MoveLerpLevel * Time.fixedDeltaTime
            );
        }

		if (teleporting || Dashing) return;
		rigidBody.linearVelocity = velocity + dashVelocity;

	}

	private void OnCollisionStay(Collision collision) {

		if (collision.collider.CompareTag("Floor")) {

			canDash = true;
			Vector3 newVelocity = rigidBody.linearVelocity - rigidBody.linearVelocity.y * Vector3.up;
			newVelocity += new Vector3(0f, JumpPower, 0f);

			rigidBody.linearVelocity = newVelocity;

		}
	}

	private void OnCollisionEnter(Collision collision) {
	
		if (collision.collider.CompareTag("Portal")) {
			
			if (teleporting) return;

			transform.DOKill();

			velocity = Vector3.zero;
			rigidBody.linearVelocity = Vector3.zero;

			teleporting = true;
			TrailRenderer.gameObject.SetActive(true);

			Portal portal = collision.gameObject.GetComponent<Portal>();
			Vector3 newPosition = portal.PairPortal.transform.position;
			transform.position = portal.transform.position;

			transform.DOMove(newPosition, TeleportDuration).SetEase(TeleportEase).OnComplete(() => 
				transform.DOMove(newPosition - portal.PairPortal.transform.forward * 0.8f, 0f).OnComplete(() => {
					teleporting = false;
					TrailRenderer.gameObject.SetActive(false);
				})
			);
		}

	}

	private void OnTriggerEnter(Collider other) {
		if (other.GetComponent<Collider>().CompareTag("Clear")) {
			StartCoroutine(Clear());			
			Image.DOColor(new Color(0f, 0f, 0f, 1f), 3f);
		}
	}

	private IEnumerator Clear() {

		float force = 0f;

		while (true) {
			
			Vector3 newVelocity = new(rigidBody.linearVelocity.x, force, rigidBody.linearVelocity.z);
			force += Time.deltaTime * 50f;

			rigidBody.linearVelocity = newVelocity;
			yield return null;

		}

	}

}