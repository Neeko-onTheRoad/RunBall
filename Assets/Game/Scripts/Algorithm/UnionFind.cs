using System;
using System.Collections.Generic;

public class UnionFind<ElementT, WeightT> : IAlgorithm 
	where ElementT : notnull
	where WeightT : notnull
{

	//======================================================================| Members

	private bool? result = null;
	private Dictionary<ElementT, ElementT> parent = new();

	//======================================================================| Properties

	public WeightGraphStructure<ElementT, WeightT> TargetGraph { get; set; }

	public bool Result => 
		AlgorithmNotInvokedException.ThrowIfNull(this, result) ?? default;

	//======================================================================| Methods

	public void Invoke() {

		AlgorithmArgumentException.ThrowIfNull(this, nameof(TargetGraph), TargetGraph);
		
		foreach (var node in TargetGraph.Nodes) {
			parent[node] = node;
		}

		foreach (var node in TargetGraph.Nodes) {
			foreach (var connected in TargetGraph.ConnectionOf(node)) {
				
				if (Find(node).Equals(Find(connected.Target))) {
					result = false;
					return;
				}

			}
		}

		result = true;

	}

	//======================================================================| Private Methods

	private ElementT Find(ElementT node) {
		
		if (parent[node].Equals(node)) return node;

		parent[node] = Find(node);
		return parent[node];

	}
}