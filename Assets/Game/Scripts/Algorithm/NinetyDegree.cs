using System.Collections.Generic;
using System.Linq;

public class NinetyDegree : IAlgorithm {

	//======================================================================| Members

	private IEnumerable<Point> resultPoints = null;

	//======================================================================| Properties

	public Point TargetPoint { get; set; } = null;
	public Point PreviousPoint { get; set; } = null;
	public List<Point> OtherPoints { get; set; } = null;
	public List<Point> ExceptPoints { get; set; } = new();

	public IEnumerable<Point> ResultPoints =>
		AlgorithmNotInvokedException.ThrowIfNull(this, resultPoints);

	//======================================================================| Methods

	public void Invoke() {
	
		HashSet<Point> possiblePoints = OtherPoints
			.Except(ExceptPoints)
			.ToHashSet();

		if (TargetPoint.X.Approximately(PreviousPoint.X)) {
			if (TargetPoint.Y < PreviousPoint.Y) {
				resultPoints = possiblePoints.Where(point => point.X <= TargetPoint.X);
				return;
			}
			else {
				resultPoints = possiblePoints.Where(point => point.X >= TargetPoint.X);
				return;
			}
		}

		if (TargetPoint.Y.Approximately(PreviousPoint.Y)) {
			if (TargetPoint.X < PreviousPoint.X) {
				resultPoints = possiblePoints.Where(point => point.Y <= TargetPoint.Y);
				return;
			}
			else {
				resultPoints = possiblePoints.Where(point => point.Y >= TargetPoint.Y);
			}
		}

		float slope = (TargetPoint.Y - PreviousPoint.Y) / (TargetPoint.X - PreviousPoint.X);

		float reverseSlope = -1 / slope;
		float reverseIntercept = TargetPoint.Y - reverseSlope * TargetPoint.X;

		List<(float Result, Point Point)> selectedPoints = possiblePoints
			.Select(point => point.X * reverseSlope - point.Y + reverseIntercept)
			.Zip(possiblePoints, (result, point) => (result, point))
			.ToList();

		if (PreviousPoint.Y > TargetPoint.Y) {
			resultPoints = selectedPoints
				.Where(pair => pair.Result >= 0f)
				.Select(pair => pair.Point);

			return;
		}
		else {
			resultPoints = selectedPoints
				.Where(pair => pair.Result <= 0f)
				.Select(pair => pair.Point);

			return;
		}

	}

}