using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Editors.UI.Widgets
{
    class StackPanelNode
    {
        public IWidget Widget;
        public bool AutoSize = false;
        public float Size = 100;
    }

    public enum StackPanelOrientation
    {
        Horizontal,
        Vertical
    }

    public class StackPanelWidget : WidgetBase
    {
        List<StackPanelNode> nodes = new List<StackPanelNode>();
        StackPanelOrientation Orientation = StackPanelOrientation.Vertical;

        public StackPanelWidget(StackPanelOrientation orientation)
        {
            this.Orientation = orientation;
        }

        public StackPanelWidget AddWidget(IWidget Widget)
        {
            var node = new StackPanelNode();
            node.Widget = Widget;
            node.AutoSize = true;
            nodes.Add(node);

            return this;
        }

        public StackPanelWidget AddWidget(IWidget Widget, float size)
        {
            var node = new StackPanelNode();
            node.Widget = Widget;
            node.Size = size;   
            nodes.Add(node);

            return this;
        }

        public override Vector2 GetDesiredSize(Vector2 size)
        {
            var fixedLength = 0.0f;
            foreach (var node in nodes)
            {
                if (!node.AutoSize)
                {
                    fixedLength += node.Size;
                }
            }

            var desiredSize = size;
            if (Orientation == StackPanelOrientation.Horizontal)
            {
                desiredSize.x = Mathf.Max(desiredSize.x, fixedLength);
            }
            else if (Orientation == StackPanelOrientation.Vertical)
            {
                desiredSize.y = Mathf.Max(desiredSize.y, fixedLength);
            }

            return desiredSize;
        }

        public override void UpdateWidget(WidgetContext context, Rect bounds)
        {
            base.UpdateWidget(context, bounds);

            float availableSize = (Orientation == StackPanelOrientation.Horizontal) ? WidgetBounds.width : WidgetBounds.height;
            int autoSizedNodeCount = 0;
            // find the available size after the fixed sized nodes have been processed
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                if (!node.AutoSize)
                {
                    availableSize -= node.Size;
                }
                else
                {
                    autoSizedNodeCount++;
                }
            }

            float autoSize = autoSizedNodeCount > 0 ? Mathf.Max(0.0f, availableSize) / autoSizedNodeCount : 0;
            float offset = 0;
            foreach (var node in nodes)
            {
                float nodeSize = node.AutoSize ? autoSize : node.Size;

                var nodeBounds = (Orientation == StackPanelOrientation.Horizontal)
                    ? new Rect(offset, 0, nodeSize, WidgetBounds.height)
                    : new Rect(0, offset, WidgetBounds.width, nodeSize);

                node.Widget.UpdateWidget(context, nodeBounds);

                offset += nodeSize;
            }

        }


        public override void Draw(WidgetContext context)
        {
            foreach (var childWidget in GetChildWidgets())
            {
                WidgetUtils.DrawWidgetGroup(context, childWidget);
            }
        }

        public override bool IsCompositeWidget()
        {
            return true;
        }

        public override IWidget[] GetChildWidgets()
        {
            var children = new List<IWidget>();

            foreach (var node in nodes)
            {
                if (node.Widget != null)
                {
                    children.Add(node.Widget);
                }
            }

            return children.ToArray();
        }
    }
}
