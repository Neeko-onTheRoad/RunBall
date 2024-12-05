using UnityEngine;

public class GameManager : MonoBehaviour {

	public Player Player;
	public MapGenerateManager MapGenerator;

	public void Start() {
		
		Vector3 startRoomPosition = MapGenerator
			.MapGeneration
			.ResultStartRoom
			.Floor
			.transform
			.position;

		Player.transform.position = startRoomPosition + Vector3.up * 3f;

	}
}