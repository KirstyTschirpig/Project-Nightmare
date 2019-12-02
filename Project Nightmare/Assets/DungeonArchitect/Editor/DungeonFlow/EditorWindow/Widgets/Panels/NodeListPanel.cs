using DungeonArchitect.Editors.UI.Widgets;
using DungeonArchitect.Grammar;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DungeonArchitect.Editors.DungeonFlow
{
    class NodeListViewConstants
    {
        public static readonly string DragDropID = "NodeTypeDragOp";
        public static readonly Color ThemeColor = new Color(0.2f, 0.3f, 0.3f);
    }

    public class NodeListViewItem : ListViewTextItemWidget
    {
        HighlightWidget highlight;

        public NodeListViewItem(GrammarNodeType nodeType)
            : base(nodeType, () => nodeType.nodeName)
        {
            DragDropEnabled = true;
            DragStart += NodeListViewItem_DragStart;

            highlight = new HighlightWidget()
                .SetContent(new NullWidget())
                .SetObjectOfInterest(nodeType);
        }

        private void NodeListViewItem_DragStart(Event e, WidgetContext context)
        {
            DragAndDrop.SetGenericData(NodeListViewConstants.DragDropID, ItemData);
        }

        public override bool IsCompositeWidget()
        {
            return true;
        }

        public override IWidget[] GetChildWidgets()
        {
            var children = new List<IWidget>();
            {
                IWidget[] baseChildren = base.GetChildWidgets();
                if (baseChildren != null)
                {
                    children.AddRange(baseChildren);
                }
            }
            children.Add(highlight);
            return children.ToArray();
        }

        public override void Draw(WidgetContext context)
        {
            base.Draw(context);

            highlight.Draw(context);
        }

        public override void UpdateWidget(WidgetContext context, Rect bounds)
        {
            base.UpdateWidget(context, bounds);

            highlight.UpdateWidget(context, bounds);
        }
    }

    public class NodeListViewSource : ListViewSource<GrammarNodeType>
    {
        DungeonFlowAsset flowAsset;
        public NodeListViewSource(DungeonFlowAsset flowAsset)
        {
            this.flowAsset = flowAsset;
        }
        public override GrammarNodeType[] GetItems()
        {
            return flowAsset != null ? flowAsset.nodeTypes : null;
        }

        public override IWidget CreateWidget(GrammarNodeType item)
        {
            var itemWidget = new NodeListViewItem(item);
            itemWidget.TextStyle.fontSize = 16;

            itemWidget.SelectedTextStyle = new GUIStyle(itemWidget.TextStyle);
            itemWidget.SelectedTextStyle.normal.textColor = Color.black;
            itemWidget.SelectedColor = NodeListViewConstants.ThemeColor * 2.0f;

            return itemWidget;
        }
    }

    public class NodeListPanel : WidgetBase
    {
        DungeonFlowAsset flowAsset;

        IWidget host;

        public ListViewWidget<GrammarNodeType> ListView;
        ToolbarWidget toolbar;


        readonly static string BTN_ADD_ITEM = "AddItem";
        readonly static string BTN_REMOVE_ITEM = "RemoveItem";
        readonly static string BTN_MOVE_UP = "MoveUp";
        readonly static string BTN_MOVE_DOWN= "MoveDown";

        public NodeListPanel(DungeonFlowAsset flowAsset)
        {
            this.flowAsset = flowAsset;

            toolbar = new ToolbarWidget();
            toolbar.ButtonSize = 24;
            toolbar.Padding = 4;
            toolbar.Background = new Color(0, 0, 0, 0);
            toolbar.AddButton(BTN_ADD_ITEM, DungeonEditorResources.ICON_PLUS_16x);
            toolbar.AddButton(BTN_REMOVE_ITEM, DungeonEditorResources.ICON_CLOSE_16x);
            toolbar.AddButton(BTN_MOVE_UP, DungeonEditorResources.ICON_MOVEUP_16x);
            toolbar.AddButton(BTN_MOVE_DOWN, DungeonEditorResources.ICON_MOVEDOWN_16x);
            toolbar.ButtonPressed += Toolbar_ButtonPressed;
            var toolbarSize = new Vector2(toolbar.Padding * 2 + toolbar.ButtonSize * 4, toolbar.Padding * 2 + toolbar.ButtonSize);

            ListView = new ListViewWidget<GrammarNodeType>();
            ListView.ItemHeight = 45;
            ListView.Bind(new NodeListViewSource(flowAsset));

            IWidget toolWidget = new StackPanelWidget(StackPanelOrientation.Horizontal)
                                .AddWidget(new NullWidget())
                                .AddWidget(toolbar, toolbarSize.x);

            toolWidget = new BorderWidget(toolWidget)
                .SetPadding(0, 0, 0, 0)
                .SetDrawOutline(false)
                .SetColor(new Color(0, 0, 0, 0.25f));

            var border = new BorderWidget()
                   .SetTitle("Node List")
                   .SetColor(NodeListViewConstants.ThemeColor)
                   .SetContent(
                        new StackPanelWidget(StackPanelOrientation.Vertical)
                        .AddWidget(toolWidget, toolbarSize.y)
                        .AddWidget(new HighlightWidget()
                            .SetContent(ListView)
                            .SetObjectOfInterest(DungeonFlowEditorHighlightID.NodePanel)
                        )
                    );

            host = border;
        }

        private void Toolbar_ButtonPressed(WidgetContext context, string id)
        {
            if (flowAsset == null)
            {
                return;
            }

            if (id == BTN_ADD_ITEM)
            {
                var nodeType = DungeonFlowEditorUtils.AddNodeType(flowAsset, "Task");
                ListView.NotifyDataChanged();

                int index = System.Array.FindIndex(flowAsset.nodeTypes, t => t == nodeType);
                ListView.SetSelectedIndex(index);
            }
            else if (id == BTN_REMOVE_ITEM)
            {
                var nodeType = ListView.GetSelectedItem();
                if (nodeType != null)
                {
                    string message = string.Format("Are you sure you want to delete the node type \'{0}\'?", nodeType.nodeName);
                    bool removeItem = EditorUtility.DisplayDialog("Delete Node Type?", message, "Delete", "Cancel");
                    if (removeItem)
                    {
                        int index = System.Array.FindIndex(flowAsset.nodeTypes, t => t == nodeType);
                        DungeonFlowEditorUtils.RemoveNodeType(flowAsset, nodeType);
                        ListView.NotifyDataChanged();

                        if (index >= flowAsset.nodeTypes.Length)
                        {
                            index = flowAsset.nodeTypes.Length - 1;
                        }
                        ListView.SetSelectedIndex(index);
                    }
                }
            }
            else if (id == BTN_MOVE_UP)
            {
                var nodeType = ListView.GetSelectedItem();
                var list = new List<GrammarNodeType>(flowAsset.nodeTypes);
                int index = list.IndexOf(nodeType);
                if (index > 0)
                {
                    list.RemoveAt(index);
                    index--;
                    list.Insert(index, nodeType);
                    flowAsset.nodeTypes = list.ToArray();

                    ListView.NotifyDataChanged();
                    ListView.SetSelectedIndex(index);
                }
            }
            else if (id == BTN_MOVE_DOWN)
            {
                var nodeType = ListView.GetSelectedItem();
                var list = new List<GrammarNodeType>(flowAsset.nodeTypes);
                int index = list.IndexOf(nodeType);
                if (index + 1 < list.Count)
                {
                    list.RemoveAt(index);
                    index++;
                    list.Insert(index, nodeType);
                    flowAsset.nodeTypes = list.ToArray();

                    ListView.NotifyDataChanged();
                    ListView.SetSelectedIndex(index);
                }
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

        public override void HandleInput(Event e, WidgetContext context)
        {
            host.HandleInput(e, context);
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
