using UnityEngine;

public class Portal : MonoBehaviour {

	public Room.Connection Connection { get; set; }

	public Room StartRoom { get; set; }
	public Room ConnectedRoom { get; set; }

	public Portal PairPortal { get; set; }

	public GameObject ConnectLine { get; set; } = null;

}