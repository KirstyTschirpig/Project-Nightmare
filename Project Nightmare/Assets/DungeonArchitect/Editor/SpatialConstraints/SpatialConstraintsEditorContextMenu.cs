using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DungeonArchitect.Graphs;

namespace DungeonArchitect.Editors.SpatialConstraints
{
    public class SpatialConstraintsEditorContextMenu : GraphContextMenu
    {
        public override void Show(GraphEditor graphEditor, GraphPin sourcePin, Vector2 mouseWorld)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Create Rule Node"), false, HandleContextMenu, SpatialConstraintEditorMenuAction.CreateRuleNode);
            menu.AddItem(new GUIContent("Create Comment Node"), false, HandleContextMenu, SpatialConstraintEditorMenuAction.CreateCommentNode);
            menu.ShowAsContext();
        }



        void HandleContextMenu(object action)
        {
            DispatchMenuItemEvent(action, BuildEvent(null));
        }
    }
}