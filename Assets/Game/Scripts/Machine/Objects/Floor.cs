using UnityEngine;

public class Floor : MonoBehaviour {

	private static MaterialPropertyBlock propertyBlock = null;
	public bool PlayerDetected { get; private set; } = false;

	//======================================================================| Members

	private bool init = false;
	private Renderer objectRenderer;

	//======================================================================| Methods

	public void OnEnable() {
		
		if (init) return;

		objectRenderer = GetComponent<Renderer>();
		Initialize();

		init = true;

	}

	//======================================================================| Private Methods

	private void Initialize() {

		if (propertyBlock == null) {
			propertyBlock = new();	
			objectRenderer.GetPropertyBlock(propertyBlock);
		}

		Vector2 tiling = new(transform.localScale.x, transform.localScale.z);
		tiling /= 2f;

		propertyBlock.SetVector("_BaseMap_ST", tiling);
		objectRenderer.SetPropertyBlock(propertyBlock);

	}

	private void OnTriggerEnter(Collider other) {
		
		if (other.gameObject.CompareTag("Player")) {
			PlayerDetected = true;
		}

	}

	private void OnTriggerExit(Collider other) {
		
		if (other.gameObject.CompareTag("Player")) {
			PlayerDetected = false;
		}

	}
}
