using System;
using System.Collections.Generic;
using UnityEngine;

public class Room {

	//======================================================================| Types

	public enum Type {
		Normal,
		Start,
		BossEntry,
		Boss,
		Chest,
		Shop
	}

	public class Connection {
		public enum Directions {
			Up, Down, Left, Right
		}

		public Room Target { get; set; }
		public Directions Direction { get; set; }
		public Portal Portal { get; set; }

		public static Directions Reverse(Directions direction) {
			return direction switch {
				Directions.Up => Directions.Down,
				Directions.Down => Directions.Up,
				Directions.Left => Directions.Right,
				Directions.Right => Directions.Left,
				_ => throw new ArgumentException()
			};
		}

		public static Quaternion AngleOf(Directions direction) {
			return Quaternion.Euler(0f, direction switch {
				Directions.Up => 0f,
				Directions.Right => 90f,
				Directions.Down => 180f,
				Directions.Left => 270f,
				_ => throw new ArgumentException()
			}, 0f);
		}

		public static Vector3 OffsetCW(Directions direction) {
			return direction switch {
				Directions.Up => new(1f, 0f, 0f),
				Directions.Down => new(-1f, 0f, 0f),
				Directions.Right => new(0f, 0f, -1f),
				Directions.Left => new(0f, 0f, 1f),
				_ => throw new ArgumentException()
			};
		}
	}

	//======================================================================| Properties

	public Floor Floor { get; set; }
	public Type RoomType { get; set; } = Type.Normal;
	public Vector2 CenterPosition { get; set; } = new();
	public float Height { get; set; } = 0;
	public Vector3 Size { get; set; } = new();
	public Vector3 Position => new(CenterPosition.x, Height + Size.y / 2f, CenterPosition.y);
	public bool PlayerDetected => Floor.PlayerDetected;
	public bool PreviousDectected { get; set; } = false;

	public List<Enemy> Enemies { get; set; } = new();

	public Dictionary<Room, Connection> Connections { get; set; } = new();

	//======================================================================| Constructors

	public Room(Vector2 position) => CenterPosition = position;

	//======================================================================| Methods

	public void AddConnection(Connection connection) {
		Connections[connection.Target] = connection;
	}

	public Vector3 CenterOf(Connection.Directions direction) {
		return new Vector3(CenterPosition.x, Height, CenterPosition.y) + direction switch {
			Connection.Directions.Up => new(0f, 0f, Size.z / 2f),
			Connection.Directions.Down => new(0f, 0f, -Size.z / 2f),
			Connection.Directions.Right => new(Size.x /  2f, 0f, 0f),
			Connection.Directions.Left => new(-Size.x / 2f, 0f, 0f),
			_ => throw new ArgumentException()
		};
	}
}