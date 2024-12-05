using System;
using UnityEngine;

public class Edge : IDisplayable, IEquatable<Edge> {

	//======================================================================| Properties

	public Point Start { get; }
 	public Point End { get; }

	public Edge PointSwaped => new(End, Start);

	public bool IsXYOrdered => Start.IsXYOrderedWith(End);
	public bool IsYXOrdered => Start.IsYXOrderedWith(End);

	public Edge XYOrdered => IsXYOrdered ? this : PointSwaped;
	public Edge YXOrdered => IsYXOrdered ? this : PointSwaped;

	public float Length => Point.Distance(Start, End);
	
	//======================================================================| Constructors

	public Edge(Edge clone) {
		Start = new(clone.Start);
		End = new(clone.End);
	}

	public Edge(Point start, Point end) {
		Start = start;
		End = end;
	}

	//======================================================================| Methods

	public override bool Equals(object other) =>
		other is Edge edge && Equals(edge);

	public bool Equals(Edge other) =>
		(Start == other.Start && End == other.End) ||
		(Start == other.End && End == other.Start);

	public override int GetHashCode() {
		
		int startHash = Start.GetHashCode();
		int endHash = End.GetHashCode();

		(int hashLarge, int hashLess) = startHash > endHash
			? (startHash, endHash)
			: (endHash, startHash);

		return HashCode.Combine(hashLarge, hashLess);
	}

	public override string ToString() => 
		$"Edge: [ Start: {Start}, End: {End} ]";

	public bool IsIntersectWith(Edge other) =>
		IsIntersectWith(this, other);

	public void Display(Color color, float duration = float.MaxValue) =>
		Debug.DrawLine(Start.Position, End.Position, color, duration);

	public Edge Move(Vector2 offset) =>
		new(Start + (Point)offset, End + (Point)offset);

	//======================================================================| Static Methods
	
	public static bool IsIntersectWith(Edge edge1, Edge edge2) {

		int leftCCW =
			Point.SignedCounterClockWise(edge1.Start, edge1.End, edge2.Start) *
			Point.SignedCounterClockWise(edge1.Start, edge1.End, edge2.End);

		int rightCCW =
			Point.SignedCounterClockWise(edge2.Start, edge2.End, edge1.Start) *
			Point.SignedCounterClockWise(edge2.Start, edge2.End, edge1.End);

		if (leftCCW == 0 && rightCCW == 0) {
			
			edge1 = edge1.XYOrdered;
			edge2 = edge2.XYOrdered;

			Func<Vector2, float> startEnd = 
				(edge1.Start.X == edge1.End.X)
				? vector => vector.y
				: vector => vector.x;
	
			float leftStart = startEnd(edge1.Start);
			float leftEnd = startEnd(edge1.End);
			float rightStart = startEnd(edge2.Start);
			float rightEnd = startEnd(edge2.End);

			return rightStart <= leftEnd && leftStart <= rightEnd;
		}

		return leftCCW <= 0 && rightCCW <= 0;
	}

	//======================================================================| Operators

	public static bool operator == (Edge left, Edge right) => left.Equals(right);
	public static bool operator != (Edge left, Edge right) => !left.Equals(right);

	public static Edge operator + (Edge edge, Vector2 offset) => edge.Move(offset);

}