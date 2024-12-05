using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WeightGraphStructure<ElementT> : WeightGraphStructure<ElementT, float> where ElementT : notnull {};
public class WeightGraphStructure<ElementT, WeightT>
	where ElementT : notnull
	where WeightT : notnull
{

	public struct Connection {
		
		//==================================================| Members
		
		public ElementT Target;
		public WeightT Weight;

		//==================================================| Constructors

		public Connection(ElementT target, WeightT weight) {
			Target = target;
			Weight = weight;
		}

	}

	public struct NodeConnectionInfo {

		//==================================================| Members

		public ElementT Start;
		public IEnumerable<Connection> Connections;

		//==================================================| Constructors

		public NodeConnectionInfo(ElementT start, IEnumerable<Connection> connections) {
			Start = start;
			Connections = connections;
		}
		
	}

	//======================================================================| Members

	private readonly HashSet<ElementT> nodes = new();
	private readonly Dictionary<ElementT, HashSet<Connection>> edges = new();

	//======================================================================| Properties

	public int Count => nodes.Count;
	public bool IsEmpty => Count == 0;

	public IEnumerable<ElementT> Nodes => nodes;

	public IEnumerable<NodeConnectionInfo> NodeConnections =>
		nodes.Select(node => new NodeConnectionInfo(node, edges[node]));


	//======================================================================| Methods

	public void AddNode(ElementT node) {
		if (!nodes.Contains(node)) nodes.Add(node);
		if(!edges.ContainsKey(node)) edges[node] = new();
	}

	public void AddEdge(ElementT n1, ElementT n2, WeightT weight) {
					
		AddNode(n1);
		AddNode(n2);

		edges[n1].Add(new(n2, weight));
		edges[n2].Add(new(n1, weight));

	}

	public IEnumerator<ElementT> GetEnumerator() =>
		nodes.GetEnumerator();

	public List<Connection> ConnectionOf(ElementT node) =>
		edges[node].ToList();

	public void Foreach(Action<ElementT> action) {
		foreach (var i in this) action(i);
	}

	public bool ContainsNode(ElementT node) =>
		nodes.Contains(node);

	//======================================================================| Static Methods

	public static WeightGraphStructure<Point> GetEulerWeightGraph(IEnumerable<Edge> edges) {

		WeightGraphStructure<Point> result = new();
		Dictionary<Point, Point> nodes = new();

		foreach (var edge in edges) {
			if (!nodes.ContainsKey(edge.Start)) nodes[edge.Start] = new(edge.Start);
			if (!nodes.ContainsKey(edge.End)) nodes[edge.End] = new(edge.End);
		}

		foreach (var edge in edges) {
			result.AddEdge(
				nodes[edge.Start],
				nodes[edge.End],
				edge.Length
			);
		}

		return result;
	}

}