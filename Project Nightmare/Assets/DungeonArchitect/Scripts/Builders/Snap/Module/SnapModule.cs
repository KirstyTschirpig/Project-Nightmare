using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using DungeonArchitect;
using DungeonArchitect.Utils;

namespace DungeonArchitect.Builders.Snap
{
    public class CellMarkerState
    {
        // Ground tile
        public HashSet<string> GroundTiles = new HashSet<string>();
        
        // Edges sharing a bottom left corner vertex
        public HashSet<string> EdgeTileX = new HashSet<string>();
        public HashSet<string> EdgeTileY = new HashSet<string>();
        public HashSet<string> EdgeTileZ = new HashSet<string>();

        // Bottom left corner   
        public HashSet<string> CornerTiles = new HashSet<string>(); 
    }

    public class ModuleMarkerRegsitry
    {
        public Dictionary<IntVector, CellMarkerState> MarkerStates = new Dictionary<IntVector, CellMarkerState>();

        public CellMarkerState GetState(IntVector position)
        {
            if (MarkerStates.ContainsKey(position))
            {
                return MarkerStates[position];
            }
            var state = new CellMarkerState();
            MarkerStates.Add(position, state);
            return state;
        }
    }

    public class SnapModuleEditorData
    {
        public IntVector cursorPosition = IntVector.Zero;
        public int brushSize = 1;
        public Matrix4x4 worldToCamera = Matrix4x4.identity;
    }

    public class SnapModule : MonoBehaviour
    {
        [Tooltip(@"This dungeon builder works on a grid based system and required modular mesh assets to be placed on each cell (floors, walls, doors etc). This important field specifies the size of the cell to use. This size is determined by the art asset used in the dungeon theme designed by the artist. In the demo, we have a floor mesh that is 400x400. The height of a floor is chosen to be 200 units as the stair mesh is 200 units high. Hence the defaults are set to 400x400x200. You should change this to the dimension of the modular asset your designer has created for the dungeon")]
        public Vector3 GridCellSize = new Vector3(4, 2, 4);
        public SnapModuleEditorData EditorData = new SnapModuleEditorData();
        public ModuleMarkerRegsitry MarkerRegistry = new ModuleMarkerRegsitry();

        void OnDrawGizmosSelected()
        {
            RenderScene();
        }

        void RenderScene() { 
            var renderCommands = new List<SnapGizmoRenderCommand>();
            foreach (var entry in MarkerRegistry.MarkerStates)
            {
                var position = entry.Key;
                var state = entry.Value;
                if (state.GroundTiles.Contains(DungeonConstants.ST_GROUND))
                {
                    var worldPosition = position * GridCellSize;
                    var command = CreateTileRenderCommand(worldPosition);
                    command.CalculateDistanceToCamera(EditorData.worldToCamera);
                    renderCommands.Add(command);
                }
            }

            // Cursor command
            {
                var cursorWorldPos = EditorData.cursorPosition * GridCellSize;
                var cursorCommand = CreateTileRenderCommand(cursorWorldPos);
                cursorCommand.drawColor = Color.red;
                cursorCommand.drawWireframe = true;
                cursorCommand.CalculateDistanceToCamera(EditorData.worldToCamera);
                renderCommands.Add(cursorCommand);

                // Create an overlay for blocked cursor region
                var cursorOverlayCommand = new SnapGizmoRenderCommandTile();
                cursorOverlayCommand.worldPosition = EditorData.cursorPosition * GridCellSize;
                cursorOverlayCommand.drawColor = new Color(1, 0, 0, .25f);
                cursorOverlayCommand.drawWireframe = true;
                cursorOverlayCommand.DistanceToCamera = 0;  // Force a draw on top of everything
                renderCommands.Add(cursorOverlayCommand);
            }

            // Sort based on depth from camera
            var depthSortedCommands = renderCommands.OrderByDescending(o => o.DistanceToCamera).ToArray();

            // Draw the commands
            foreach (var command in depthSortedCommands)
            {
                command.Draw(this);
            }
        }

        SnapGizmoRenderCommandTile CreateTileRenderCommand(Vector3 worldPosition)
        {
            var command = new SnapGizmoRenderCommandTile();
            command.worldPosition = worldPosition;
            command.drawColor = Color.white;
            command.drawWireframe = false;
            command.CalculateDistanceToCamera(EditorData.worldToCamera);
            return command;
        }

        void DrawCursor()
        {
            DrawTile(EditorData.cursorPosition, Color.red);
            
        }

        void DrawTile(IntVector position, Color color) {
            var worldSize = GridCellSize;
            var worldPosition = position * GridCellSize;
            worldPosition += new Vector3(worldSize.x, -worldSize.y, worldSize.z) / 2;
            
            Gizmos.color = color;
            Gizmos.DrawCube(worldPosition, worldSize);

            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(worldPosition, worldSize);

            
            //var CamRelativePos = EditorData.worldToCamera.MultiplyPoint(worldPosition);
            //Debug.Log(CamRelativePos.magnitude);
        }

    }

    abstract class SnapGizmoRenderCommand
    {
        public abstract void Draw(SnapModule module);
        public float DistanceToCamera = 0;
        public Vector3 worldPosition;
        public void CalculateDistanceToCamera(Matrix4x4 worldToCamera)
        {
            var camRelativePos = worldToCamera.MultiplyPoint(worldPosition);
            DistanceToCamera = camRelativePos.sqrMagnitude;
        }
    }

    class SnapGizmoRenderCommandTile : SnapGizmoRenderCommand
    {
        public Color drawColor = Color.white;
        public bool drawWireframe = false;
        public override void Draw(SnapModule module)
        {
            var worldSize = module.GridCellSize;
            //var worldPosition = position * module.GridCellSize;
            var drawPosition = worldPosition + new Vector3(worldSize.x, -worldSize.y, worldSize.z) / 2;

            Gizmos.color = drawColor;
            Gizmos.DrawCube(drawPosition, worldSize);
            if (drawWireframe)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawWireCube(drawPosition, worldSize);
            }
        }
    }
}
