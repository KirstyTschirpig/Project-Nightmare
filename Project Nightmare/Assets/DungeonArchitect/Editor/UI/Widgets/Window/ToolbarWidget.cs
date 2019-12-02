using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.UI.Widgets
{
    public class ToolbarWidget : WidgetBase
    {
        public float ButtonSize = 20;
        public float Padding = 0;
        public Color Background = new Color(0, 0, 0, 0.25f);
        public delegate void OnButtonPressed(WidgetContext context, string id);
        public event OnButtonPressed ButtonPressed;
        public List<ButtonInfo> buttons = new List<ButtonInfo>();

        public class ButtonInfo
        {
            public string ButtonId;
            public string IconId;
            public Rect Bounds;
        }
        DungeonEditorResources resources = new DungeonEditorResources();
        GUIStyle buttonStyle;

        public override void UpdateWidget(WidgetContext context, Rect bounds)
        {
            base.UpdateWidget(context, bounds);

            var size = new Vector2(
                Padding * 2 + buttons.Count * ButtonSize,
                Padding * 2 + ButtonSize);
            WidgetBounds = new Rect(WidgetBounds.position, size);
            UpdateButtonBounds();
        }

        public override void Draw(WidgetContext context)
        {
            if (buttonStyle == null)
            {
                var skin = resources.GetResource<GUISkin>(DungeonEditorResources.SKIN_TOOLBAR_BUTTONS);
                buttonStyle = skin.button;
            }
            if (buttonStyle == null)
            {
                buttonStyle = EditorStyles.toolbarButton;
            }

            var toolbarBounds = WidgetBounds;
            EditorGUI.DrawRect(toolbarBounds, Background);

            var style = EditorStyles.toolbarButton;
            foreach (var button in buttons)
            {
                var icon = resources.GetResource<Texture>(button.IconId);
                if (GUI.Button(button.Bounds, new GUIContent(icon), buttonStyle))
                {
                    if (ButtonPressed != null)
                    {
                        ButtonPressed.Invoke(context, button.ButtonId);
                    }
                }
            }
        }

        public void AddButton(string buttonId, string iconId)
        {
            var button = new ButtonInfo();
            button.ButtonId = buttonId;
            button.IconId = iconId;
            buttons.Add(button);
        }

        void UpdateButtonBounds()
        {
            float x = Padding;
            float y = Padding;
            foreach (var button in buttons)
            {
                button.Bounds = new Rect(x, y, ButtonSize, ButtonSize);
                x += ButtonSize;
            }
        }

    }
}
