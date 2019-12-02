using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Builders.Maze
{
	public class MazeDungeonConfig : DungeonConfig {
		public int mazeWidth = 20;  
		public int mazeHeight = 25;
        
        public Vector2 gridSize = new Vector2(4, 4);
	}
}
