using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class BinarySpaceParticipation : IAlgorithm, IDisplayable {

	//======================================================================| Members

	private float randomRange = 0.3f;
	private TreeStructure<Rect> spaceTree;

	private IEnumerable<Rect> resultRects = null;

	//======================================================================| Properties

	public Vector2 Size { get; set; }
	public uint Generation { get; set; }

	public float RandomRange { 
		get => randomRange;
		set {
			if (value < 0f) {
				Debug.LogWarning(
					$"Range of BSP is '{value}', but it cannot be negative. " +
					$"It has beed clapmed to 0f."
				);
				randomRange = 0f;
				return;
			}

			if (value > 1f) {
				Debug.LogWarning(
					$"Range of BSP is '{value}', but it cannot be larger then 1f. " +
					$"It has beed clamped to 1f."
				);
				randomRange = 1f;
				return;
			}

			randomRange = value;
		}
	}

	public IEnumerable<Rect> ResultRects =>
		AlgorithmNotInvokedException.ThrowIfNull(this, resultRects);

	//======================================================================| Methods

	public void Invoke() {

		spaceTree = new(new(0, 0, Size.x, Size.y));
		SpacePartition(spaceTree);

		resultRects = spaceTree.Leaves;

	}

	public void Display(Color color, float duration = float.MaxValue) {

		AlgorithmNotInvokedException.ThrowIfNull(this, spaceTree);	

		foreach (var rect in spaceTree.Leaves) {
			DebugVisual.Draw(rect, color, duration);
		}

	}

	//======================================================================| Private Methods

	private void SpacePartition(TreeStructure<Rect> tree, int generation = 0) {
		
		if (generation >= Generation) return;

		Rect rootRect = tree.Element;
		bool isVertical =
			Mathf.Approximately(rootRect.width, rootRect.height)
			? Random.value > 0.5f
			: rootRect.height < rootRect.width;


		if (isVertical) {

			float lineCenter = rootRect.width / 2f;
			float rangeOffset = rootRect.width * RandomRange / 2f;
			float linePosition = Random.Range(lineCenter - rangeOffset, lineCenter + rangeOffset);

			Rect leftRect = new() {
				x = rootRect.xMin,
				y = rootRect.yMin,
				width = linePosition,
				height = rootRect.height
			};

			Rect rightRect = new() {
				x = leftRect.xMax,
				y = rootRect.yMin,
				width = rootRect.width - leftRect.width,
				height = rootRect.height
			};

			tree.Left = new(leftRect);
			tree.Right = new(rightRect);
		}
		else {

			float lineCenter = rootRect.height / 2f;
			float rangeOffset = rootRect.height * RandomRange / 2f;
			float linePosition = Random.Range(lineCenter - rangeOffset, lineCenter + rangeOffset);

			Rect lowerRect = new() {
				x = rootRect.xMin,
				y = rootRect.yMin,
				width = rootRect.width,
				height = linePosition
			};

			Rect upperRect = new() {
				x = rootRect.x,
				y = lowerRect.yMax,
				width = rootRect.width,
				height = rootRect.height - lowerRect.height
			};

			tree.Left = new(lowerRect);
			tree.Right = new(upperRect);
		}

		SpacePartition(tree.Left, generation + 1);
		SpacePartition(tree.Right, generation + 1);
	}

}
