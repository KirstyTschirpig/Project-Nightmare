using System.Collections;
using System.Collections.Generic;
using DungeonArchitect.Grammar;
using DungeonArchitect.Graphs;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.DungeonFlow
{
    public class DungeonFlowExecutionGraphContextMenu : GraphContextMenu
    {
        public override void Show(GraphEditor graphEditor, GraphPin sourcePin, Vector2 mouseWorld)
        {
            this.sourcePin = sourcePin;
            var execEditor = graphEditor as DungeonFlowExecutionGraphEditor;
            var flowAsset = (execEditor != null) ? execEditor.FlowAsset : null;

            var menu = new GenericMenu();
            if (flowAsset != null && flowAsset.productionRules.Length > 0)
            {
                foreach (var rule in flowAsset.productionRules)
                {
                    string text = "Add Rule: " + rule.ruleName;
                    menu.AddItem(new GUIContent(text), false, HandleContextMenu, new DungeonFlowExecutionGraphEditorMenuData(DungeonFlowExecutionGraphEditorAction.CreateRuleNode, rule));
                }
                menu.AddSeparator("");
            }
            menu.AddItem(new GUIContent("Add Comment Node"), false, HandleContextMenu, new DungeonFlowExecutionGraphEditorMenuData(DungeonFlowExecutionGraphEditorAction.CreateCommentNode));
            menu.ShowAsContext();
        }

        void HandleContextMenu(object action)
        {
            DispatchMenuItemEvent(action, BuildEvent(null));
        }
    }
}
