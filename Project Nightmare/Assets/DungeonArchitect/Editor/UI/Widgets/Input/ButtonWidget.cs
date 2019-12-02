using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Editors.UI.Widgets
{
    public class ButtonWidget : WidgetBase
    {
        GUIContent content;
        Color color = new Color(0.8f, 0.8f, 0.8f);

        public delegate void OnButtonPressed();
        public event OnButtonPressed ButtonPressed;

        public ButtonWidget(GUIContent content)
        {
            this.content = content;
        }

        public ButtonWidget SetColor(Color color)
        {
            this.color = color;
            return this;
        }

        public override void Draw(WidgetContext context)
        {
            var state = new GUIState();
            var bounds = new Rect(Vector2.zero, WidgetBounds.size);
            GUI.color = color;
            if (GUI.Button(bounds, content))
            {
                if (ButtonPressed != null)
                {
                    ButtonPressed.Invoke();
                }
            }
            state.Restore();
        }
    }
}
