using System;
using System.Collections;
using System.Collections.Generic;

public class TreeStructure<ElementT> : IEnumerable<ElementT> where ElementT : notnull {

	//======================================================================| Enums

	public enum TraversalOrders {
		FloorOrder,
		PreOrder,
		MidOrder,
		PostOrder,
	}

	//======================================================================| Properties

	public ElementT Element { get; set; } = default;

	public TreeStructure<ElementT> Left { get; set; } = null;
	public TreeStructure<ElementT> Right { get; set; } = null;

	public TraversalOrders TraversalOrder { get; set; } = TraversalOrders.FloorOrder;
	public IEnumerable<ElementT> Leaves => GetLeaves(this);
	public bool IsEmpty => Left is null && Right is null;

	//======================================================================| Constructors

	public TreeStructure() {}
	public TreeStructure(ElementT element) {
		Element = element;
	}

	//======================================================================| Methods

	public override bool Equals(object other) {
		if (other is not TreeStructure<ElementT> treeStructure) return false;
		return treeStructure.Element.Equals(Element);

	}

	public override int GetHashCode() =>
		Element.GetHashCode();

	public IEnumerator<ElementT> GetEnumerator() =>
		new TreeEnumerator<ElementT>(this, TraversalOrder);

	IEnumerator IEnumerable.GetEnumerator() =>
		GetEnumerator();

	public void Foreach(Action<ElementT> action) {
		foreach(var i in this) action(i);
	}

	//======================================================================| Operators

	public static bool operator == (TreeStructure<ElementT> left, TreeStructure<ElementT> right) => left.Equals(right);
	public static bool operator != (TreeStructure<ElementT> left, TreeStructure<ElementT> right) => !left.Equals(right);

	//======================================================================| Private Methods {
	
	private IEnumerable<ElementT> GetLeaves(TreeStructure<ElementT> root) {
		
		Queue<TreeStructure<ElementT>> queue = new();
		queue.Enqueue(root);

		while (queue.Count > 0) {
			
			bool isLeave = true;
			TreeStructure<ElementT> previous = queue.Dequeue();
			
			if (previous.Left is not null) {
				queue.Enqueue(previous.Left);
				isLeave = false;
			}

			if (previous.Right is not null) {
				queue.Enqueue(previous.Right);
				isLeave = false;
			}

			if (isLeave) {
				yield return previous.Element;
			}

		}

	}

}