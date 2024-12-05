using UnityEngine;

public static class VectorExtensions {

	public static Vector3 XZ(this Vector2 v2, float y = 0f) => new(v2.x, y, v2.y);

	public static Vector2 ToVector2(this Vector3 v3) => new(v3.x, v3.y);
	public static Vector2 ToVector2(this Vector3Int v3i) => new(v3i.x, v3i.y);

	public static Vector2Int ToVector2Int(this Vector2 v2) => new((int)v2.x, (int)v2.y);
	public static Vector2Int ToVector2Int(this Vector3 v3) => new((int)v3.x, (int)v3.y);
	public static Vector2Int ToVector2Int(this Vector3Int v3i) => new(v3i.x, v3i.y);

	public static Vector3 ToVector3(this Vector2 v2, float z = 0f) => new(v2.x, v2.y, z);
	public static Vector3 ToVector3(this Vector2Int v2, float z = 0f) => new(v2.x, v2.y, z);

	public static Vector3Int ToVector3Int(this Vector2 v2, int z = 0) => new((int)v2.x, (int)v2.y, z);
	public static Vector3Int ToVector3Int(this Vector2Int v2i, int z = 0) => new(v2i.x, v2i.y, z);
	public static Vector3Int ToVector3Int(this Vector3 v3) => new((int)v3.x, (int)v3.y, (int)v3.z);

	public static bool IsApproximately(this Vector2 self, Vector2 other) =>
		Mathf.Approximately(self.x, other.x) &&
		Mathf.Approximately(self.y, other.y);

	public static bool IsApproximately(this Vector3 self, Vector3 other) =>
		Mathf.Approximately(self.x, other.x) &&
		Mathf.Approximately(self.y, other.y) &&
		Mathf.Approximately(self.z, other.z);

	public static bool IsXYOrderedWith(this Vector2 self, Vector2 other) =>
		self.x < other.x || (self.x == other.x && self.y < other.y);

	public static bool IsYXOrderedWith(this Vector2 self, Vector2 other) =>
		self.y < other.y || (self.y == other.y && self.x < other.x);

	public static Point ToPoint(this Vector2 self) =>
		new(self);

}