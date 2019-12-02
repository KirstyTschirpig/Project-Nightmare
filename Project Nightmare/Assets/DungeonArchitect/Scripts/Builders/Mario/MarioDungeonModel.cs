using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Builders.Mario
{
	public enum MarioTileType {
		Ground,
        Corridor,
        Gap
	}

	public class MarioTile {
		public IntVector position;
		public MarioTileType tileType;
	}

	public class MarioDungeonModel : DungeonModel {

		[HideInInspector]
		public MarioDungeonConfig Config;

		[HideInInspector]
		public MarioTile[] tiles;

        [HideInInspector]
        public int levelWidth;
        
    }
}
