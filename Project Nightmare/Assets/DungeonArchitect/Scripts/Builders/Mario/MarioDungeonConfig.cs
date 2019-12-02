using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Builders.Mario
{
	public class MarioDungeonConfig : DungeonConfig {
		public int minLength = 20;  
		public int maxLength = 25;

        public int minY = -2;
        public int maxY = 20;
        public int minDepth = -4;
        public int maxDepth = 15;

        public int minGap = 2;
        public int maxGap = 6;

        public int minNonGap = 4;

        public int maxStairHeight = 1;
        public float heightVariationProbablity = 0.1f;

        public float gapProbability = 0.1f;

        public int maxJumpTileDistance = 1;

        public Vector3 gridSize = new Vector3(4, 2, 4);

	}
}
