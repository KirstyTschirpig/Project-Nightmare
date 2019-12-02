using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DungeonArchitect.Grammar;
using DungeonArchitect.Editors.UI.Widgets;
using DungeonArchitect.Graphs;
using DungeonArchitect.RuntimeGraphs.Layouts;

namespace DungeonArchitect.Editors.DungeonFlow
{
    public class DungeonFlowEditorWindow : EditorWindow
    {
        DungeonFlowAsset flowAsset;
        ProductionRuleWidget ruleEditor;
        RuleListPanel ruleListPanel;
        NodeListPanel nodeListPanel;
        ErrorListPanel errorListPanel;

        GraphPanel<DungeonFlowResultGraphEditor> resultGraphPanel; 
        GraphPanel<DungeonFlowExecutionGraphEditor> execGraphPanel;

        DungeonFlowErrorList errorList;
        IWidget layout;
        WidgetInputManager inputManager;


        List<IDeferredUICommand> deferredCommands = new List<IDeferredUICommand>();

        readonly static string CMD_EXEC_GRAMMAR = "ExecuteGrammar";

        public void Init(DungeonFlowAsset flowAsset)
        {
            titleContent = new GUIContent("Dungeon Flow Editor");

            this.flowAsset = flowAsset;
            inputManager = new WidgetInputManager();
            ruleEditor = new ProductionRuleWidget();

            // Build the result graph
            {
                var graph = flowAsset ? flowAsset.resultGraph : null;
                resultGraphPanel = new GraphPanel<DungeonFlowResultGraphEditor>(graph, flowAsset);
                resultGraphPanel.Border.SetTitle("Result Graph");
                resultGraphPanel.Border.SetColor(new Color(0.2f, 0.3f, 0.2f));
            }

            // Build the execution graph
            {
                var execToolbar = new ToolbarWidget();
                execToolbar.ButtonSize = 24;
                execToolbar.Padding = 4;
                execToolbar.Background = new Color(0, 0, 0, 0);
                execToolbar.AddButton(CMD_EXEC_GRAMMAR, DungeonEditorResources.ICON_PLAY_16x);
                execToolbar.ButtonPressed += ExecToolbar_ButtonPressed;

                var graph = flowAsset ? flowAsset.executionGraph : null;
                execGraphPanel = new GraphPanel<DungeonFlowExecutionGraphEditor>(graph, flowAsset, execToolbar);
                execGraphPanel.Border.SetTitle("Execution Graph");
                execGraphPanel.Border.SetColor(new Color(0.2f, 0.2f, 0.5f));
            }

            // Build the rule list view
            {
                ruleListPanel = new RuleListPanel(flowAsset);
                ruleListPanel.ListView.SelectionChanged += RuleListView_SelectionChanged;
                ruleListPanel.ListView.ItemClicked += RuleListView_ItemClicked;
            }

            // Build the node list view
            {
                nodeListPanel = new NodeListPanel(flowAsset);
                nodeListPanel.ListView.SelectionChanged += NodeListView_SelectionChanged;
                nodeListPanel.ListView.ItemClicked += NodeListView__ItemClicked;
            }

            // Build the error list panel
            {
                errorList = new DungeonFlowErrorList();
                errorListPanel = new ErrorListPanel(errorList);
                errorListPanel.ListView.ItemDoubleClicked += ErrorListView_ItemDoubleClicked;
            }

            BuildLayout();
            
            // Select the first rule for modification
            ruleListPanel.ListView.SetSelectedIndex(0);
        }

        private void ExecToolbar_ButtonPressed(WidgetContext context, string id)
        {
            if (id == CMD_EXEC_GRAMMAR)
            {
                ExecuteGraphGrammar();
            }
        }

        private void ExecuteGraphGrammar()
        {
            int seed = Random.Range(0, 100000);
            var processor = new GraphGrammarProcessor(flowAsset, seed);
            processor.Build();

            // Perform layout
            var layout = new RuntimeGraphLayeredLayout<GrammarRuntimeGraphNodeData>(GetResultGraphLayoutConfig());
            layout.Layout(processor.Grammar.ResultGraph);

            if (flowAsset != null && flowAsset.resultGraph != null)
            {
                resultGraphPanel.GraphEditor.RefreshGraph(flowAsset.resultGraph, processor.Grammar.ResultGraph);
            }
        }
         
        private void RuleListView_ItemClicked(GrammarProductionRule rule)
        {
            UpdateProductionRuleGraphCameras(rule);
            Selection.activeObject = rule;
        }

        private void NodeListView__ItemClicked(GrammarNodeType nodeType)
        {
            Selection.activeObject = nodeType;
        }

        private void ErrorListView_ItemDoubleClicked(DungeonFlowErrorEntry Item)
        {
            if (Item.Action != null)
            {
                Item.Action.Execute(this);
            }
        }

        private void NodeListView_SelectionChanged(GrammarNodeType nodeType)
        {
        }

        private void RuleListView_SelectionChanged(GrammarProductionRule rule)
        {
            UpdateProductionRuleGraphCameras(rule);
        }

        void UpdateProductionRuleGraphCameras(GrammarProductionRule rule)
        {
            if (ruleEditor != null)
            {
                ruleEditor.Init(flowAsset, rule);
                ruleEditor.UpdateWidget(GetWidgetContext(), ruleEditor.WidgetBounds);

                deferredCommands.Add(new EditorCommand_InitializeGraphCameras(ruleEditor));
            }
        }


        public IWidget GetRootLayout() { return layout; }
        public DungeonFlowAsset GetDungeonFlowAsset() { return flowAsset; }
        public ProductionRuleWidget GetProductionRuleWidget() { return ruleEditor; }
        public RuleListPanel GetRuleListPanel() { return ruleListPanel; }
        public NodeListPanel GetNodeListPanel() { return nodeListPanel; }
        public ErrorListPanel GetErrorListPanel() { return errorListPanel; }
        public GraphPanel<DungeonFlowExecutionGraphEditor> GetExecGraphPanel() { return execGraphPanel; }
        public GraphPanel<DungeonFlowResultGraphEditor> GetResultGraphPanel() { return resultGraphPanel; }

        public void AddDeferredCommand(IDeferredUICommand command)
        {
            deferredCommands.Add(command);
        }


        void OnGUI()
        {
            // Draw the background
            var bounds = new Rect(Vector2.zero, position.size);
            EditorGUI.DrawRect(bounds, new Color(0.5f, 0.5f, 0.5f));

            // Draw the main layout
            if (layout != null)
            {
                var context = GetWidgetContext();
                layout.UpdateWidget(context, bounds);
                layout.Draw(context);
            }
            else
            {
                BuildLayout();
            }

            ProcessDeferredCommands();

            HandleInput(Event.current);
        }


        void ProcessDeferredCommands()
        {
            // Execute the deferred UI commands
            foreach (var command in deferredCommands)
            {
                command.Execute();
            }

            deferredCommands.Clear();
        }

        void PerformValidation()
        {
            if (errorListPanel == null)
            {
                return;
            }

            int selectedIndex = errorListPanel.ListView.GetSelectedIndex();
            var scrollPosition = errorListPanel.ListView.ScrollView.ScrollPosition;

            errorList.Errors.Clear();
            DungeonFlowValidator.Validate(flowAsset, errorList);

            // Notify the list view that the data has changed and restore the selected index after a reload
            errorListPanel.ListView.NotifyDataChanged();
            errorListPanel.ListView.SetSelectedIndex(selectedIndex);
            errorListPanel.ListView.ScrollView.ScrollPosition = scrollPosition;
        }

        void BuildLayout()
        {
            layout = new Splitter(SplitterDirection.Horizontal)
                .AddWidget(
                    new Splitter(SplitterDirection.Vertical)
                    .AddWidget(ruleListPanel)
                    .AddWidget(nodeListPanel)
                )
                .AddWidget(
                    new Splitter(SplitterDirection.Vertical)
                    .AddWidget(ruleEditor, 5)
                    .AddWidget(errorListPanel)
                , 2)
                .AddWidget(
                    new Splitter(SplitterDirection.Vertical)
                    .AddWidget(execGraphPanel)
                    .AddWidget(resultGraphPanel)
                , 2)
            ;

            deferredCommands.Add(new EditorCommand_InitializeGraphCameras(layout));
        }


        void OnEnable()
        {
            this.wantsMouseMove = true;

            Init(flowAsset);

            var graphEditors = WidgetUtils.GetWidgetsOfType<GraphEditor>(layout);
            graphEditors.ForEach(g => g.OnEnable());
        }

        void OnDisable()
        {
            var graphEditors = WidgetUtils.GetWidgetsOfType<GraphEditor>(layout);
            graphEditors.ForEach(g => g.OnDisable());
        }

        void OnDestroy()
        {
            var graphEditors = WidgetUtils.GetWidgetsOfType<GraphEditor>(layout);
            graphEditors.ForEach(g => {
                g.OnDisable();
                g.OnDestroy();
            });
        }

        void Update()
        {
            var graphEditors = WidgetUtils.GetWidgetsOfType<GraphEditor>(layout);
            graphEditors.ForEach(g => g.Update());
        }

        void OnInspectorUpdate()
        {
            PerformValidation();

            Repaint();
        }

        WidgetContext GetWidgetContext()
        {
            var context = new WidgetContext();
            context.inputManager = inputManager;
            return context;
        }

        public void ForceUpdateWidgetFromCache(IWidget widget)
        {
            widget.UpdateWidget(GetWidgetContext(), widget.WidgetBounds);
        }

        void UpdateDragDropState(Event e)
        {
            if (inputManager != null)
            {
                if (e.type == EventType.DragUpdated)
                {
                    inputManager.IsDragDrop = true;
                }
                else if (e.type == EventType.DragPerform || e.type == EventType.DragExited)
                {
                    inputManager.IsDragDrop = false;
                }
            }
        }

        void HandleInput(Event e)
        {

            var context = GetWidgetContext();

            if (e.type == EventType.MouseDown || e.type == EventType.ScrollWheel)
            {
                WidgetUtils.ProcessInputFocus(e.mousePosition, inputManager, layout);
            }

            if (inputManager.IsDragDrop)
            {
                WidgetUtils.ProcessDragOperation(e, layout, context);
            }

            UpdateDragDropState(e);

            if (inputManager.FocusedWidget != null)
            {
                Vector2 resultMousePosition = Vector2.zero;
                if (WidgetUtils.BuildWidgetEvent(e.mousePosition, layout, inputManager.FocusedWidget, ref resultMousePosition))
                {
                    Event widgetEvent = new Event(e);
                    widgetEvent.mousePosition = resultMousePosition;
                    inputManager.FocusedWidget.HandleInput(widgetEvent, context);
                }
            }

            if (e.isScrollWheel)
            {
                Repaint();
            }

            switch (e.type)
            {
                case EventType.MouseMove:
                case EventType.MouseDrag:
                case EventType.MouseDown:
                case EventType.MouseUp:
                case EventType.KeyDown:
                case EventType.KeyUp:
                case EventType.MouseEnterWindow:
                case EventType.MouseLeaveWindow:
                    Repaint();
                    break;
            }
        }

        RuntimeGraphLayeredLayoutConfig GetResultGraphLayoutConfig()
        {
            DungeonFlowResultGraphEditorConfig resultGraphPanelConfig = null;
            var resultGraphEditor = resultGraphPanel.GraphEditor as DungeonFlowResultGraphEditor;
            if (resultGraphEditor != null)
            {
                resultGraphPanelConfig = resultGraphEditor.ResultGraphPanelConfig;
            }

            RuntimeGraphLayeredLayoutConfig layoutConfig
                = resultGraphPanelConfig != null
                ? resultGraphPanelConfig.layoutConfig
                : new RuntimeGraphLayeredLayoutConfig(new Vector2(120, 100));

            return layoutConfig;
        }

        [MenuItem("Window/Dungeon Architect/Dungeon Flow Editor")]
        static void ShowEditor()
        {
            DungeonFlowEditorWindow window = EditorWindow.GetWindow<DungeonFlowEditorWindow>(); 
            window.Init(null);
        }
    }

    public interface IDeferredUICommand
    {
        void Execute();
    }

    abstract class DeferredUICommandBase : IDeferredUICommand
    {
        public abstract void Execute();
    }

    class EditorCommand_InitializeGraphCameras : DeferredUICommandBase
    {
        IWidget host;
        public EditorCommand_InitializeGraphCameras(IWidget host)
        {
            this.host = host;
        }

        public override void Execute()
        {
            var graphEditors = WidgetUtils.GetWidgetsOfType<GraphEditor>(host);
            foreach (var graphEditor in graphEditors)
            {
                var bounds = new Rect(Vector2.zero, graphEditor.WidgetBounds.size);
                graphEditor.FocusCameraOnBestFit(bounds);
            }
        }
    }

    class EditorCommand_FocusOnGraphNode : DeferredUICommandBase
    {
        GraphEditor graphEditor;
        GraphNode graphNode;
        public EditorCommand_FocusOnGraphNode(GraphEditor graphEditor, GraphNode graphNode)
        {
            this.graphEditor = graphEditor;
            this.graphNode = graphNode;
        }

        public override void Execute()
        {
            graphEditor.FocusCameraOnNode(graphNode);
            graphEditor.SelectNode(graphNode);
            Selection.activeObject = graphNode;
        }
    }


    public enum DungeonFlowEditorHighlightID
    {
        RulePanel,
        NodePanel,
        ProductionAddRHSButton
    }

    class DungeonFlowEditorHighlighter
    {
        private static void TraverseTree(IWidget widget, System.Action<IWidget> visit)
        {
            if (widget == null) return;

            visit(widget);

            var children = widget.GetChildWidgets();
            if (children != null)
            {
                foreach (var child in children)
                {
                    TraverseTree(child, visit);
                }
            }
        }

        public static void HighlightObjects(IWidget root, object objectOfInterest)
        {
            if (objectOfInterest == null)
            {
                return;
            }

            TraverseTree(root, widget => {
                if (widget is HighlightWidget)
                {
                    var highlightWidget = widget as HighlightWidget;
                    var highlightObject = highlightWidget.ObjectOfInterest;
                    if (objectOfInterest.Equals(highlightObject))
                    {
                        highlightWidget.Activate();
                    }
                }
            });
        }

        public static void HighlightObjects(IWidget root, object[] objectsOfInterest)
        {
            if (objectsOfInterest == null)
            {
                return;
            }

            TraverseTree(root, widget => {
                if (widget is HighlightWidget)
                {
                    var highlightWidget = widget as HighlightWidget;
                    var highlightObject = highlightWidget.ObjectOfInterest;
                    if (objectsOfInterest.Contains(highlightObject))
                    {
                        highlightWidget.Activate();
                    }
                }
            });
        }
    }
}
