using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DungeonArchitect.Grammar;
using DungeonArchitect.Graphs;

namespace DungeonArchitect.Editors.DungeonFlow
{
    public class DungeonFlowGrammarGraphContextMenu : GraphContextMenu
    {
        public override void Show(GraphEditor graphEditor, GraphPin sourcePin, Vector2 mouseWorld)
        {
            this.sourcePin = sourcePin;
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add Wildcard Node"), false, HandleContextMenu, new DungeonFlowGrammarGraphEditorContextMenuData(DungeonFlowGrammarGraphEditorAction.CreateWildcard));
            menu.AddSeparator("");

            var grammarGraphEditor = graphEditor as DungeonFlowGrammarGraphEditor;
            var flowAsset = (grammarGraphEditor != null) ? grammarGraphEditor.FlowAsset : null;
            if (flowAsset != null && flowAsset.nodeTypes != null && flowAsset.nodeTypes.Length > 0)
            {
                foreach (var nodeType in flowAsset.nodeTypes)
                {
                    var data = new DungeonFlowGrammarGraphEditorContextMenuData(DungeonFlowGrammarGraphEditorAction.CreateTaskNode, nodeType);
                    menu.AddItem(new GUIContent("Add Node: " + nodeType.nodeName), false, HandleContextMenu, data);
                }
                menu.AddSeparator("");
            }

            menu.AddItem(new GUIContent("Add Comment Node"), false, HandleContextMenu, new DungeonFlowGrammarGraphEditorContextMenuData(DungeonFlowGrammarGraphEditorAction.CreateCommentNode));
            menu.ShowAsContext();
        }

        void HandleContextMenu(object action)
        {
            DispatchMenuItemEvent(action, BuildEvent(null));
        }
    }
}
