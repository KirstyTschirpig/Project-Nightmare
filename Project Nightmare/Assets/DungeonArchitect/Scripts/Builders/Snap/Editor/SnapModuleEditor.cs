//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEditor;
using UnityEngine;
using System.Collections;

using DungeonArchitect;
using DungeonArchitect.Utils;
using DungeonArchitect.Builders.Snap;
using DMathUtils = DungeonArchitect.Utils.MathUtils;

namespace DungeonArchitect.Editors
{
    [CustomEditor(typeof(SnapModule))]
    public class SnapModuleEditor : DungeonPaintModeEditor
    {
        bool modeDelete = false;
        int cursorHeight = 0;
        //float overlayOpacity = 1.0f;

        
        protected override void SceneGUI(SceneView sceneview)
        {
            var e = Event.current;
            UpdateCursorPosition();
            modeDelete = e.shift;

            int buttonId = 0;
            if (e.type == EventType.MouseDown && e.button == buttonId)
            {
                ProcessPaintRequest(e);
                e.Use();
            }
            else if (e.type == EventType.MouseUp && e.button == buttonId)
            {
                e.Use();
            }
            else if (e.type == EventType.MouseDrag && e.button == buttonId)
            {
                ProcessPaintRequest(e);
                e.Use();
            }
            else if (e.type == EventType.ScrollWheel)
            {
                e.Use();
            }
            else if (e.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
            }
            
            sceneview.Repaint();

            var module = GetModule();
            module.EditorData.worldToCamera = sceneview.camera.worldToCameraMatrix;
        }

        void ProcessPaintRequest(Event e)
        {
            var module = GetModule();
            var cursorPosition = module.EditorData.cursorPosition;
            var state = module.MarkerRegistry.GetState(cursorPosition);

            if (modeDelete)
            {
                state.GroundTiles.Remove(DungeonConstants.ST_GROUND);
            }
            else
            {
                state.GroundTiles.Add(DungeonConstants.ST_GROUND);
            }
        }


        void UpdateCursorPosition()
        {
            float distance;
            var e = Event.current;
            Plane plane;
            var planePoint = new Vector3(0, cursorHeight, 0);
            plane = new Plane(Vector3.up, planePoint);

            var ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (plane.Raycast(ray, out distance))
            {
                var hitpoint = ray.GetPoint(distance);

                var module = GetModule();
                // Get the grid position
                module.EditorData.cursorPosition = SnapToGrid(hitpoint);

                if (e.type == EventType.ScrollWheel)
                {
                    var delta = (int)Mathf.Sign(e.delta.y);
                    cursorHeight += delta;
                }
            }
        }

        SnapModule GetModule()
        {
            return target as SnapModule;
        }

        IntVector SnapToGrid(Vector3 value)
        {
            var result = new IntVector();
            var module = GetModule();
            var gridSize = module.GridCellSize;
            result.x = Mathf.FloorToInt(value.x / gridSize.x);
            result.y = Mathf.FloorToInt(cursorHeight / gridSize.y);
            result.z = Mathf.FloorToInt(value.z / gridSize.z);
            return result;
        }
    }
}