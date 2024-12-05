using System;

public struct UnorderedPair<ElementT> : IEquatable<UnorderedPair<ElementT>> where ElementT : notnull {

	//======================================================================| Members

	public ElementT Item1;
	public ElementT Item2;

	//======================================================================| Constructors

	public UnorderedPair(ElementT item1, ElementT item2) {
		Item1 = item1;
		Item2 = item2;
	}

	//======================================================================| Methods

	public override readonly bool Equals(object obj) {

		if (obj is UnorderedPair<float> floatPair) return Equals(floatPair);
		if (obj is UnorderedPair<ElementT> pair) return Equals(pair);

		return false;

	}

	public readonly bool Equals(UnorderedPair<float> other) {

		if (Item1 is not float item1) return false;
		if (Item2 is not float item2) return false;

		if (item1.Approximately(other.Item1) && item2.Approximately(other.Item2)) return true;
		if (item1.Approximately(other.Item2) && item2.Approximately(other.Item1)) return true;

		return false;

	}

	public readonly bool Equals(UnorderedPair<ElementT> other) =>
		(Item1.Equals(other.Item1) && Item2.Equals(other.Item2)) ||
		(Item1.Equals(other.Item2) && Item2.Equals(other.Item1));

	public override readonly int GetHashCode() {

		int hash1 = Item1.GetHashCode();
		int hash2 = Item2.GetHashCode();

		(int hashLarge, int hashLess) =
			(hash1 > hash2)
			? (hash1, hash2)
			: (hash2, hash1);

		return HashCode.Combine(hashLarge, hashLess);

	}
}