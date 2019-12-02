using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Editors.UI.Widgets
{
    public class LabelWidget : WidgetBase
    {
        GUIStyle style;
        GUIContent content;

        public LabelWidget(string caption) 
            : this(new GUIContent(caption), new GUIStyle(GUI.skin.label))
        {
        }

        public LabelWidget(GUIContent content)
            : this(content, new GUIStyle(GUI.skin.label))
        {
        }

        public LabelWidget(GUIContent content, GUIStyle style)
        {
            this.content = content;
            this.style = style;
        }

        public override void Draw(WidgetContext context)
        {
            var bounds = new Rect(Vector2.zero, WidgetBounds.size);
            GUI.Label(bounds, content, style);
        }
    }
}
