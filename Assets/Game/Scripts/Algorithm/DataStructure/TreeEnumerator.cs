using System;
using System.Collections;
using System.Collections.Generic;

public class TreeEnumerator<T> : IEnumerator<T> where T : notnull {

	//======================================================================| Members

	private int position = -1;
	private readonly List<T> serialized = new();

	private readonly TreeStructure<T> original;
	private readonly TreeStructure<T>.TraversalOrders traversalOrder;

	//======================================================================| Constructors

	public TreeEnumerator(TreeStructure<T> treeStructure, TreeStructure<T>.TraversalOrders traversalOrder) {

		original = treeStructure;
		this.traversalOrder = traversalOrder;

		Serialize();

	}

	//======================================================================| Methods

	public bool MoveNext() {
		position++;
		return position < serialized.Count;
	}

	public void Reset() => position = -1;

	void IDisposable.Dispose() {}

	//======================================================================| Properties

	object IEnumerator.Current => Current;

	public T Current {
		get {
			if (position >= serialized.Count) throw new InvalidOperationException();
			return serialized[position];
		}
	}

	//======================================================================| Private Methods

	private void Serialize() {
		
		switch (traversalOrder) {
			
			case TreeStructure<T>.TraversalOrders.FloorOrder:
				SerializeFloorOrder();
				return;

			case TreeStructure<T>.TraversalOrders.PreOrder: 
				SerializePreOrder(original);
				return;

			case TreeStructure<T>.TraversalOrders.MidOrder:
				SerializeMidOrder(original);
				return;

			case TreeStructure<T>.TraversalOrders.PostOrder:
				SerializePostOrder(original);
				return;

		}
	
	}

	private void SerializeFloorOrder() {

		Queue<TreeStructure<T>> queue = new();
		queue.Enqueue(original);

		while (queue.Count > 0) {

			TreeStructure<T> previous = queue.Dequeue();

			serialized.Add(previous.Element);

			if (previous.Left is not null) queue.Enqueue(previous.Left);
			if (previous.Right is not null) queue.Enqueue(previous.Right);
		}

	}

	private void SerializePreOrder(TreeStructure<T> root) {
		
		serialized.Add(root.Element);
		if (root.Left is not null) SerializePreOrder(root.Left);
		if (root.Right is not null) SerializePreOrder(root.Right);

	}

	private void SerializeMidOrder(TreeStructure<T> root) {
		
		if (root.Left is not null) SerializeMidOrder(root.Left);
		serialized.Add(root.Element);
		if (root.Right is not null) SerializeMidOrder(root.Right);
	}

	private void SerializePostOrder(TreeStructure<T> root) {
	
		if (root.Left is not null) SerializePostOrder(root.Left);
		if (root.Right is not null) SerializePostOrder(root.Right);
		serialized.Add(root.Element);

	}


}