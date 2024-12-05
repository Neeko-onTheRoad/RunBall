using UnityEngine;

public class Enemy : MonoBehaviour {

	public static Transform Player;
	public float JumpPower;
	public float MoveLerpLevel;
	public float MoveSpeed;

	[HideInInspector]
	public bool PlayerFound = false;
	[HideInInspector]
	public int Health = 5;

	private Rigidbody rigidBody;
	private bool playerFoundPrevious = false;

	private Vector3 velocity = new();
	private Vector3 pushVelocity = new();

	//======================================================================| Methods

	private void Awake() {
		rigidBody = GetComponent<Rigidbody>();
	}

	private void FixedUpdate() {

		if (PlayerFound != playerFoundPrevious) {

			if (PlayerFound == false) {
				rigidBody.linearVelocity = rigidBody.linearVelocity.y * Vector3.up;
			}

			playerFoundPrevious = PlayerFound;
		}

		if (PlayerFound) {
			
			Vector3 direction = Player.transform.position - transform.position;
			direction.y = 0;
			direction.Normalize();
			direction *= MoveSpeed;

			velocity = Vector3.Lerp(rigidBody.linearVelocity, direction, MoveLerpLevel * Time.fixedDeltaTime);

			rigidBody.linearVelocity = velocity;

		}

	}

	private void OnCollisionStay(Collision collision) {

		if (collision.collider.CompareTag("Floor")) {

			Vector3 newVelocity = rigidBody.linearVelocity - rigidBody.linearVelocity.y * Vector3.up;
			newVelocity += new Vector3(0f, JumpPower, 0f);

			rigidBody.linearVelocity = newVelocity;

		}

	}

	private void OnTriggerEnter(Collider collision) {
		if (collision.GetComponent<Collider>().CompareTag("Dead")) {
			Destroy(gameObject);
		}
	}
}