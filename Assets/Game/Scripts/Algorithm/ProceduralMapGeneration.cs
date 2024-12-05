using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

public class ProceduralMapGeneration : IAlgorithm, IDisplayable {

	//======================================================================| Constants

	public const uint MinimumRoomCount = 5u;
	public const uint MinimumGeneration = 4u;

	//======================================================================| Members

	private readonly BinarySpaceParticipation binarySpaceParticipation = new();
	private readonly Triangulation triangulation = new();
	private readonly MinimumSpanningTree<Point, float> minimumSpanningTree = new();
	private readonly ConvexHull convexHull = new();
	private readonly RotatingCalipers rotatingCalipers = new();

	private Room resultStartRoom = null;
	private Room resultBossEntryRoom = null;
	private Room resultBossRoom = null;

	private List<Room> resultRooms = null;
	private List<Vector2> resultRoomPositions = null;
	private HashSet<UnorderedPair<Room>> resultConnections = null;
	private HashSet<Edge> resultEdges = null;

	private List<Rect> selectedSpaces;
	private WeightGraphStructure<Point> triangulatedGraph;
	private Dictionary<Vector2, Room> positionRoomTable = null;
	private List<Room> deadEndRooms = null;

	//======================================================================| Properties

	public uint RoomCount { get; set; }
	public Vector2 Size { get; set; }
	public uint Generation { get; set; }
	public float ConnectionRandomRange { get; set; }
	public uint DisplayLevel { get; set; } = 0;
	public bool DisplaySingle { get; set; } = false;

	public float PartitioningRange {
		get => binarySpaceParticipation.RandomRange;
		set => binarySpaceParticipation.RandomRange = value;
	}

	public List<Room> ResultRooms =>
		AlgorithmNotInvokedException.ThrowIfNull(this, resultRooms);

	public List<Vector2> ResultRoomPositions =>
		AlgorithmNotInvokedException.ThrowIfNull(this, resultRoomPositions);

	public Room ResultStartRoom =>
		AlgorithmNotInvokedException.ThrowIfNull(this, resultStartRoom);

	public Room ResultBossEntryRoom =>
		AlgorithmNotInvokedException.ThrowIfNull(this, resultBossEntryRoom);

	public Room ResultBossRoom =>
		AlgorithmNotInvokedException.ThrowIfNull(this, resultBossRoom);

	public HashSet<UnorderedPair<Room>> ResultConnections =>
		AlgorithmNotInvokedException.ThrowIfNull(this, resultConnections);

	public HashSet<Edge> ResultEdges =>
		AlgorithmNotInvokedException.ThrowIfNull(this, resultEdges);

	//======================================================================| Methods

	public void Invoke() {

		OutOfRangeException.ThrowIfLess(RoomCount, MinimumRoomCount, nameof(RoomCount));
		OutOfRangeException.ThrowIfLess(Generation, MinimumGeneration, nameof(Generation));
		OutOfRangeException.ThrowIfEqualOrLess(Size.x, 0, nameof(Size) + nameof(Size.x));

		InvokeBinarySpacePartitioning();
		SelectLargestRooms();
		
		InvokeTriangulation();

		InvokeMinimumSpanningTree();
		ConnectRooms();

		InvokeConvexHullThenRotatingCalipers();
		MakeSpecialRooms();
		GetDeadEndRooms();
		ConnectRandomRoom();

	}

	public void Display(Color color, float duration = float.MaxValue) {
		
		if (DisplaySingle) {
			if (DisplayLevel == 0) DebugVisual.Draw(binarySpaceParticipation, Color.green, duration);
			if (DisplayLevel == 1) DebugVisual.Draw(selectedSpaces, Color.green, duration);
			if (DisplayLevel == 2) DebugVisual.Draw(resultRoomPositions, Color.green, duration);
			if (DisplayLevel == 3) DebugVisual.Draw(triangulation.ResultTriangles, Color.blue, duration);
			if (DisplayLevel == 4) DebugVisual.Draw(minimumSpanningTree.ResultGraph, Color.red, duration);
			if (DisplayLevel == 5) DebugVisual.Draw(convexHull, Color.magenta, duration);
			if (DisplayLevel == 6) DebugVisual.Draw(resultStartRoom.CenterPosition, Color.gray, duration);
			if (DisplayLevel == 6) DebugVisual.Draw(ResultBossEntryRoom.CenterPosition, Color.yellow, duration);
			if (DisplayLevel == 7) DebugVisual.Draw(deadEndRooms.Select(r => r.CenterPosition), Color.magenta, duration);
			if (DisplayLevel == 8) DebugVisual.Draw(resultEdges, Color.green, duration);
		}
		else {
			if (DisplayLevel >= 0) DebugVisual.Draw(binarySpaceParticipation, Color.green, duration);
			if (DisplayLevel >= 1) DebugVisual.Draw(selectedSpaces, Color.green, duration);
			if (DisplayLevel >= 2) DebugVisual.Draw(resultRoomPositions, Color.green, duration);
			if (DisplayLevel >= 3) DebugVisual.Draw(triangulation.ResultTriangles, Color.blue, duration);
			if (DisplayLevel >= 5) DebugVisual.Draw(convexHull, Color.yellow, duration);
			if (DisplayLevel >= 4) DebugVisual.Draw(minimumSpanningTree.ResultGraph, Color.red, duration);
			if (DisplayLevel >= 6) DebugVisual.Draw(resultStartRoom.CenterPosition, Color.gray, duration);
			if (DisplayLevel >= 6) DebugVisual.Draw(ResultBossEntryRoom.CenterPosition, Color.yellow, duration);
			if (DisplayLevel >= 7) DebugVisual.Draw(deadEndRooms.Select(r => r.CenterPosition), Color.magenta, duration);
			if (DisplayLevel >= 8) DebugVisual.Draw(resultEdges, Color.green, duration);
		}

	}

	//======================================================================| Private Methods

	private void MakeRoom(Room room) {

		resultRooms.Add(room);
		resultRoomPositions.Add(room.CenterPosition);
		positionRoomTable[room.CenterPosition] = room;

	}

	private void ConnectRoom(Room room1, Room room2) {

		Vector2 onCenter = room2.CenterPosition - room1.CenterPosition;

		Room.Connection.Directions direction1;
		Room.Connection.Directions direction2;

		if (Mathf.Abs(onCenter.y) > Mathf.Abs(onCenter.x)) {
			if (onCenter.y > 0f) direction1 = Room.Connection.Directions.Up;
			else direction1 = Room.Connection.Directions.Down;
		}
		else {
			if (onCenter.x > 0f) direction1 = Room.Connection.Directions.Right;
			else direction1 = Room.Connection.Directions.Left;
		}

		direction2 = Room.Connection.Reverse(direction1);

		Room.Connection connection1 = new() { Target = room2, Direction = direction1 };
		Room.Connection connection2 = new() { Target = room1, Direction = direction2 };

		room1.Connections[room2] = connection1;
		room2.Connections[room1] = connection2;

		resultConnections.Add(new(room1, room2));
		resultEdges.Add(new(room1.CenterPosition, room2.CenterPosition));

	}

	private void InvokeBinarySpacePartitioning() {
 		binarySpaceParticipation.Generation = Generation;
		binarySpaceParticipation.Size = Size;
		binarySpaceParticipation.Invoke();
	}

	private void SelectLargestRooms() {

		List<Rect> allSpaces = binarySpaceParticipation.ResultRects.ToList();
		List<Rect> selectedSpaces;

		if (RoomCount > allSpaces.Count) {
			Debug.LogWarning(
				$"Room count is larger then all rooms' count. " +
				$"The max amount of room in generation {Generation} is {allSpaces.Count}. " +
				$"Room amount will clamp to {allSpaces.Count}"
			);

			selectedSpaces = allSpaces;
			RoomCount = (uint)allSpaces.Count;
		}
		else {
			selectedSpaces = allSpaces
				.OrderByDescending(rect => rect.width * rect.height)
				.ToList()
				.GetRange(0, (int)RoomCount);
		}

		resultRoomPositions = selectedSpaces.Select(space => space.center).ToList();
		this.selectedSpaces = selectedSpaces;

		resultRooms = new();
		positionRoomTable = new();

		foreach (var position in resultRoomPositions) {
			Room newRoom = new(position);
			resultRooms.Add(newRoom);
			positionRoomTable[position] = newRoom;
		}
	}

	private void InvokeTriangulation() {
 		triangulation.Points = resultRoomPositions.Select(position => position.ToPoint()).ToList();
		triangulation.Invoke();

		triangulatedGraph = WeightGraphStructure<Point>.GetEulerWeightGraph(triangulation.ResultEdges);
	}

	private void InvokeMinimumSpanningTree() {
 		minimumSpanningTree.TargetGraph = triangulatedGraph;
		minimumSpanningTree.Invoke();
	}

	private void ConnectRooms() {

 		resultConnections = new();
		resultEdges = new();

		foreach (var room in resultRooms) {
			var otherRooms = minimumSpanningTree
				.ResultGraph
				.ConnectionOf(room.CenterPosition)
				.Select(connection => positionRoomTable[connection.Target])
				.ToHashSet();

			foreach (var other in otherRooms) {
				ConnectRoom(room, other);
			}
		}
	}

	private void InvokeConvexHullThenRotatingCalipers() {

 		convexHull.Points = resultRoomPositions.Select(position => position.ToPoint()).ToList();
		convexHull.Invoke();

		rotatingCalipers.ConvexHull = convexHull.ResultPoints.ToList();
		rotatingCalipers.Invoke(); 

	}

	private void MakeSpecialRooms() {

 		var (startRoomPosition, bossEntryRoomPosition) = Random.Range(0, 1) == 0
			? (rotatingCalipers.ResultPoints.P1, rotatingCalipers.ResultPoints.P2)
			: (rotatingCalipers.ResultPoints.P2, rotatingCalipers.ResultPoints.P1);

		resultStartRoom = positionRoomTable[startRoomPosition];
		resultBossEntryRoom = positionRoomTable[bossEntryRoomPosition];

		resultStartRoom.RoomType = Room.Type.Start;
		resultBossEntryRoom.RoomType = Room.Type.BossEntry;

	}

	private void GetDeadEndRooms() {
		deadEndRooms = resultRooms
			.Where(room => 
				room != resultStartRoom &&
				room != resultBossEntryRoom &&
				room != resultBossRoom
			)
			.Where(room => room.Connections.Count == 1)
			.ToList();
	}

	private void ConnectRandomRoom() {

 		foreach(var room in deadEndRooms) {

			if (Random.Range(0f, 1f) > ConnectionRandomRange) continue;
			
			var otherRooms = resultRooms.Except(new[] { room }).Select(room => room.CenterPosition.ToPoint());

			NinetyDegree ninetyDegree = new() {
				TargetPoint = room.CenterPosition,
				PreviousPoint = room.Connections.First().Key.CenterPosition,
				OtherPoints = otherRooms.ToList()
			};
			ninetyDegree.Invoke();

			if (ninetyDegree.ResultPoints.Count() == 0) continue;

			var connectionRoomPosition = ninetyDegree
				.ResultPoints
				.Aggregate(
					(point1, point2) => {

						float distance1 = point1.DistanceTo(room.CenterPosition);
						float distance2 = point2.DistanceTo(room.CenterPosition);

						return distance1 < distance2 ? point1 : point2;

					}
				);

			var connectionRoom = positionRoomTable[connectionRoomPosition];

			ConnectRoom(room, connectionRoom);
		}
	}

}