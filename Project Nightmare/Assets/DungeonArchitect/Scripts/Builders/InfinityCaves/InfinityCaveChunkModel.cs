using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Builders.Infinity.Caves
{
	public enum MazeTileState {
		Empty,
        Rock,
        Wall
	}

	public class InfinityCaveChunkModel : DungeonModel {
		[HideInInspector]
		public InfinityCaveChunkConfig Config;

		[HideInInspector]
		public MazeTileState[,] tileStates;
    }
}
