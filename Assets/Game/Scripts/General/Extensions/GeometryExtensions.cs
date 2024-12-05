using System.Collections.Generic;

public static class GeometryExctensions {

	public static Triangle GetSuperTriangle(this IEnumerable<Point> self) =>
		Triangle.SuperTriangleOf(self);

}