//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using UnityEditor;
using System.Collections;
using DungeonArchitect.Graphs;
using DMathUtils = DungeonArchitect.Utils.MathUtils;


namespace DungeonArchitect.Editors
{
    public enum GraphLinkRendererMode
    {
        Splines,
        StraightLines
    }

    /// <summary>
    /// Renders the graph link in the graph editor
    /// </summary>
    public class GraphLinkRenderer
    {
        public static void DrawGraphLink(GraphRendererContext rendererContext, GraphLink link, GraphCamera camera)
        {
            if (link.Input == null || link.Output == null)
            {
                // Link not initialized yet. nothing to draw
                return;
            }

            GraphLinkRendererMode renderMode = (rendererContext.GraphEditor != null)
                ? rendererContext.GraphEditor.LinkRenderMode : GraphLinkRendererMode.Splines;

            if (renderMode == GraphLinkRendererMode.Splines)
            {
                DrawGraphLink_Splines(rendererContext, link, camera);
            }
            else if (renderMode == GraphLinkRendererMode.StraightLines)
            {
                DrawGraphLink_StraightLine(rendererContext, link, camera);
            }
        }

        private static Vector2 GetPointOnNodeBounds(Vector2 position, GraphPin pin, float distanceBias)
        {
            var nodeRect = (pin.Node != null) ? pin.Node.Bounds : new Rect(pin.WorldPosition, Vector2.one);
            var center = nodeRect.position + nodeRect.size * 0.5f;
            var b = new Bounds(center, nodeRect.size);
            var direction = (center - position).normalized;
            var r = new Ray(position, direction);
            float intersectDistance;
            if (b.IntersectRay(r, out intersectDistance))
            {
                return position + direction * (intersectDistance + distanceBias);
            }

            return pin.WorldPosition;
        }

        private static void DrawGraphLink_StraightLine(GraphRendererContext rendererContext, GraphLink link, GraphCamera camera)
        {
            var lineColor = new Color(1, 1, 1, 1);
            float lineThickness = 3;

            Vector2 startPos, endPos;
            {
                float bias = -5;
                startPos = GetPointOnNodeBounds(link.Input.WorldPosition, link.Output, bias);
                endPos = GetPointOnNodeBounds(link.Output.WorldPosition, link.Input, bias);

                startPos = camera.WorldToScreen(startPos);
                endPos = camera.WorldToScreen(endPos);
            }

            Handles.color = lineColor;
            Handles.DrawAAPolyLine(lineThickness, startPos, endPos);

            Handles.ArrowHandleCap(0, endPos, Quaternion.identity, 30, EventType.Used);

            var rotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), (startPos - endPos).normalized);
            float arrowSize = 10.0f / camera.ZoomLevel;
            float arrowWidth = 0.5f / camera.ZoomLevel;
            var arrowTails = new Vector2[] {
                rotation * new Vector3(1, arrowWidth) * arrowSize,
                rotation * new Vector3(1, -arrowWidth) * arrowSize,
            };

            var p0 = endPos;
            var p1 = endPos + arrowTails[0];
            var p2 = endPos + arrowTails[1];

            Handles.DrawAAConvexPolygon(p0, p1, p2, p0);
        }

        private static void DrawGraphLink_Splines(GraphRendererContext rendererContext, GraphLink link, GraphCamera camera)
        {
            float lineThickness = 3;

            Vector2 startPos = camera.WorldToScreen(link.Output.WorldPosition);
            Vector2 endPos = camera.WorldToScreen(link.Input.WorldPosition);
			var tangentStrength = link.GetTangentStrength() / camera.ZoomLevel;
            Vector3 startTan = startPos + link.Output.Tangent * tangentStrength;
            Vector3 endTan = endPos + link.Input.Tangent * tangentStrength;
            var lineColor = new Color(1, 1, 1, 0.75f);
            Handles.DrawBezier(startPos, endPos, startTan, endTan, lineColor, null, lineThickness);

            // Draw the arrow cap
            var rotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), link.Input.Tangent.normalized);
			float arrowSize = 10.0f / camera.ZoomLevel;
			float arrowWidth = 0.5f / camera.ZoomLevel;
            var arrowTails = new Vector2[] {
			    rotation * new Vector3(1, arrowWidth) * arrowSize, 
			    rotation * new Vector3(1, -arrowWidth) * arrowSize, 
		    };
            Handles.color = lineColor;

            //Handles.DrawPolyLine(arrowTails);
            Handles.DrawLine(endPos, endPos + arrowTails[0]);
            Handles.DrawLine(endPos, endPos + arrowTails[1]);
            Handles.DrawLine(endPos + arrowTails[0], endPos + arrowTails[1]);

        }
    }
}
