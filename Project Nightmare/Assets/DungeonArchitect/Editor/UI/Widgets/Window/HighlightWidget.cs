using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DMathUtils = DungeonArchitect.Utils.MathUtils;

namespace DungeonArchitect.Editors.UI.Widgets
{
    public class HighlightWidget : WidgetBase
    {
        public IWidget Widget;
        public object ObjectOfInterest;
        public Color HighlightColor = Color.red;
        public float HighlightThickness = 3.0f;
        public float HighlightTime = 1.0f;
        double lastUpdateTime = 0;

        float remainingTime = 0;
        Texture2D lineTexture;
        

        public HighlightWidget()
        { 
            lastUpdateTime = EditorApplication.timeSinceStartup;
            DungeonEditorResources resources = new DungeonEditorResources();
            lineTexture = resources.GetResource<Texture2D>(DungeonEditorResources.ICON_WHITE_16x);
        }

        public HighlightWidget SetContent(IWidget widget)
        {
            this.Widget = widget;
            return this;
        }

        public HighlightWidget SetHighlightColor(Color highlightColor)
        {
            this.HighlightColor = highlightColor;
            return this;
        }

        public HighlightWidget SetHighlightThickness(float highlightThickness)
        {
            this.HighlightThickness = highlightThickness;
            return this;
        }

        public HighlightWidget SetHighlightTime(float highlightTime)
        {
            this.HighlightTime = highlightTime;
            return this;
        }

        public HighlightWidget SetObjectOfInterest(object objectOfInterest)
        {
            this.ObjectOfInterest = objectOfInterest;
            return this;
        }

        public override void Draw(WidgetContext context)
        {
            if (Widget != null)
            {
                WidgetUtils.DrawWidgetGroup(context, Widget);
            }

            if (remainingTime > 0)
            {
                var bounds = new Rect(Vector2.zero, WidgetBounds.size);
                bounds = DMathUtils.ExpandRect(bounds, -HighlightThickness * 0.5f);
                float intensity = Mathf.Sin(Mathf.PI * remainingTime * 2);
                intensity = Mathf.Abs(intensity);
                var color = HighlightColor;
                color.a *= intensity;
                WidgetUtils.DrawWidgetFocusHighlight(bounds, color, HighlightThickness, lineTexture);
            }
        }

        public void Activate()
        {
            remainingTime = HighlightTime;
            lastUpdateTime = EditorApplication.timeSinceStartup;
        }

        public override void UpdateWidget(WidgetContext context, Rect bounds)
        {
            base.UpdateWidget(context, bounds);

            if (remainingTime > 0)
            {
                double currentTime = EditorApplication.timeSinceStartup;
                float deltaTime = (float)(currentTime - lastUpdateTime);

                remainingTime -= deltaTime;
                remainingTime = Mathf.Max(0, remainingTime);
                lastUpdateTime = currentTime;
            }

            if (Widget != null)
            {
                var contentBounds = new Rect(Vector2.zero, WidgetBounds.size);
                Widget.UpdateWidget(context, contentBounds);
            }

        }

        public override bool IsCompositeWidget()
        {
            return true;
        }

        public override IWidget[] GetChildWidgets()
        {
            return Widget != null ? new[] { Widget } : null;
        }

    }
}
