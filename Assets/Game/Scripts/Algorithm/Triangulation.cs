using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Triangulation : IAlgorithm, IDisplayable {

	//======================================================================| Members

	private IEnumerable<Triangle> resultTriangles = null;
	private Triangle resultSuperTriangle = null;
	private IEnumerable<Edge> resultEdges = null;

	//======================================================================| Properties

	public List<Point> Points { get; set; } = null;

	public IEnumerable<Triangle> ResultTriangles => 
		AlgorithmNotInvokedException.ThrowIfNull(this, resultTriangles);

	public Triangle ResultSuperTriangle => 
		AlgorithmNotInvokedException.ThrowIfNull(this, resultSuperTriangle);

	public IEnumerable<Edge> ResultEdges =>
		AlgorithmNotInvokedException.ThrowIfNull(this, resultEdges);

	//======================================================================| Methods

	public void Invoke() {
		
		AlgorithmArgumentException.ThrowIfNull(this, nameof(Points), Points);

		Triangle superTriangle = Triangle.SuperTriangleOf(Points);
		HashSet<Triangle> triangulation = new() {
			superTriangle
		};

		foreach (Point point in Points) {

			HashSet<Triangle> badTriangles = new();

			foreach (Triangle triangle in triangulation) {
				if (triangle.CircumCircle.ContainsPoint(point)) {
					badTriangles.Add(triangle);
				}
			}

			HashSet<Edge> boundaryEdges = new();

			foreach (Triangle badTriangle in badTriangles) {
				foreach (Edge edge in badTriangle.Edges) {
					if (boundaryEdges.Contains(edge)) {
						boundaryEdges.Remove(edge);
					} else {
						boundaryEdges.Add(edge);
					}
				}
			}

			foreach (Triangle badTriangle in badTriangles) {
				triangulation.Remove(badTriangle);
			}

			foreach (Edge edge in boundaryEdges) {
				Triangle newTriangle = new(edge.Start, edge.End, point);
				triangulation.Add(newTriangle);
			}
		}

		triangulation.RemoveWhere(triangle => 
			triangle.Points.Any(vertex => superTriangle.Points.Contains(vertex))
		);


		resultTriangles = triangulation;
		resultSuperTriangle = superTriangle;

		HashSet<Edge> edges = new();
		foreach (var triangle in resultTriangles) {
			foreach (var edge in triangle.Edges) edges.Add(edge); 
		}

		resultEdges = edges;

	}

	public void Display(Color color, float duration = float.MaxValue) {
		AlgorithmNotInvokedException.ThrowIfNull(this, ResultTriangles);
		DebugVisual.Draw(ResultTriangles, color, duration);
	}

}