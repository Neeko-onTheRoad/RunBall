using System;
using UnityEngine;

public class Circle : IDisplayable, IEquatable<Circle> {

	//======================================================================| Properties

	public Point MidPoint { get; }
	public float Radius { get; }

	public uint DisplayResolution { get; set; } = 32u;

	//======================================================================| Constructors

	public Circle(Point midPoint, float radius) {
		MidPoint = midPoint;
		Radius = radius;
	}

	//======================================================================| Methods

	public override bool Equals(object other) {
		if (other is Circle circle) return Equals(circle);
		return false;
	}

	public bool Equals(Circle other) =>
		other.MidPoint.Equals(MidPoint) &&
		other.Radius.Approximately(Radius);

	public override int GetHashCode() =>
		HashCode.Combine(MidPoint, Radius);

	public override string ToString() => 
		$"Circle: [ MidPoint: {MidPoint}, Radius: {Radius} ]";

	public bool ContainsPoint(Point point) =>
		MidPoint.DistanceTo(point) <= Radius;

	public void Display(Color color, float duration) =>
		DebugVisual.DrawCircle(MidPoint, Radius, color, duration, DisplayResolution);

	//======================================================================| Static Methods

	public static Circle CircumCircleOf(Triangle triangle) =>
		triangle.CircumCircle;

	//======================================================================| Operators

	public static bool operator == (Circle left, Circle right) => left.Equals(right);
	public static bool operator != (Circle left, Circle right) => !left.Equals(right);

}