using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Editors.UI.Widgets
{
    public class ScrollPanelWidget : WidgetBase
    {
        IWidget content;
        bool MouseScrollingEnabled = true;

        public ScrollPanelWidget(IWidget content)
            : this(content, true)
        {
        }

        public ScrollPanelWidget(IWidget content, bool mouseScrollingEnabled)
        {
            this.content = content;
            this.MouseScrollingEnabled = mouseScrollingEnabled;
        }

        public override void Draw(WidgetContext context)
        {
            var bounds = new Rect(Vector2.zero, WidgetBounds.size);
            ScrollPosition = GUI.BeginScrollView(bounds, ScrollPosition, content.WidgetBounds);
            content.Draw(context);
            GUI.EndScrollView(MouseScrollingEnabled);
        }

        public override void UpdateWidget(WidgetContext context, Rect bounds)
        {
            base.UpdateWidget(context, bounds);

            var contentSize = content.GetDesiredSize(bounds.size);
            if (contentSize.y > bounds.height)
            {
                contentSize.x -= 16;
            }
            var contentBounds = new Rect(Vector2.zero, contentSize);
            content.UpdateWidget(context, contentBounds);
        }

        public override bool IsCompositeWidget()
        {
            return true;
        }

        public override IWidget[] GetChildWidgets()
        {
            return new[] { content };
        }
    }
}
