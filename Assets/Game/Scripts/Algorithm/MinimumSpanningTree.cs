using System;
using System.Collections.Generic;

public class MinimumSpanningTree<ElementT, WeightT> : IAlgorithm
	where ElementT : notnull
	where WeightT : notnull, IComparable
{

	//======================================================================| Types

	public struct Connection {

		//==================================================| Properties

		public UnorderedPair<ElementT> Targets { get; set; }
		public WeightT Weight { get; set; }

		//==================================================| Constructors

		public Connection(UnorderedPair<ElementT> targets, WeightT weight) {
			Targets = targets;
			Weight = weight;
		}

		//==================================================| Methods

		public override readonly int GetHashCode() => 
			HashCode.Combine(Targets, Weight);

	}

	private class ComparerBase : IComparer<Connection> {

		//==================================================| Properties

		public IComparer<WeightT> WeightComparer { get; set; }

		//==================================================| Constructors

		public ComparerBase(IComparer<WeightT> comparer = null) {
			WeightComparer = comparer ??
				Comparer<WeightT>.Create((element1, element2) =>
					element1.CompareTo(element2)
				);
		}

		//==================================================| Methods

		public int Compare(Connection connection1, Connection connection2) {
			
			int weightCompare = WeightComparer.Compare(connection1.Weight, connection2.Weight);
			
			if (weightCompare == 0) return connection1.Targets.Equals(connection2.Targets) ? 0 : 1;
			return weightCompare;

		}
	}

	//======================================================================| Members

	private WeightGraphStructure<ElementT, WeightT> resultGraph = null;

	//======================================================================| Properties

	public IComparer<WeightT> Comparer { get; set; } = null;
	public WeightGraphStructure<ElementT, WeightT> TargetGraph { get; set; } = null;

	public WeightGraphStructure<ElementT, WeightT> ResultGraph  => 
		AlgorithmNotInvokedException.ThrowIfNull(this, resultGraph);

	//======================================================================| Methods

	public void Invoke() {

		AlgorithmArgumentException.ThrowIfNull(this, nameof(TargetGraph), TargetGraph);

		SortedSet<Connection> connections = new(new ComparerBase(Comparer));
		
		foreach (var nodeConnection in TargetGraph.NodeConnections) {
			foreach (var other in nodeConnection.Connections) {
				UnorderedPair<ElementT> connectionPair = new(
					nodeConnection.Start,
					other.Target
				);
				connections.Add(new Connection(connectionPair, other.Weight));
			}
		}

		Dictionary<ElementT, ElementT> parent = new();
		foreach (var node in TargetGraph.Nodes) {
			parent[node] = node;
		}

		ElementT Find(ElementT node) {
			if (!parent[node].Equals(node)) {
				parent[node] = Find(parent[node]);
			}
			return parent[node];
		}

		void Union(ElementT node1, ElementT node2) {
			ElementT root1 = Find(node1);
			ElementT root2 = Find(node2);

			if (!root1.Equals(root2)) {
				parent[root2] = root1;
			}
		}

		resultGraph = new();
		foreach (var node in TargetGraph.Nodes) {
			resultGraph.AddNode(node);
		}

		foreach (var connection in connections) {
			ElementT node1 = connection.Targets.Item1;
			ElementT node2 = connection.Targets.Item2;

			if (!Find(node1).Equals(Find(node2))) {
				resultGraph.AddEdge(node1, node2, connection.Weight);
				Union(node1, node2);
			}
		}

	}


}