using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConvexHull : IAlgorithm, IDisplayable {

	//======================================================================| Members

	private IEnumerable<Point> resultPoints = null;

	//======================================================================| Properties

	public List<Point> Points { get; set; } = null;

	public IEnumerable<Point> ResultPoints => 
		AlgorithmNotInvokedException.ThrowIfNull(this, resultPoints);

	//======================================================================| Methods

	public void Invoke() {

		AlgorithmArgumentException.ThrowIfNull(this, nameof(Points), Points);

		Point startVertex = Points.Aggregate(
			(point1, point2) => {
				if (point1.Y < point2.Y) return point1;
				if (point1.Y == point2.Y) {
					if (point1.X < point2.X) return point1;
				}
				return point2;
			}
		);

		List<Point> vertices = Points
			.Where(point => point != startVertex)
			.ToList();

		vertices.Sort((Point left, Point right) => 
			Point.SignedCounterClockWise(startVertex, right, left)
		);

		List<Point> stack = new() {
			startVertex,
			vertices[0]
		};
		
		foreach (var next in vertices) {
			while (true) {
				
				if (stack.Count < 2) break;
				if (Point.CounterClockWise(stack[^2], stack[^1], next) > 0f) break;

				stack.RemoveAt(stack.Count - 1);

			}
			stack.Add(next);
		}
		resultPoints = stack;
	}

	public void Display(Color color, float duration = float.MaxValue) {
		DebugVisual.Draw(ResultPoints, color, duration);
		
		var resultPoints = ResultPoints.ToList();

		for (int i = 0; i < resultPoints.Count; i++) {
			Debug.DrawLine(resultPoints[i].Vecotr3, resultPoints[(i + 1) % resultPoints.Count].Vecotr3, color, duration);
		}

	}

}