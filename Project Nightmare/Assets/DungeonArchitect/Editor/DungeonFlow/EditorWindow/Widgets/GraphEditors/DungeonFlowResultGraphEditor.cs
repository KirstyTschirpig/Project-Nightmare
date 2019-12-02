using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using DungeonArchitect;
using DungeonArchitect.Graphs;
using DungeonArchitect.Grammar;
using DungeonArchitect.RuntimeGraphs;
using UnityEditor;

namespace DungeonArchitect.Editors.DungeonFlow
{
    public enum DungeonFlowResultGraphEditorAction
    {
        CreateTaskNode,
        CreateCommentNode
    }

    public class DungeonFlowResultGraphEditor : GraphEditor
    {
        [SerializeField]
        public DungeonFlowResultGraphEditorConfig ResultGraphPanelConfig { get; private set; }

        public override void Init(Graph graph, Rect editorBounds, UnityEngine.Object assetObject)
        {
            base.Init(graph, editorBounds, assetObject);
            LinkRenderMode = GraphLinkRendererMode.StraightLines;
            if (ResultGraphPanelConfig == null)
            {
                ResultGraphPanelConfig = CreateInstance<DungeonFlowResultGraphEditorConfig>();
                ResultGraphPanelConfig.layoutConfig.separation = new Vector2(130, 100);
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();

            ResultGraphPanelConfig = CreateInstance<DungeonFlowResultGraphEditorConfig>();
        }

        protected override GraphContextMenu CreateContextMenu()
        {
            return new DungeonFlowResultGraphContextMenu(); 
        }

        protected override void InitializeNodeRenderers(GraphNodeRendererFactory nodeRenderers)
        {
            nodeRenderers.RegisterNodeRenderer(typeof(GrammarTaskNode), new GrammarTaskNodeRenderer());
            nodeRenderers.RegisterNodeRenderer(typeof(CommentNode), new CommentNodeRenderer(EditorStyle.commentTextColor));
        }

        protected override void OnMenuItemClicked(object userdata, GraphContextMenuEvent e)
        {
            var action = (DungeonFlowResultGraphEditorAction)userdata;

            var mouseScreen = lastMousePosition;
            if (action == DungeonFlowResultGraphEditorAction.CreateTaskNode)
            {
                //CreateSpatialNodeAtMouse<SCRuleNode>(mouseScreen);
            }
            else if (action == DungeonFlowResultGraphEditorAction.CreateCommentNode)
            {
                CreateCommentNode(mouseScreen);
            }
        }

        public override void OnNodeSelectionChanged()
        {
            base.OnNodeSelectionChanged();

            // Fetch all selected nodes
            var selectedNodes = from node in graph.Nodes
                                where node.Selected
                                select node;

            if (selectedNodes.Count() == 0)
            {
                Selection.activeObject = ResultGraphPanelConfig;
            }
            
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

        public override GraphSchema GetGraphSchema()
        {
            return new DungeonFlowGrammarGraphSchema();
        }

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
            editorStyle.branding = "Result Graph";
            editorStyle.backgroundColor = new Color(0.15f, 0.2f, 0.15f);
            return editorStyle;
        }

        void ClearActiveGraph()
        {
            // Clear the existing graphs
            DeleteNodes(graph.Nodes.ToArray());
            graph.Nodes.Clear();
            graph.Links.Clear();

        }
        public void RefreshGraph(Graph graph, GrammarRuntimeGraph runtimeGraph)
        {
            SetGraph(graph);

            ClearActiveGraph();

            var map = new Dictionary<RuntimeGraphNode<GrammarRuntimeGraphNodeData>, GraphNode>();
            // Add nodes
            foreach (var runtimeNode in runtimeGraph.Nodes)
            {
                var screenCoord = camera.WorldToScreen(runtimeNode.Position);
                var graphNode = CreateNode<GrammarTaskNode>(screenCoord);
                graphNode.NodeType = runtimeNode.Payload.nodeType;
                graphNode.DisplayExecutionIndex = false;

                map.Add(runtimeNode, graphNode);
            }

            // Add links
            {
                foreach (var runtimeNode in runtimeGraph.Nodes)
                {
                    foreach (var outgoingRuntimeNode in runtimeNode.Outgoing)
                    {
                        GraphNode srcGraphNode = map.ContainsKey(runtimeNode) ? map[runtimeNode] : null;
                        GraphNode dstGraphNode = map.ContainsKey(outgoingRuntimeNode) ? map[outgoingRuntimeNode] : null;
                        if (srcGraphNode == null || dstGraphNode == null || srcGraphNode.OutputPin == null || dstGraphNode.InputPin == null)
                        {
                            Debug.LogWarning("Cannot create link in result graph due to invalid node state");
                            continue;
                        }
                        CreateLinkBetweenPins(srcGraphNode.OutputPin, dstGraphNode.InputPin);
                    }
                }
            }

            FocusCameraOnBestFit();
        }
    }
}
