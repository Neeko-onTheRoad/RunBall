using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Point : IDisplayable, IEquatable<Point> {

	//======================================================================| Properties

	public Vector2 Position { get; }

	public float RenderSize { get; set; } = 0.025f;

	public float X => Position.x;
	public float Y => Position.y;

	public float Magnitude2 => X * X + Y * Y;
	public float Magnitude => Mathf.Sqrt(Magnitude2);
	public Point Normalized {
		get {
			float magnitude = Magnitude;
			return new Vector2(X / magnitude, Y / magnitude);
		}
	}

	public Vector3 Vecotr3 => new (X, Y, 0f);

	//======================================================================| Constructors

	public Point() {
		Position = new();
	}

	public Point(Point clone) {
		Position = clone.Position;
	}

	public Point(float x, float y) {
		Position = new(x, y);
	}

	public Point(Vector2 position) {
		Position = position;
	}

	//======================================================================| Methods

	public override bool Equals(object other) {

		if (other is Point point) return Equals(point);
		if (other is Vector2 vector2) return Equals(vector2);
		return false;
	}

	public bool Equals(Point other) =>
		other.Position.IsApproximately(Position);

	public override int GetHashCode() =>
		Position.GetHashCode();

	public override string ToString() => 
		$"({X}, {Y})";

	public float DistanceTo(Point other) =>
		Distance(this, other);

	public bool IsXYOrderedWith(Point other) =>
		Position.IsXYOrderedWith(other);

	public bool IsYXOrderedWith(Point other) =>
		Position.IsYXOrderedWith(other);

	public bool IsInCircle(Circle circle) =>
		circle.ContainsPoint(this);

	public void Display(Color color, float duration = float.MaxValue) =>
		DebugVisual.Draw(Position, color, duration, RenderSize);

	//======================================================================| Static Methods

	public static float CounterClockWise(Point start, Point center, Point end) =>
		(center.X - start.X) * (end.Y - start.Y) - 
		(center.Y - start.Y) * (end.X - start.X);

	public static float ConnectedCounterClockWise(Edge floor, Edge top) {
		Point endPoint = top.End - (top.Start - floor.End);
		return CounterClockWise(floor.Start, floor.End, endPoint);
	}

	public static int SignedCounterClockWise(Point start, Point center, Point end) =>
		CounterClockWise(start, center, end).Sign();

	public static float Distance(Point point1, Point point2) =>
		Vector2.Distance(point1, point2);

	public static Point Reverse(Point center, Point other) =>
		center * 2 - other;
		
	public static Point CenterOfGravity(IEnumerable<Point> points) {

		Vector2 result = new();
		foreach (var point in points) result += point.Position;

		return result / points.Count();

	}

	//======================================================================| Operators

	public static bool operator == (Point left, Point right) => left.Equals(right);
	public static bool operator != (Point left, Point right) => !left.Equals(right);

	public static bool operator == (Point left, Vector2 right) => left.Equals(right);
	public static bool operator != (Point left, Vector2 right) => !left.Equals(right);

	public static bool operator == (Vector2 left, Point right) => left.Equals(right);
	public static bool operator != (Vector2 left, Point right) => !left.Equals(right);


	public static Point operator + (Point left, Point right) => left.Position + right.Position;
	public static Point operator - (Point left, Point right) => left.Position - right.Position;
	public static Point operator * (Point left, Point right) => left.Position * right.Position;
	public static Point operator / (Point left, Point right) => left.Position / right.Position;

	public static Point operator * (Point point, float mlt) => point.Position / mlt;
	public static Point operator / (Point point, float div) => point.Position / div;

	//======================================================================| Casters

	public static implicit operator Vector2(Point point) => point.Position;
	public static implicit operator Point(Vector2 vector2) => new(vector2);

}