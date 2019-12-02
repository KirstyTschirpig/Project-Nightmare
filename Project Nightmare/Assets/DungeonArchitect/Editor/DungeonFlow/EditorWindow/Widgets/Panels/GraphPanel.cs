using DungeonArchitect.Editors.UI.Widgets;
using DungeonArchitect.Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Editors.DungeonFlow
{

    public class GraphPanel<TGraphEditor> : WidgetBase where TGraphEditor : GraphEditor
    {
        IWidget host;

        public BorderWidget Border;
        public ToolbarWidget FloatingToolbar;
        public ToolbarWidget Toolbar;
        public TGraphEditor GraphEditor;

        readonly static string BTN_FOCUS_ON_GRAPH = "FocusOnGraph";

        public GraphPanel(Graph graph, Object assetObject)
            : this(graph, assetObject, null)
        {
        }

        public GraphPanel(Graph graph, Object assetObject, ToolbarWidget toolbar)
        {
            GraphEditor = ScriptableObject.CreateInstance<TGraphEditor>();
            GraphEditor.Init(graph, Rect.zero, assetObject);

            FloatingToolbar = new ToolbarWidget();
            FloatingToolbar.ButtonSize = 24;
            FloatingToolbar.Padding = 0;
            FloatingToolbar.Background = new Color(0, 0, 0, 0);
            FloatingToolbar.AddButton(BTN_FOCUS_ON_GRAPH, DungeonEditorResources.ICON_ZOOMFIT_16x);

            FloatingToolbar.ButtonPressed += Toolbar_ButtonPressed;

            IWidget widget = new OverlayPanelWidget()
                        .AddWidget(new HighlightWidget()
                            .SetContent(GraphEditor)
                            .SetObjectOfInterest(graph)
                        )
                        .AddWidget(FloatingToolbar, OverlayPanelHAlign.Right, OverlayPanelVAlign.Top, new Vector2(24, 24), new Vector2(10, 10));

            if (toolbar != null)
            {
                var toolbarSize = new Vector2(
                    toolbar.Padding * 2 + toolbar.ButtonSize * toolbar.buttons.Count, 
                    toolbar.Padding * 2 + toolbar.ButtonSize);

                IWidget toolWidget = new StackPanelWidget(StackPanelOrientation.Horizontal)
                                    .AddWidget(toolbar, toolbarSize.x)
                                    .AddWidget(new NullWidget())
                                    ;

                toolWidget = new BorderWidget(toolWidget)
                    .SetPadding(0, 0, 0, 0)
                    .SetDrawOutline(false)
                    .SetColor(new Color(0, 0, 0, 0.25f));

                widget = new StackPanelWidget(StackPanelOrientation.Vertical)
                        .AddWidget(toolWidget, toolbarSize.y)
                        .AddWidget(widget);
            }

            Border = new BorderWidget()
                   .SetContent(widget);

            host = Border;
        }

        private void Toolbar_ButtonPressed(WidgetContext context, string id)
        {
            if (context.inputManager != null)
            {
                context.inputManager.RequestFocus(GraphEditor);
            }

            if (id == BTN_FOCUS_ON_GRAPH)
            {
                GraphEditor.FocusCameraOnBestFit();
            }
        }

        public override void UpdateWidget(WidgetContext context, Rect bounds)
        {
            base.UpdateWidget(context, bounds);

            if (host != null)
            {
                var childBounds = new Rect(Vector2.zero, bounds.size);
                host.UpdateWidget(context, childBounds);
            }
        }

        public override void Draw(WidgetContext context)
        {
            host.Draw(context);
        }

        public override bool IsCompositeWidget()
        {
            return true;
        }

        public override IWidget[] GetChildWidgets()
        {
            return new[] { host };
        }
    }
}
