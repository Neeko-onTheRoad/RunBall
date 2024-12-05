using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Triangle : IDisplayable, IEquatable<Triangle> {

	//======================================================================| Members

	private Circle circumCircle = null;

	//======================================================================| Properties

	public Point Point1 { get; }
	public Point Point2 { get; }
	public Point Point3 { get; }

	public Edge Edge1 { get; }
	public Edge Edge2 { get; }
	public Edge Edge3 { get; }

	public readonly List<Point> Points;
	public readonly List<Edge> Edges;

	public Circle CircumCircle {
		get {
			if (circumCircle is not null) return circumCircle;

			float determinant = 2f * (
				(Point1.X * (Point2.Y - Point3.Y)) +
				(Point2.X * (Point3.Y - Point1.Y)) +
				(Point3.X * (Point1.Y - Point2.Y))
			);

			float centerX = (
				(Mathf.Pow(Point1.X, 2f) + Mathf.Pow(Point1.Y, 2f)) * (Point2.Y - Point3.Y) + 
				(Mathf.Pow(Point2.X, 2f) + Mathf.Pow(Point2.Y, 2f)) * (Point3.Y - Point1.Y) +
				(Mathf.Pow(Point3.X, 2f) + Mathf.Pow(Point3.Y, 2f)) * (Point1.Y - Point2.Y)
			) / determinant;

			float centerY = (
				(Mathf.Pow(Point1.X, 2f) + Mathf.Pow(Point1.Y, 2f)) * (Point3.X - Point2.X) + 
				(Mathf.Pow(Point2.X, 2f) + Mathf.Pow(Point2.Y, 2f)) * (Point1.X - Point3.X) +
				(Mathf.Pow(Point3.X, 2f) + Mathf.Pow(Point3.Y, 2f)) * (Point2.X - Point1.X)
			) / determinant;

			Point centerPoint = new(centerX, centerY);
			float radius = centerPoint.DistanceTo(Point1);

			circumCircle = new Circle(centerPoint, radius);
			return circumCircle;
		}
	}

	//======================================================================| Constructors

	public Triangle(Point point1, Point point2, Point point3) {

		Point1 = point1;
		Point2 = point2;
		Point3 = point3;
		Points = new() { Point1, Point2, Point3 };

		Edge1 = new(Point1, Point2);
		Edge2 = new(Point2, Point3);
		Edge3 = new(Point3, Point1);
		Edges = new() { Edge1, Edge2, Edge3 };

	}

	//======================================================================| Methods

	public override bool Equals(object other) {
		if (other is Triangle triangle) return Equals(triangle);
		return false;
	}

	public bool Equals(Triangle other) =>
		Points.ToHashSet().SetEquals(other.Points);

	public override int GetHashCode() {

		var orderedPoints = Points
			.OrderBy(point => point.X)
			.ThenBy(point => point.Y)
			.ToList();

		return HashCode.Combine(
			orderedPoints[0],
			orderedPoints[1],
			orderedPoints[2]
		);

	}

	public void Display(Color color, float duration = float.MaxValue) {
		foreach (var edge in Edges) {
			edge.Display(color, duration);
		}
	}

	public bool ContainsPoint(Point point) =>
		Points.Contains(point);

	public bool ContainsSamePointWith(Triangle other) {
		foreach (var point in other.Points) {
			if (Points.Contains(point)) return true;
		}
		return false;
	}

	public bool ContainsEdge(Edge edge) =>
		Edges.Contains(edge);

	//======================================================================| Static Methods

	public static Triangle SuperTriangleOf(IEnumerable<Point> points) {
		
		Rect range = new() {
			xMin = float.MaxValue,
			yMin = float.MaxValue,
			xMax = float.MinValue,
			yMax = float.MinValue
		};

		foreach (var point in points) {
			
			if (range.xMin > point.X) range.xMin = point.X;
			if (range.yMin > point.Y) range.yMin = point.Y;
			if (range.xMax < point.X) range.xMax = point.X;
			if (range.yMax < point.Y) range.yMax = point.Y;

		}

		float dy = range.height;
		float dx = range.width;

		Point p1 = new(range.xMin - dx, range.yMin - dy * 2);
		Point p2 = new((range.xMin + range.xMax) / 2, range.yMax + dy * 2);
		Point p3 = new(range.xMax + dx, range.yMin - dy * 2);

		return new(p1, p2, p3);

	}

	//======================================================================| Operators

	public static bool operator == (Triangle left, Triangle right) => left.Equals(right);
	public static bool operator != (Triangle left, Triangle right) => !left.Equals(right);

}