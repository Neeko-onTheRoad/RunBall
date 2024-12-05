using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

public class MapGenerateManager : MonoBehaviour {

	public Vector2 MapSize;
	public ProceduralMapGeneration MapGeneration;

	public GameObject EndMark;

	[Header("Floor")]
	public GameObject FloorPrefab;
	public float TurainMultiply;
	public float NoiseMultiply;
	public int RoomSizeMin;
	public int RoomSizeMax;
	public float RoomFloorHeight;

	[Header("Enemy")]
	public GameObject EnemyPrefab;
	public int MaxEnemyRate;
	public int MinEnemyRate;

	[Header("Portal")]
	public GameObject PortalPrefab;
	public GameObject LinePrefab;
	public float PortalSpacing;
	public float PortalFloorMargin;

	//======================================================================| Methods

	private void Start() {

		InvokePmg();
		MakeRooms();
		SpawnEnemies();

		MakePortals();

		EndMark.transform.position = MapGeneration.ResultBossEntryRoom.Position;

	}

	private void InvokePmg() {
 		MapGeneration = new() {
			Size = MapSize,
			PartitioningRange = 0.6f,
			ConnectionRandomRange = 0.5f,
			Generation = 7,
			RoomCount = 17,
			DisplayLevel = 10
		};
		MapGeneration.Invoke();
	}

	private void Update() {
		foreach (var room in MapGeneration.ResultRooms) {
			if (room.PlayerDetected != room.PreviousDectected) {
				foreach (var enemy in room.Enemies) enemy.PlayerFound = room.PlayerDetected;
				room.PreviousDectected = room.PlayerDetected;
			}
		}
	}

	private void MakeRooms() {

 		FloorPrefab.SetActive(false);
		foreach (var room in MapGeneration.ResultRooms) {
			
			GameObject instantiatedFloor = Instantiate(FloorPrefab);
			
			Vector3 newPosition = room.CenterPosition.XZ();
			float perlinNoise = Mathf.PerlinNoise(newPosition.x * NoiseMultiply, newPosition.z * NoiseMultiply);
			newPosition.y = perlinNoise * TurainMultiply;
			room.Height = newPosition.y;

			instantiatedFloor.transform.localPosition = newPosition;

			Vector3 newScale = new(
				Random.Range(RoomSizeMin, RoomSizeMax),
				RoomFloorHeight,
				Random.Range(RoomSizeMin, RoomSizeMax)
			);

			room.Size = newScale;
			instantiatedFloor.transform.localScale = newScale;
			instantiatedFloor.SetActive(true);

			room.Floor = instantiatedFloor.GetComponent<Floor>();

		}
		FloorPrefab.SetActive(true); 
	}

	private void MakePortals() {
		
		foreach (var room in MapGeneration.ResultRooms) {

			Dictionary<Room.Connection.Directions, List<Portal>> portalTable = new() {
				{ Room.Connection.Directions.Up, new() },
				{ Room.Connection.Directions.Down, new() },
				{ Room.Connection.Directions.Left, new() },
				{ Room.Connection.Directions.Right, new() }
			};
			
			foreach (var (_, connection) in room.Connections) {
				
				GameObject portal = Instantiate(PortalPrefab);

				connection.Portal = portal.GetComponent<Portal>();
				connection.Portal.ConnectedRoom = connection.Target;
				connection.Portal.StartRoom = room;
				connection.Portal.transform.rotation = Room.Connection.AngleOf(connection.Direction);
				connection.Portal.Connection = connection;

				portalTable[connection.Direction].Add(connection.Portal);

			}

			Dictionary<Room.Connection.Directions, List<Portal>> directionPortals = new() {
				{ Room.Connection.Directions.Up, new() },
				{ Room.Connection.Directions.Down, new() },
				{ Room.Connection.Directions.Left, new() },
				{ Room.Connection.Directions.Right, new() },
			};

			foreach (var (direction, portals) in portalTable) {
				foreach (var portal in portals) {
					
					Vector3 position =
						portal.StartRoom.CenterOf(direction) +
						Vector3.up * RoomFloorHeight / 2f +
						Vector3.up * PortalFloorMargin;

					portal.transform.position = position;

					directionPortals[direction].Add(portal);

				}
			}

			for (int dir = 0; dir <= (int)Room.Connection.Directions.Right; dir++) {
				
				Room.Connection.Directions direction = (Room.Connection.Directions)dir;
				List<Portal> portals = directionPortals[direction].OrderBy(portal => {
					return portal.Connection.Direction switch {
						Room.Connection.Directions.Up => portal.ConnectedRoom.Position.x,
						Room.Connection.Directions.Down => -portal.ConnectedRoom.Position.x,
						Room.Connection.Directions.Right => -portal.ConnectedRoom.Position.z,
						Room.Connection.Directions.Left => portal.ConnectedRoom.Position.z,
						_ => throw new ArgumentException()
					};
				}).ToList();


				Vector3 offset = Room.Connection.OffsetCW(direction) * PortalSpacing;
				Vector3 pointer = -offset * (portals.Count - 1) / 2f;

				foreach (var portal in portals) {
					portal.transform.position += pointer;
					pointer += offset;
				}
			}
		}

		foreach (var room in MapGeneration.ResultRooms) {
			foreach (var (nextRoom, connection) in room.Connections) {
				if (connection.Portal.PairPortal == null) {

					Portal pairPortal = nextRoom
						.Connections
						.Values
						.Select(connection => connection.Portal)
						.First(portal => portal.ConnectedRoom == room);

					connection.Portal.PairPortal = pairPortal;
					pairPortal.PairPortal = connection.Portal;

					connection.Portal.ConnectLine = Instantiate(LinePrefab);
					LineRenderer lineRenderer = connection.Portal.ConnectLine.GetComponent<LineRenderer>();
					lineRenderer.SetPositions(new Vector3[] {
						connection.Portal.transform.position,
						pairPortal.transform.position
					});

					connection.Portal.ConnectLine = lineRenderer.gameObject;
					pairPortal.ConnectLine = lineRenderer.gameObject;

				}
			}
		}
	}

	private void SpawnEnemies() {

		foreach (var room in MapGeneration.ResultRooms) {

			if (room == MapGeneration.ResultStartRoom || room == MapGeneration.ResultBossEntryRoom) continue;

			float minSize = RoomSizeMin * RoomSizeMin;
			float maxSize = RoomSizeMax * RoomSizeMax;

			float roomSize = room.Size.x * room.Size.z;

			float roomLerp = (roomSize - minSize) / (maxSize - minSize);

			float enemyRate = Mathf.Lerp(MinEnemyRate, MaxEnemyRate, roomLerp);

			for (int i = 0; i < enemyRate; i++) {

				float posX = Random.Range(-room.Size.x / 2f + RoomSizeMin * 0.2f, room.Size.x / 2f - 0.5f - RoomSizeMin * 0.2f);
				float posZ = Random.Range(-room.Size.z / 2f + RoomSizeMin * 0.2f, room.Size.z / 2f - 0.5f - RoomSizeMin * 0.2f);

				float posY = Random.Range(0f, 5f);

				GameObject enemy = Instantiate(EnemyPrefab);
				enemy.transform.position = new Vector3(posX, posY, posZ) + room.Position;

				room.Enemies.Add(enemy.GetComponent<Enemy>());

			}
		}
	}
}
