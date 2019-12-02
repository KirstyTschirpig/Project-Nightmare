using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DungeonArchitect.Graphs;

namespace DungeonArchitect.Editors
{
    /// <summary>
    /// The type of menu action to perform
    /// </summary>
    public enum ThemeEditorMenuAction
    {
        AddGameObjectNode,
        AddGameObjectArrayNode,
        AddSpriteNode,
        AddMarkerNode,
        AddMarkerEmitterNode,
        AddCommentNode
    }

    public class DungeonThemeEditorContextMenu : GraphContextMenu
    {
        bool showItemMeshNode;
        bool showItemMarkerNode;
        bool showCommentNode;
        bool showItemMarkerEmitterNode;

        /// <summary>
        /// Shows the context menu in the theme graph editor
        /// </summary>
        /// <param name="graph">The graph shown in the graph editor</param>
        /// <param name="sourcePin">The source pin, if the user dragged a link out of a pin. null otherwise</param>
        /// <param name="mouseWorld">The position of the mouse. The context menu would be shown from here</param>
        public override void Show(GraphEditor graphEditor, GraphPin sourcePin, Vector2 mouseWorld)
        {
            this.sourcePin = sourcePin;
            this.mouseWorldPosition = mouseWorld;

            if (sourcePin == null)
            {
                showItemMeshNode = true;
                showItemMarkerNode = true;
                showItemMarkerEmitterNode = true;
                showCommentNode = true;
            }
            else
            {
                showItemMeshNode = false;
                showItemMarkerNode = false;
                showItemMarkerEmitterNode = false;
                showCommentNode = false;

                if (sourcePin.Node is MarkerNode)
                {
                    showItemMeshNode = true;
                }
                else if (sourcePin.Node is VisualNode)
                {
                    if (sourcePin.PinType == GraphPinType.Input)
                    {
                        // We can only create marker nodes from here
                        showItemMarkerNode = true;
                    }
                    else
                    {
                        // We can only create marker emitter nodes from here
                        showItemMarkerEmitterNode = true;
                    }
                }
                else if (sourcePin.Node is MarkerEmitterNode)
                {
                    // we can only create mesh nodes from the input pin of this node
                    showItemMeshNode = true;
                }
            }

            ShowMenu(graphEditor);
        }

        string[] GetMarkers(Graph graph)
        {
            var markers = new List<string>();
            if (graph != null)
            {
                foreach (var node in graph.Nodes)
                {
                    if (node is MarkerNode)
                    {
                        markers.Add(node.Caption);
                    }
                }
            }
            var markerArray = markers.ToArray();
            System.Array.Sort(markerArray);
            return markerArray;
        }


        private void ShowMenu(GraphEditor graphEditor)
        {
            var menu = new GenericMenu();
            if (showItemMeshNode)
            {
                menu.AddItem(new GUIContent("Add Game Object Node"), false, AddGameObjectNode);
                menu.AddItem(new GUIContent("Add Game Object Array Node"), false, AddGameObjectArrayNode);
                menu.AddItem(new GUIContent("Add Sprite Node"), false, AddSpriteNode);
            }
            if (showItemMarkerNode)
            {
                menu.AddItem(new GUIContent("Add Marker Node"), false, AddMarkerNode);
            }

            if (showCommentNode)
            {
                menu.AddItem(new GUIContent("Add Comment Node"), false, AddCommentNode);
            }

            if (showItemMarkerEmitterNode)
            {
                var markers = GetMarkers(graphEditor.Graph);
                if (markers.Length > 0)
                {
                    if (showItemMeshNode || showItemMarkerNode)
                    {
                        menu.AddSeparator("");
                    }
                    foreach (var marker in markers)
                    {
                        menu.AddItem(new GUIContent("Add Marker Emitter: " + marker), false, AddMarkerEmitterNode, marker);
                    }
                }
            }

            menu.ShowAsContext();
        }

        void AddGameObjectNode()
        {
            DispatchMenuItemEvent(ThemeEditorMenuAction.AddGameObjectNode, BuildEvent(null));
        }

        void AddGameObjectArrayNode()
        {
            DispatchMenuItemEvent(ThemeEditorMenuAction.AddGameObjectArrayNode, BuildEvent(null));
        }

        void AddSpriteNode()
        {
            DispatchMenuItemEvent(ThemeEditorMenuAction.AddSpriteNode, BuildEvent(null));
        }

        void AddMarkerNode()
        {
            DispatchMenuItemEvent(ThemeEditorMenuAction.AddMarkerNode, BuildEvent(null));
        }

        void AddCommentNode()
        {
            DispatchMenuItemEvent(ThemeEditorMenuAction.AddCommentNode, BuildEvent(null));
        }

        void AddMarkerEmitterNode(object userdata)
        {
            DispatchMenuItemEvent(ThemeEditorMenuAction.AddMarkerEmitterNode, BuildEvent(userdata));
        }

    }
}