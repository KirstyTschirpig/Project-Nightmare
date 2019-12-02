//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using DungeonArchitect;
using DungeonArchitect.Graphs;

namespace DungeonArchitect.Editors
{

    /// <summary>
    /// The graph context menu event data
    /// </summary>
    public class GraphContextMenuEvent
    {
        public GraphPin sourcePin;
        public Vector2 mouseWorldPosition;
        public object userdata;
    }

    /// <summary>
    /// The context menu shown when the user right clicks on the theme graph editor
    /// </summary>
    public abstract class GraphContextMenu
    {
        protected bool dragged;
        protected int dragButtonId = 1;


        protected GraphPin sourcePin;
        protected Vector2 mouseWorldPosition;

        public delegate void OnRequestContextMenuCreation(Event e);
        public event OnRequestContextMenuCreation RequestContextMenuCreation;

        public delegate void OnMenuItemClicked(object userdata, GraphContextMenuEvent e);
        public event OnMenuItemClicked MenuItemClicked;

        /// <summary>
        /// Handles mouse input
        /// </summary>
        /// <param name="e">Input event data</param>
        public void HandleInput(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == dragButtonId)
                    {
                        dragged = false;
                    }
                    break;

                case EventType.MouseDrag:
                    if (e.button == dragButtonId)
                    {
                        dragged = true;
                    }
                    break;

                case EventType.MouseUp:
                    if (e.button == dragButtonId && !dragged)
                    {
                        if (RequestContextMenuCreation != null)
                        {
                            RequestContextMenuCreation(e);
                        }
                    }
                    break;
            }

        }

        protected GraphContextMenuEvent BuildEvent(object userdata)
        {
            var e = new GraphContextMenuEvent();
            e.userdata = userdata;
            e.sourcePin = sourcePin;
            e.mouseWorldPosition = mouseWorldPosition;
            return e;
        }


        /// <summary>
        /// Shows the context menu in the theme graph editor
        /// </summary>
        /// <param name="graph">The graph shown in the graph editor</param>
        /// <param name="sourcePin">The source pin, if the user dragged a link out of a pin. null otherwise</param>
        /// <param name="mouseWorld">The position of the mouse. The context menu would be shown from here</param>
        public abstract void Show(GraphEditor graphEditor, GraphPin sourcePin, Vector2 mouseWorld);

        protected void DispatchMenuItemEvent(object action, GraphContextMenuEvent e)
        {
            if (MenuItemClicked != null)
            {
                MenuItemClicked(action, e);
            }
        }
    }


    public class NullGraphContextMenu : GraphContextMenu
    {
        public override void Show(GraphEditor graphEditor, GraphPin sourcePin, Vector2 mouseWorld)
        {
        }

    }
}
