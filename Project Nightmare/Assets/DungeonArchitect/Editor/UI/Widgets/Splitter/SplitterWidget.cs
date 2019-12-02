using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DMathUtils = DungeonArchitect.Utils.MathUtils;

namespace DungeonArchitect.Editors.UI.Widgets
{
    public enum SplitterDirection
    {
        Horizontal,
        Vertical
    }

    public class SplitterNode
    {
        public IWidget Content;
        public float Weight;
    }

    public class Splitter : WidgetBase
    {
        SplitterDirection direction;
        public SplitterDirection Direction { get { return direction; } }
        public int Padding;

        public Splitter(SplitterDirection direction)
        {
            this.direction = direction;
            Padding = 1;
        }

        private List<SplitterNode> nodes = new List<SplitterNode>();

        public Splitter AddWidget(IWidget widget)
        {
            return AddWidget(widget, 1);
        }

        public Splitter AddWidget(IWidget widget, float weight)
        {
            var node = new SplitterNode();
            node.Content = widget;
            node.Weight = weight;
            nodes.Add(node);

            return this;
        }

        public override bool IsCompositeWidget() { return true; }
        public override IWidget[] GetChildWidgets()
        {
            var widgets = new List<IWidget>();
            foreach (var node in nodes)
            {
                if (node.Content != null)
                {
                    widgets.Add(node.Content);
                }
            }
            return widgets.ToArray();
        }

        public override void UpdateWidget(WidgetContext context, Rect bounds)
        {
            base.UpdateWidget(context, bounds);

            var sizes = GetLayoutSizes(bounds.size);
            float offset = 0;
            for (int i = 0; i < nodes.Count; i++)
            {
                var size = sizes[i];
                var node = nodes[i];
                 
                if (node.Content != null) 
                {
                    var nodeBounds = GetWidgetBounds(WidgetBounds.size, offset, size);
                    node.Content.UpdateWidget(context, nodeBounds);
                }

                offset += size;
            }
        }

        public override void Draw(WidgetContext context)
        {
            var children = GetChildWidgets();
            foreach (var childWidget in children)
            {
                WidgetUtils.DrawWidgetGroup(context, childWidget);
            }
        }

        Rect GetWidgetBounds(Vector2 hostSize, float offset, float size)
        {
            Rect bounds = new Rect(Vector2.zero, hostSize);
            if (Direction == SplitterDirection.Horizontal)
            {
                bounds.x += offset;
                bounds.width = size;
            }
            else
            {
                bounds.y += offset;
                bounds.height = size;
            }
            return bounds;
        }

        float[] GetLayoutSizes(Vector2 windowSize)
        {
            float totalSize = (Direction == SplitterDirection.Horizontal) ? windowSize.x : windowSize.y;
            float totalWeight = 0;
            foreach (var node in nodes)
            {
                totalWeight += node.Weight;
            }

            var sizes = new List<float>();
            foreach (var node in nodes)
            {
                var ratio = node.Weight / totalWeight;
                var size = totalSize * ratio;
                sizes.Add(size);
            }

            return sizes.ToArray();
        }

    }
}
