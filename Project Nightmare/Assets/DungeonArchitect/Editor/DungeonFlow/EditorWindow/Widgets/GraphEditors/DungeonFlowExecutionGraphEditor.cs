using DungeonArchitect.Editors.UI.Widgets;
using DungeonArchitect.Grammar;
using DungeonArchitect.Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.DungeonFlow
{
    public enum DungeonFlowExecutionGraphEditorAction
    {
        CreateRuleNode,
        CreateCommentNode
    }

    public class DungeonFlowExecutionGraphEditorMenuData
    {
        public DungeonFlowExecutionGraphEditorMenuData(DungeonFlowExecutionGraphEditorAction action)
        {
            this.Action = action;
        }
        public DungeonFlowExecutionGraphEditorMenuData(DungeonFlowExecutionGraphEditorAction action, GrammarProductionRule rule)
        {
            this.Action = action;
            this.Rule = rule;
        }

        public DungeonFlowExecutionGraphEditorAction Action;
        public GrammarProductionRule Rule;
    }

    public class DungeonFlowExecutionGraphSchema : GraphSchema
    {
        public override bool CanCreateLink(GraphPin output, GraphPin input, out string errorMessage)
        {
            errorMessage = "";
            if (output == null || input == null)
            {
                errorMessage = "Invalid connection";
                return false;
            }

            if (input.Node != null)
            {
                input = input.Node.InputPin;
            }

            var sourceNode = output.Node;
            var destNode = input.Node;

            if (destNode is GrammarExecEntryNode)
            {
                errorMessage = "Not Allowed: Cannot connect to entry node";
                return false;
            }

            // Make sure we don't already have this connection
            foreach (var link in output.GetConntectedLinks())
            {
                if (link.Input == input)
                {
                    errorMessage = "Not Allowed: Already connected";
                    return false;
                }
            }

            return true;
        }
    }

    public class DungeonFlowExecutionGraphEditor : GraphEditor
    {
        public DungeonFlowAsset FlowAsset { get; private set; }
        public override void Init(Graph graph, Rect editorBounds, UnityEngine.Object assetObject)
        {
            FlowAsset = assetObject as DungeonFlowAsset;
            LinkRenderMode = GraphLinkRendererMode.StraightLines;

            base.Init(graph, editorBounds, assetObject);
        }

        GrammarProductionRule GetDragData()
        {
            var dragDropData = DragAndDrop.GetGenericData(RuleListViewConstants.DragDropID);
            if (dragDropData != null && dragDropData is GrammarProductionRule)
            {
                return dragDropData as GrammarProductionRule;
            }
            return null;
        }

        public override GraphSchema GetGraphSchema()
        {
            return new DungeonFlowExecutionGraphSchema();
        }

        public override T CreateLink<T>(Graph graph, GraphPin output, GraphPin input)
        {
            if (input != null && input.Node != null)
            {
                input = input.Node.InputPin;
            }

            // If we have a link in the opposite direction, then break that link
            var sourceNode = output.Node;
            var destNode = input.Node;
            if (sourceNode != null && destNode != null)
            {
                var links = destNode.OutputPin.GetConntectedLinks();
                foreach (var link in links)
                {
                    if (link.Output.Node == input.Node && link.Input.Node == output.Node)
                    {
                        GraphOperations.DestroyLink(link);
                    }
                }

                if (sourceNode is GrammarExecEntryNode)
                {
                    // Destroy all outgoing links first
                    var outgoingLinks = output.GetConntectedLinks();
                    foreach (var link in outgoingLinks)
                    {
                        GraphOperations.DestroyLink(link);
                    }
                }
            }

            if (input.Node != null)
            {
                input = input.Node.InputPin;
                if (input != null)
                {
                    return base.CreateLink<T>(graph, output, input);
                }
            }
            return null;
        }

        bool IsDragDataSupported()
        {
            return GetDragData() != null;
        }

        public override void Draw(WidgetContext context)
        {
            base.Draw(context);


            bool isDragging = (context.inputManager != null && context.inputManager.IsDragDrop);
            if (isDragging && IsDragDataSupported())
            {
                // Show the drag drop overlay
                var bounds = new Rect(Vector2.zero, WidgetBounds.size);
                var dragOverlayColor = new Color(1, 0, 0, 0.25f);
                EditorGUI.DrawRect(bounds, dragOverlayColor);
            }
        }

        public override void HandleInput(Event e, WidgetContext context)
        {
            base.HandleInput(e, context);

            switch (e.type)
            {
                case EventType.DragUpdated:
                    if (IsDragDataSupported())
                    {
                        HandleDragUpdate(e, context);
                    }
                    break;

                case EventType.DragPerform:
                    if (IsDragDataSupported())
                    {
                        HandleDragPerform(e, context);
                    }
                    break;
            }
        }

        private void HandleDragUpdate(Event e, WidgetContext context)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        }

        private void HandleDragPerform(Event e, WidgetContext context)
        {
            // TODO: Create a node here
            var rule = GetDragData();
            if (rule != null)
            {
                if (context.inputManager != null)
                {
                    context.inputManager.RequestFocus(this);
                }

                var ruleNode = CreateNewExecRuleNode(rule, e.mousePosition);
                SelectNode(ruleNode);
            }

            DragAndDrop.AcceptDrag();
        }

        GrammarExecRuleNode CreateNewExecRuleNode(GrammarProductionRule rule, Vector2 mousePosition)
        {
            var node = CreateNode<GrammarExecRuleNode>(mousePosition);
            node.rule = rule;

            // Adjust the initial position of the placed node
            {
                var nodeRenderer = nodeRenderers.GetRenderer(node.GetType());
                if (nodeRenderer is GrammarNodeRendererBase)
                {
                    var grammarNodeRenderer = nodeRenderer as GrammarNodeRendererBase;
                    grammarNodeRenderer.UpdateNodeBounds(node, 1.0f);
                }

                var mouseWorld = camera.ScreenToWorld(mousePosition);
                var bounds = node.Bounds;
                bounds.position = mouseWorld - bounds.size / 2.0f;
                node.Bounds = bounds;
            }

            return node;

        }

        protected override GraphContextMenu CreateContextMenu()
        {
            return new DungeonFlowExecutionGraphContextMenu();
        }

        protected override void InitializeNodeRenderers(GraphNodeRendererFactory nodeRenderers)
        {
            nodeRenderers.RegisterNodeRenderer(typeof(CommentNode), new CommentNodeRenderer(EditorStyle.commentTextColor));
            nodeRenderers.RegisterNodeRenderer(typeof(GrammarExecRuleNode), new GrammarExecRuleNodeRenderer());
            nodeRenderers.RegisterNodeRenderer(typeof(GrammarExecEntryNode), new GrammarExecEntryNodeRenderer());
        }

        protected override void OnMenuItemClicked(object userdata, GraphContextMenuEvent e)
        {
            var data = userdata as DungeonFlowExecutionGraphEditorMenuData;

            var mouseScreen = lastMousePosition;
            if (data.Action == DungeonFlowExecutionGraphEditorAction.CreateRuleNode)
            {
                var ruleNode = CreateNewExecRuleNode(data.Rule, LastMousePosition);
                if (ruleNode != null)
                {
                    CreateLinkBetweenPins(e.sourcePin, ruleNode.InputPin);
                    SelectNode(ruleNode);
                }
            }
            else if (data.Action == DungeonFlowExecutionGraphEditorAction.CreateCommentNode)
            {
                CreateCommentNode(mouseScreen);
            }

        }

        protected override void DrawHUD(Rect bounds) { }

        void CreateCommentNode(Vector2 screenPos)
        {
            var worldPos = camera.ScreenToWorld(screenPos);
            var commentNode = CreateNode<CommentNode>(worldPos);
            commentNode.Position = worldPos;
            commentNode.background = new Color(0.224f, 1.0f, 0.161f, 0.7f);
            BringToFront(commentNode);
            SelectNode(commentNode);
        }

        protected override string GetGraphNotInitializedMessage()
        {
            return "Graph not initialize";
        }

        protected override GraphEditorStyle CreateEditorStyle()
        {
            var editorStyle = base.CreateEditorStyle();
            editorStyle.branding = "Execution Graph";
            editorStyle.backgroundColor = new Color(0.15f, 0.15f, 0.2f);
            return editorStyle;
        }
    }
}
