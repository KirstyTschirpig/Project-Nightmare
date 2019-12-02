
using System.Collections;
using System.Collections.Generic;
using DungeonArchitect.Graphs;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.DungeonFlow
{
    public class DungeonFlowResultGraphContextMenu : GraphContextMenu
    {
        public override void Show(GraphEditor graphEditor, GraphPin sourcePin, Vector2 mouseWorld)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add Comment Node"), false, HandleContextMenu, DungeonFlowResultGraphEditorAction.CreateCommentNode);
            menu.ShowAsContext();
        }

        void HandleContextMenu(object action)
        {
            DispatchMenuItemEvent(action, BuildEvent(null));
        }
    }
}
