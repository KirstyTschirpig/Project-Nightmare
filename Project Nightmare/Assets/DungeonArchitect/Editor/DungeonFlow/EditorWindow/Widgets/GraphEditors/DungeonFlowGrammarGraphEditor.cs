using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DungeonArchitect.Grammar;
using DungeonArchitect.Graphs;
using DungeonArchitect.Editors.UI.Widgets;
using UnityEditor;

namespace DungeonArchitect.Editors.DungeonFlow
{
    public enum DungeonFlowGrammarGraphEditorAction
    {
        CreateTaskNode,
        CreateWildcard,
        CreateCommentNode
    }

    public class DungeonFlowGrammarGraphEditorContextMenuData
    {
        public DungeonFlowGrammarGraphEditorContextMenuData(DungeonFlowGrammarGraphEditorAction action)
        {
            this.Action = action;
        }

        public DungeonFlowGrammarGraphEditorContextMenuData(DungeonFlowGrammarGraphEditorAction action, object userData)
        {
            this.Action = action;
            this.UserData = userData;
        }

        public DungeonFlowGrammarGraphEditorAction Action;
        public object UserData;
    }

    public class DungeonFlowGrammarGraphSchema : GraphSchema
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

    public class DungeonFlowGrammarGraphEditor : GraphEditor
    {
        public DungeonFlowAsset FlowAsset { get; private set; }

        public void SetBranding(string branding)
        {
            if (EditorStyle != null)
            {
                EditorStyle.branding = branding;
            }
        }

        public override void Init(Graph graph, Rect editorBounds, UnityEngine.Object assetObject)
        {
            FlowAsset = assetObject as DungeonFlowAsset;
            LinkRenderMode = GraphLinkRendererMode.StraightLines;

            base.Init(graph, editorBounds, assetObject);
        }

        T GetDragData<T>() where T : Object
        {
            var dragDropData = DragAndDrop.GetGenericData(NodeListViewConstants.DragDropID);
            if (dragDropData != null && dragDropData is T)
            {
                return dragDropData as T;
            }
            return null;
        }

        public override GraphSchema GetGraphSchema()
        {
            return new DungeonFlowGrammarGraphSchema();
        }

        public override void OnNodeSelectionChanged()
        {
            base.OnNodeSelectionChanged();
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
            // We are dragging. Check if we support this data type
            return GetDragData<GrammarNodeType>() != null;
        }
        
        public override void Draw(WidgetContext context)
        {
            base.Draw(context);
            var bounds = new Rect(Vector2.zero, WidgetBounds.size);

            bool isDragging = (context.inputManager != null && context.inputManager.IsDragDrop);
            if (isDragging && IsDragDataSupported())
            {
                // Show the drag drop overlay
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
            var nodeType = GetDragData<GrammarNodeType>();
            if (nodeType != null)
            {
                if (context.inputManager != null)
                {
                    context.inputManager.RequestFocus(this);
                }

                CreateNewTaskNode(nodeType, e.mousePosition, true);
            }

            DragAndDrop.AcceptDrag();
        }

        int FindNextAvailableIndex()
        {
            if (graph.Nodes.Count == 0)
            {
                return 0;
            }

            // Find an appropriate execution index for this node
            var usedIndices = new HashSet<int>();
            foreach (var graphNode in graph.Nodes)
            {
                if (graphNode is GrammarTaskNode)
                {
                    var taskNode = graphNode as GrammarTaskNode;
                    usedIndices.Add(taskNode.executionIndex);
                }
            }

            for (int i = 0; i < usedIndices.Count + 1; i++)
            {
                if (!usedIndices.Contains(i))
                {
                    return i;
                }
            }
            return 0;
        }

        GrammarTaskNode CreateNewTaskNode(GrammarNodeType nodeType, Vector2 mousePosition, bool selectAfterCreate)
        {
            int index = FindNextAvailableIndex();
            var node = CreateNode<GrammarTaskNode>(mousePosition);
            node.NodeType = nodeType;
            node.executionIndex = index;
            node.DisplayExecutionIndex = true;

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

            if (selectAfterCreate)
            {
                BringToFront(node);
                SelectNode(node);
            }

            return node;
        }

        protected override GraphContextMenu CreateContextMenu()
        {
            return new DungeonFlowGrammarGraphContextMenu();
        }

        protected override void InitializeNodeRenderers(GraphNodeRendererFactory nodeRenderers)
        {
            nodeRenderers.RegisterNodeRenderer(typeof(GrammarTaskNode), new GrammarTaskNodeRenderer());
            nodeRenderers.RegisterNodeRenderer(typeof(CommentNode), new CommentNodeRenderer(EditorStyle.commentTextColor));
        }

        protected override void OnMenuItemClicked(object userdata, GraphContextMenuEvent e)
        {
            var data = (DungeonFlowGrammarGraphEditorContextMenuData)userdata;

            var mouseScreen = lastMousePosition;
            if (data.Action == DungeonFlowGrammarGraphEditorAction.CreateTaskNode)
            {
                if (data.UserData != null && data.UserData is GrammarNodeType)
                {
                    var nodeType = data.UserData as GrammarNodeType;
                    var node = CreateNewTaskNode(nodeType, lastMousePosition, true);
                    CreateLinkBetweenPins(e.sourcePin, node.InputPin);
                }
            }
            else if (data.Action == DungeonFlowGrammarGraphEditorAction.CreateWildcard)
            {
                var nodeType = FlowAsset.wildcardNodeType;
                if (nodeType != null)
                {
                    var node = CreateNewTaskNode(nodeType, lastMousePosition, true);
                    CreateLinkBetweenPins(e.sourcePin, node.InputPin);
                }
            }
            else if (data.Action == DungeonFlowGrammarGraphEditorAction.CreateCommentNode)
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
            editorStyle.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            return editorStyle;
        }

    }
}
