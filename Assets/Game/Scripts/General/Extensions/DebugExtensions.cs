using System.Collections.Generic;
using UnityEngine;

public interface IDisplayable {

	public void Display(float duration = float.MaxValue) => Display(Color.white, duration);
	public void Display(Color color, float duration = float.MaxValue);

}

public static class DebugVisual {

	public static void Draw(Rect rect, float duration = float.MaxValue) => Draw(rect, Color.white, duration);
	public static void Draw(Rect rect, Color color, float duration = float.MaxValue) {

		Vector2[] vertexs = {
			new(rect.x, rect.y),
			new(rect.x + rect.width, rect.y),
			new(rect.x + rect.width, rect.y + rect.height),
			new(rect.x, rect.y + rect.height)
		};

		for (int i = 0; i < 4; i++) {
			Debug.DrawLine(vertexs[i], vertexs[(i + 1) % 4], color, duration);
		}

	}

	public static void Draw(IEnumerable<Rect> rects, float duration = float.MaxValue) => Draw(rects, Color.white, duration);
	public static void Draw(IEnumerable<Rect> rects, Color color, float duration = float.MaxValue) {
		foreach (var rect in rects) {
			Draw(rect, color, duration);
		}
	}

	public static void Draw(Vector2 position, float duration = float.MaxValue, float size = 0.025f) => Draw(position, Color.white, duration, size);
	public static void Draw(Vector2 position, Color color, float duration = float.MaxValue, float size = 0.025f) {
		DrawCircle(position, size, color, duration, 16);
	}

	public static void Draw(IEnumerable<Vector2> positions, float duration = float.MaxValue, float size = 0.025f) => Draw(positions, Color.white, duration, size);
	public static void Draw(IEnumerable<Vector2> positions, Color color, float duration = float.MaxValue, float size = 0.025f) {
		foreach (var position in positions) {
			Draw(position, color, duration, size);
		}
	}

	public static void DrawCircle(Vector2 position, float radius, float duration = float.MaxValue, uint resolution = 12) => DrawCircle(position, radius, Color.white, duration, resolution);
	public static void DrawCircle(Vector2 position, float radius, Color color, float duration = float.MaxValue, uint resolution = 12) {

		List<Vector2> vertexs = new();

		for (int i = 0; i < resolution; i++) {

			Vector2 vertex = new(
				radius * Mathf.Cos(2 * Mathf.PI * i / resolution),
				radius * Mathf.Sin(2 * Mathf.PI * i / resolution)
			);

			vertexs.Add(vertex + position);
		}

		for (int i = 0; i < resolution; i++) {
			Debug.DrawLine(vertexs[i], vertexs[(int)((i + 1) % resolution)], color, duration);
		}

	}

	public static void Draw(IDisplayable geometry, float duration = float.MaxValue) => Draw(geometry, Color.white, duration);
	public static void Draw(IDisplayable geometry, Color color, float duration = float.MaxValue) =>
		geometry.Display(color, duration);

	public static void Draw(IEnumerable<IDisplayable> geometries, float duration = float.MaxValue) => Draw(geometries, Color.white, duration);
	public static void Draw(IEnumerable<IDisplayable> geometries, Color color, float duration = float.MaxValue) {
		foreach (var geometry in geometries) {
			geometry.Display(color, duration);
		}
	}

	public static void Draw(WeightGraphStructure<Point, float> eulerGraph, float duration = float.MaxValue) => Draw(eulerGraph, Color.white, duration);
	public static void Draw(WeightGraphStructure<Point, float> eulerGraph, Color color, float duration = float.MaxValue) {
		foreach (var nodeConnection in eulerGraph.NodeConnections) {
			foreach (var connection in nodeConnection.Connections) {
				Debug.DrawLine(nodeConnection.Start.Vecotr3, connection.Target.Vecotr3, color, duration);
			}
		}
	}

}