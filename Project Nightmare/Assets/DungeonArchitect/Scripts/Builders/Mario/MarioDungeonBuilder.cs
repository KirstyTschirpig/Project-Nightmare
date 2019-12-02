using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Builders.Mario
{
	class MarioDungeonConstants
    {
        public static readonly string Ground = "Ground";
        public static readonly string WallFront = "WallFront";
        public static readonly string WallBack = "WallBack";
        public static readonly string WallSide = "WallSide";
        public static readonly string BackgroundGround = "BG Ground";
        public static readonly string BackgroundCeiling = "BG Ceiling";
        public static readonly string BackgroundWall = "BG Wall";
        public static readonly string Stair = "Stair";
        public static readonly string Corridor = "Corridor";
    }

	public class MarioDungeonBuilder : DungeonBuilder {

		MarioDungeonConfig marioConfig;
		MarioDungeonModel marioModel;

		new System.Random random;

		/// <summary>
		/// Builds the dungeon layout.  In this method, you should build your dungeon layout and save it in your model file
		/// No markers should be emitted here.   (EmitMarkers function will be called later by the engine to do that)
		/// </summary>
		/// <param name="config">The builder configuration</param>
		/// <param name="model">The dungeon model that the builder will populate</param>
		public override void BuildDungeon(DungeonConfig config, DungeonModel model)
		{
			base.BuildDungeon(config, model);

			random = new System.Random((int)config.Seed);

			// We know that the dungeon prefab would have the appropriate config and models attached to it
			// Cast and save it for future reference
			marioConfig = config as MarioDungeonConfig;
			marioModel = model as MarioDungeonModel;
			marioModel.Config = marioConfig;

			// Generate the city layout and save it in a model.   No markers are emitted here. 
			GenerateLevelLayout();
		}

		/// <summary>
		/// Override the builder's emit marker function to emit our own markers based on the layout that we built
		/// You should emit your markers based on the layout you have saved in the model generated previously
		/// When the user is designing the theme interactively, this function will be called whenever the graph state changes,
		/// so the theme engine can populate the scene (BuildDungeon will not be called if there is no need to rebuild the layout again)
		/// </summary>
		public override void EmitMarkers()
		{
			base.EmitMarkers();

			EmitLevelMarkers();

			ProcessMarkerOverrideVolumes();
		}

		void GenerateLevelLayout() {
			var tiles = new List<MarioTile> ();

			marioModel.levelWidth = random.Next (marioConfig.minLength, marioConfig.maxLength);

            bool addingGap = false;
            int targetGapDistance = 0;
            int currentGapDistance = 0;
            int currentNonGapDistance = 0;

            int y = 0;
            for (int x = 0; x < marioModel.levelWidth; x++)
            {
                var position = new IntVector(x, y, 0);

                if (!addingGap)
                {
                    if (x >= 2)
                    {
                        bool changeHeight = random.NextFloat() < marioConfig.heightVariationProbablity;
                        if (changeHeight)
                        {
                            bool moveUp = random.NextFloat() < 0.5f;
                            y += moveUp ? 1 : -1;
                        }
                    }

                    // Create a tile here

                    var tile = new MarioTile();
                    tile.position = position;
                    tile.tileType = MarioTileType.Ground;
                    tiles.Add(tile);

                    currentNonGapDistance++;

                    if (currentNonGapDistance >= marioConfig.minNonGap)
                    {
                        // Check if we need to add a gap
                        addingGap = random.NextFloat() < marioConfig.gapProbability;
                        if (addingGap)
                        {
                            currentGapDistance = 0;
                            targetGapDistance = random.Next(marioConfig.minGap, marioConfig.maxGap);
                        }
                    }
                }
                else
                {
                    bool corridor = targetGapDistance > marioConfig.maxJumpTileDistance;
                    // We are adding a gap
                    var tile = new MarioTile();
                    tile.position = position;
                    tile.tileType = corridor ? MarioTileType.Corridor : MarioTileType.Gap;
                    tiles.Add(tile);

                    currentGapDistance++;
                    if (currentGapDistance >= targetGapDistance)
                    {
                        addingGap = false;
                        currentNonGapDistance = 0;
                    }
                }

			}

			marioModel.tiles = tiles.ToArray();
		}

        

		void EmitLevelMarkers()
        {
            var gridSize = marioConfig.gridSize;
            var tileMap = new Dictionary<int, MarioTile>();
            var heights = new int[marioModel.levelWidth];

            foreach (var tile in marioModel.tiles) {
                if (tile.tileType == MarioTileType.Ground || tile.tileType == MarioTileType.Corridor)
                {
                    var worldPosition = tile.position * gridSize;
                    var markerTransform = Matrix4x4.TRS(worldPosition, Quaternion.identity, Vector3.one);
                    string markerName = tile.tileType == MarioTileType.Ground ? MarioDungeonConstants.Ground : MarioDungeonConstants.Corridor;
                    EmitMarker(markerName, markerTransform, tile.position, -1);
                }

                tileMap.Add(tile.position.x, tile);
                heights[tile.position.x] = tile.tileType == MarioTileType.Ground ? tile.position.y : marioConfig.minY;
			}
            
            for (int x = 0; x < marioModel.levelWidth; x++)
            {
                for (int z = marioConfig.minDepth; z <= marioConfig.maxDepth; z++)
                {
                    // Insert the background ground
                    {
                        var positionI = new IntVector(x, marioConfig.minY, z);
                        var positionF = positionI * gridSize;
                        var markerTransform = Matrix4x4.TRS(positionF, Quaternion.identity, Vector3.one);
                        EmitMarker(MarioDungeonConstants.BackgroundGround, markerTransform, positionI, -1);
                    }

                    // Insert the background ceiling
                    {
                        var positionI = new IntVector(x, marioConfig.maxY + 1, z);
                        var positionF = positionI * gridSize;
                        var markerTransform = Matrix4x4.TRS(positionF, Quaternion.Euler(180, 0, 0), Vector3.one);
                        EmitMarker(MarioDungeonConstants.BackgroundCeiling, markerTransform, positionI, -1);
                    }
                }

                // Insert the background walls (back)
                for (int y = marioConfig.minY; y <= marioConfig.maxY; y++)
                {
                    var positionI = new IntVector(x, y, marioConfig.maxDepth);
                    var positionF = positionI * gridSize;
                    positionF.z += gridSize.z / 2.0f;
                    var markerTransform = Matrix4x4.TRS(positionF, Quaternion.identity, Vector3.one);
                    EmitMarker(MarioDungeonConstants.BackgroundWall, markerTransform, positionI, -1);
                }

                // Insert the side walls
                if (x + 1 < heights.Length)
                {
                    int minY = Mathf.Min(heights[x], heights[x + 1]);
                    int maxY = Mathf.Max(heights[x], heights[x + 1]);
                    
                    for (int y = minY; y < maxY; y++)
                    {
                        var positionI = new IntVector(x, y, 0);
                        var positionF = positionI * gridSize;
                        positionF.x += gridSize.x / 2.0f;
                        var markerTransform = Matrix4x4.TRS(positionF, Quaternion.Euler(0, 90, 0), Vector3.one);
                        EmitMarker(MarioDungeonConstants.WallSide, markerTransform, positionI, -1);
                    }

                    // Insert stairs
                    var heightDifference = maxY - minY;
                    if (heightDifference > 0)
                    {
                        // Make sure we have ground tiles on both the sides
                        if (tileMap[x].tileType != MarioTileType.Gap && tileMap[x + 1].tileType != MarioTileType.Gap)
                        {
                            string markerName = MarioDungeonConstants.Stair;
                            if (heightDifference > 1)
                            {
                                markerName += heightDifference.ToString();
                            }

                            IntVector positionI = new IntVector(x, heights[x], 0);
                            Quaternion rotation = Quaternion.identity;
                            if (heights[x] > heights[x + 1])
                            {
                                positionI.x++;
                                positionI.y = heights[x + 1];
                                rotation = Quaternion.Euler(0, 180, 0);
                            }
                            var positionF = positionI * gridSize;
                            var markerTransform = Matrix4x4.TRS(positionF, rotation, Vector3.one);
                            EmitMarker(markerName, markerTransform, positionI, -1);
                        }
                    }
                }
                
            }
            
            for (int z = marioConfig.minDepth; z <= marioConfig.maxDepth; z++)
            {
                for (int y = marioConfig.minY; y <= marioConfig.maxY; y++)
                {
                    // Insert the background walls (left)
                    {
                        int x = 0;
                        var positionI = new IntVector(x, y, z);
                        var positionF = positionI * gridSize;
                        positionF.x -= gridSize.x / 2.0f;
                        var markerTransform = Matrix4x4.TRS(positionF, Quaternion.Euler(0, -90, 0), Vector3.one);
                        EmitMarker(MarioDungeonConstants.BackgroundWall, markerTransform, positionI, -1);
                    }
                    {
                        int x = marioModel.levelWidth;
                        var positionI = new IntVector(x, y, z);
                        var positionF = positionI * gridSize;
                        positionF.x -= gridSize.x / 2.0f;
                        var markerTransform = Matrix4x4.TRS(positionF, Quaternion.Euler(0, 90, 0), Vector3.one);
                        EmitMarker(MarioDungeonConstants.BackgroundWall, markerTransform, positionI, -1);
                    }
                }
            }

            // Insert the front / back walls
            foreach (var tile in marioModel.tiles)
            {
                if (tile.tileType == MarioTileType.Ground)
                {
                    // Insert front walls
                    for (int y = marioConfig.minY; y < tile.position.y; y++)
                    {
                        var positionI = tile.position;
                        positionI.y = y;
                        var positionF = positionI * gridSize;
                        positionF -= new Vector3(0, 0, gridSize.z / 2.0f);
                        var markerTransform = Matrix4x4.TRS(positionF, Quaternion.identity, Vector3.one);
                        EmitMarker(MarioDungeonConstants.WallFront, markerTransform, positionI, -1);
                    }

                    // insert back wall
                    {
                        var positionI = tile.position;
                        var positionF = positionI * gridSize;
                        positionF += new Vector3(0, 0, gridSize.z / 2.0f);
                        var markerTransform = Matrix4x4.TRS(positionF, Quaternion.identity, Vector3.one);
                        EmitMarker(MarioDungeonConstants.WallBack, markerTransform, positionI, -1);
                    }

                }
            }
		} 

	}
}