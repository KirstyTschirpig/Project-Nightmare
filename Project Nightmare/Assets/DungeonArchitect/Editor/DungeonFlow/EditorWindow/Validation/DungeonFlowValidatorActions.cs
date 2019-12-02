using DungeonArchitect.Editors.UI.Widgets;
using DungeonArchitect.Grammar;
using DungeonArchitect.Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Editors.DungeonFlow
{
    public interface IDungeonFlowValidatorAction
    {
        void Execute(DungeonFlowEditorWindow editor);
    }

    public abstract class ValidatorActionBase : IDungeonFlowValidatorAction
    {
        public abstract void Execute(DungeonFlowEditorWindow editor);
    }

    public class EmptyNodeListValidatorAction : ValidatorActionBase
    {
        public override void Execute(DungeonFlowEditorWindow editor)
        {
            var layoutRoot = editor.GetRootLayout();
            DungeonFlowEditorHighlighter.HighlightObjects(layoutRoot, DungeonFlowEditorHighlightID.NodePanel);
        }
    }
    public class EmptyRuleListValidatorAction : ValidatorActionBase
    {
        public override void Execute(DungeonFlowEditorWindow editor)
        {
            var layoutRoot = editor.GetRootLayout();
            DungeonFlowEditorHighlighter.HighlightObjects(layoutRoot, DungeonFlowEditorHighlightID.RulePanel);
        }
    }

    public class InvalidExecutionGraphValidatorAction : ValidatorActionBase
    {
        public override void Execute(DungeonFlowEditorWindow editor)
        {
            var layoutRoot = editor.GetRootLayout();
            var flowAsset = editor.GetDungeonFlowAsset();
            var execGraph = (flowAsset != null) ? flowAsset.executionGraph : null;
            DungeonFlowEditorHighlighter.HighlightObjects(layoutRoot, execGraph);
        }
    }


    public class InvalidProductionRuleAction : ValidatorActionBase
    {
        GrammarProductionRule productionRule;
        public InvalidProductionRuleAction(GrammarProductionRule productionRule)
        {
            this.productionRule = productionRule;
        }
        public override void Execute(DungeonFlowEditorWindow editor)
        {
            var rulePanel = editor.GetRuleListPanel();
            rulePanel.ListView.SetSelectedItem(productionRule, true);
            DungeonFlowEditorHighlighter.HighlightObjects(rulePanel, productionRule);
        }
    }

    public class MissingProductionRuleRHSAction : ValidatorActionBase
    {
        GrammarProductionRule productionRule;
        public MissingProductionRuleRHSAction(GrammarProductionRule productionRule)
        {
            this.productionRule = productionRule;
        }
        public override void Execute(DungeonFlowEditorWindow editor)
        {
            var rulePanel = editor.GetRuleListPanel();
            rulePanel.ListView.SetSelectedItem(productionRule);
            editor.ForceUpdateWidgetFromCache(rulePanel);

            DungeonFlowEditorHighlighter.HighlightObjects(editor.GetProductionRuleWidget(), DungeonFlowEditorHighlightID.ProductionAddRHSButton);
        }
    }

    public class InvalidNodeTypeAction : ValidatorActionBase
    {
        GrammarNodeType nodeType;
        public InvalidNodeTypeAction(GrammarNodeType nodeType)
        {
            this.nodeType = nodeType;
        }
        public override void Execute(DungeonFlowEditorWindow editor)
        {
            var nodesPanel = editor.GetNodeListPanel();
            nodesPanel.ListView.SetSelectedItem(nodeType, true);
            DungeonFlowEditorHighlighter.HighlightObjects(nodesPanel, nodeType);
        }
    }

    public class InvalidProductionGraphAction : ValidatorActionBase
    {
        GrammarGraph graph;
        GrammarProductionRule productionRule;

        public InvalidProductionGraphAction(GrammarProductionRule productionRule, GrammarGraph graph)
        {
            this.productionRule = productionRule;
            this.graph = graph;
        }

        public override void Execute(DungeonFlowEditorWindow editor)
        {
            var rulePanel = editor.GetRuleListPanel();
            rulePanel.ListView.SetSelectedItem(productionRule);
            editor.ForceUpdateWidgetFromCache(rulePanel);

            var productionRuleEditor = editor.GetProductionRuleWidget();
            DungeonFlowEditorHighlighter.HighlightObjects(productionRuleEditor, graph);
        }
    }

    public class InvalidProductionGraphNodeAction : ValidatorActionBase
    {
        GraphNode node;
        GrammarProductionRule productionRule;

        public InvalidProductionGraphNodeAction(GrammarProductionRule productionRule, GraphNode node)
        {
            this.productionRule = productionRule;
            this.node = node;
        }

        GraphEditor FindGraphEditor(DungeonFlowEditorWindow editor, Graph graph)
        {
            var graphEditors = editor.GetProductionRuleWidget().GetGraphEditors();
            foreach (var graphEditor in graphEditors)
            {
                if (graphEditor.Graph == graph)
                {
                    return graphEditor;
                }
            }
            return null;
        }

        public override void Execute(DungeonFlowEditorWindow editor)
        {
            var rulePanel = editor.GetRuleListPanel();
            rulePanel.ListView.SetSelectedItem(productionRule);
            editor.ForceUpdateWidgetFromCache(rulePanel);

            var productionRuleEditor = editor.GetProductionRuleWidget();
            DungeonFlowEditorHighlighter.HighlightObjects(productionRuleEditor, node.Graph);

            var graphEditor = FindGraphEditor(editor, node.Graph);
            if (graphEditor != null)
            {
                editor.AddDeferredCommand(new EditorCommand_FocusOnGraphNode(graphEditor, node));
            }
        }
    }

    public class InvalidExecutionGraphNodeAction : ValidatorActionBase
    {
        GraphNode node;

        public InvalidExecutionGraphNodeAction(GraphNode node)
        {
            this.node = node;
        }

        public override void Execute(DungeonFlowEditorWindow editor)
        {
            var execGraphPanel = editor.GetExecGraphPanel();
            if (execGraphPanel != null)
            {
                DungeonFlowEditorHighlighter.HighlightObjects(execGraphPanel, node.Graph);

                var graphEditor = execGraphPanel.GraphEditor;
                if (graphEditor != null)
                {
                    editor.AddDeferredCommand(new EditorCommand_FocusOnGraphNode(graphEditor, node));
                }
            }
        }
    }
}
