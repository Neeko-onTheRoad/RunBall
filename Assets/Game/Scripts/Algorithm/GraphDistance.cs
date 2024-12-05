using System;
using System.Collections.Generic;

public class GraphDistance<ElementT, WeightT> : IAlgorithm
	where ElementT : notnull
	where WeightT : notnull
{

	//======================================================================| Member

	public Dictionary<ElementT, uint> resultDistance = null;
	public HashSet<ElementT> visited = null;

	//======================================================================| Properties

	public WeightGraphStructure<ElementT, WeightT> TargetGraph { get; set; }
	public ElementT StartNode { get; set; }

	public Dictionary<ElementT, uint> ResultDistance => 
		AlgorithmNotInvokedException.ThrowIfNull(this, resultDistance);

	//======================================================================| Methods

	public void Invoke() {
		
		AlgorithmArgumentException.ThrowIfNull(this, nameof(TargetGraph), TargetGraph);
		AlgorithmArgumentException.ThrowIfNull(this, nameof(StartNode), StartNode);

		if (!TargetGraph.ContainsNode(StartNode)) {
			throw new InvalidOperationException(
				"There are no matching node with StartNode in TargetGraph. " +
				"Please assign StartNode with node in graph."
			);
		}

		visited = new() { StartNode };
		resultDistance = new() { { StartNode, 0 } };

		Queue<ElementT> queue = new();
		queue.Enqueue(StartNode);

		while (queue.Count != 0) {
			
			ElementT current = queue.Dequeue();

			foreach (var next in TargetGraph.ConnectionOf(current)) {
				if (!visited.Contains(next.Target)) {
					visited.Add(next.Target);
					queue.Enqueue(next.Target);
					resultDistance[next.Target] = resultDistance[current] + 1;
				}
			}
		}
	}

}	