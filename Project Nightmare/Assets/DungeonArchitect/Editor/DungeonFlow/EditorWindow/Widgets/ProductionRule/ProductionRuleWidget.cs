using DungeonArchitect.Editors.UI.Widgets;
using DungeonArchitect.Grammar;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DungeonArchitect.Editors.DungeonFlow
{
    public class ProductionRuleWidgetRHSState
    {
        public GraphPanel<DungeonFlowGrammarGraphEditor> GraphPanel;
        public WeightedGrammarGraph WeightedGraph;
        public ProductionRuleRHSTitleWidget WidgetTitle;
    }

    public class ProductionRuleWidget : WidgetBase
    {
        DungeonFlowAsset flowAsset;
        GrammarProductionRule productionRule;
        GraphPanel<DungeonFlowGrammarGraphEditor> LHSGraphPanel;
        List<ProductionRuleWidgetRHSState> RHSEditorStates = new List<ProductionRuleWidgetRHSState>();

        IWidget layout;
        StackPanelWidget panel;
        bool layoutDirty = false;

        public void Init(DungeonFlowAsset flowAsset, GrammarProductionRule productionRule)
        {
            this.flowAsset = flowAsset;
            this.productionRule = productionRule;

            if (productionRule != null)
            {
                var LHSGraph = productionRule ? productionRule.LHSGraph : null;
                LHSGraphPanel = new GraphPanel<DungeonFlowGrammarGraphEditor>(LHSGraph, flowAsset);
                LHSGraphPanel.GraphEditor.SetBranding("LHS");
                LHSGraphPanel.Border.SetTitleWidget(new ProductionRuleLHSTitleWidget());
                LHSGraphPanel.Border.SetColor(new Color(0.3f, 0.3f, 0.3f));

                RHSEditorStates = new List<ProductionRuleWidgetRHSState>();
                foreach (var RHSGraph in productionRule.RHSGraphs)
                {
                    AddRHSState(RHSGraph);
                }
            }

            BuildLayout();
        }

        public GraphEditor[] GetGraphEditors()
        {
            var graphEditors = new List<GraphEditor>();
            graphEditors.Add(LHSGraphPanel.GraphEditor);
            foreach (var rhsState in RHSEditorStates)
            {
                graphEditors.Add(rhsState.GraphPanel.GraphEditor);
            }
            return graphEditors.ToArray();
        }

        void BuildLayout()
        {
            float PADDING = 10;
            float GRAPH_HEIGHT = 300;

            panel = new StackPanelWidget(StackPanelOrientation.Vertical);

            panel.AddWidget(LHSGraphPanel, GRAPH_HEIGHT);
            panel.AddWidget(new NullWidget(), PADDING);
            foreach (var rhsEditorState in RHSEditorStates)
            {
                panel.AddWidget(rhsEditorState.GraphPanel, GRAPH_HEIGHT);
                panel.AddWidget(new NullWidget(), PADDING);
            }

            var addRhsButton = new ButtonWidget(new GUIContent("Add RHS"))
                    .SetColor(new Color(0.8f, 0.8f, 0.8f));
            addRhsButton.ButtonPressed += AddRhsButton_ButtonPressed;

            panel.AddWidget(new HighlightWidget()
                .SetContent(addRhsButton)
                .SetObjectOfInterest(DungeonFlowEditorHighlightID.ProductionAddRHSButton)
            , 30);

            layout = new BorderWidget(new ScrollPanelWidget(panel, false))
                .SetTitleGetter(GetProductionTitle)
                .SetTitleFontSize(16)
                .SetTitleColor(Color.black)
                .SetColor(new Color(.4f, 0.4f, 0.4f, 1))
                .SetDrawOutline(false)
                .SetPadding(5, 30, 5, 5);

        }

        string GetProductionTitle()
        {
            if (productionRule != null)
            {
                return "Production Rule: " + productionRule.ruleName;
            }
            else
            {
                return "Production Rule";
            }
        }

        private void AddRhsButton_ButtonPressed()
        {
            AddNewRHSState();
            layoutDirty = true;
        }

        ProductionRuleWidgetRHSState AddNewRHSState()
        {
            WeightedGrammarGraph RHSGraph = DungeonFlowEditorUtils.AddProductionRuleRHS(flowAsset, productionRule);
            return AddRHSState(RHSGraph);
        }

        ProductionRuleWidgetRHSState AddRHSState(WeightedGrammarGraph RHSGraph)
        {
            if (RHSGraph == null)
            {
                return null;
            }
            var state = new ProductionRuleWidgetRHSState();

            var rhsTitle = new ProductionRuleRHSTitleWidget();
            rhsTitle.State = state;
            rhsTitle.DeletePressed += RhsTitle_DeletePressed;

            var RHSGraphPanel = new GraphPanel<DungeonFlowGrammarGraphEditor>(RHSGraph.graph, flowAsset);
            RHSGraphPanel.GraphEditor.SetBranding("RHS");
            RHSGraphPanel.Border.SetTitleWidget(rhsTitle);
            RHSGraphPanel.Border.SetColor(new Color(0.3f, 0.3f, 0.3f));

            state.GraphPanel = RHSGraphPanel;
            state.WeightedGraph = RHSGraph;
            state.WidgetTitle = rhsTitle;

            RHSEditorStates.Add(state);

            return state;
        }

        private void RhsTitle_DeletePressed(ProductionRuleWidgetRHSState state)
        {
            string message = "Are you sure you want to delete this RHS graph?";
            bool removeItem = EditorUtility.DisplayDialog("Delete RHS Graph?", message, "Delete", "Cancel");
            if (removeItem)
            {
                RemoveRHSState(state);
                layoutDirty = true;
            }
        }

        void RemoveRHSState(ProductionRuleWidgetRHSState state)
        {
            RHSEditorStates.Remove(state);
            state.GraphPanel.GraphEditor.OnDestroy();

            DungeonFlowEditorUtils.RemoveProductionRuleRHS(productionRule, state.WeightedGraph);
        }

        public override void UpdateWidget(WidgetContext context, Rect bounds)
        {
            base.UpdateWidget(context, bounds);

            if (productionRule == null)
            {
                return;
            }

            if (RHSEditorStates == null)
            {
                RHSEditorStates = new List<ProductionRuleWidgetRHSState>();
            }

            if (layoutDirty)
            {
                BuildLayout();
                layoutDirty = false;
            }

            layout.UpdateWidget(context, bounds);
        }

        public override void Draw(WidgetContext context)
        {
            if (flowAsset == null || productionRule == null)
            {
                return;
            }

            layout.Draw(context);
        }

        public override bool IsCompositeWidget()
        {
            return true;
        }

        public override IWidget[] GetChildWidgets()
        {
            return new[] { layout };
        }
    }
}
