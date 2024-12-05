using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RotatingCalipers : IAlgorithm, IDisplayable {

	//======================================================================| Members

	private (Point P1, Point P2)? resultPoints = null;

	//======================================================================| Properties

	public List<Point> ConvexHull { get; set; } = null;

	public (Point P1, Point P2) ResultPoints => 
		AlgorithmNotInvokedException.ThrowIfNull(this, resultPoints) ?? default;

	//======================================================================| Methods

	public void Invoke() {
		
		AlgorithmArgumentException.ThrowIfNull(this, nameof(ConvexHull), ConvexHull);

		List<Point> convexHull = ConvexHull.ToList();

		int caliperTop = 1;
		float maxDistance = 0f;

		Point maxStart = new();
		Point maxEnd = new();

		for (int caliperFloor = 0; caliperFloor < convexHull.Count; caliperFloor++) {
			int nextFloor = (caliperFloor + 1) % convexHull.Count;		
			
			while (true) {
				int nextTop = (caliperTop + 1) % convexHull.Count;

				float counterClockWiseResult = Point.ConnectedCounterClockWise(
					new(convexHull[caliperFloor], convexHull[nextFloor]),
					new(convexHull[caliperTop], convexHull[nextTop]
				));

				if (counterClockWiseResult > 0f) {
					caliperTop = nextTop;
				}
				else break;
			}

			float distance = Point.Distance(convexHull[caliperFloor], convexHull[caliperTop]);
			if (distance > maxDistance) {
				maxDistance = distance;
				maxStart = convexHull[caliperFloor];
				maxEnd = convexHull[caliperTop];
			}

		}

		resultPoints = (maxStart, maxEnd);

	}

	public void Display(Color color, float duration = float.MaxValue) {
		
		AlgorithmNotInvokedException.ThrowIfNull(this, resultPoints);
		
		DebugVisual.Draw(resultPoints?.P1 ?? new(), color, duration);
		DebugVisual.Draw(resultPoints?.P2 ?? new(), color, duration);

	}

}