using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.UI.Widgets
{

    public delegate void OnWidgetFocus(IWidget widget);
    public delegate void OnWidgetLostFocus(IWidget widget);

    public class WidgetInputManager
    {
        private IWidget focusedWidget = null;
        public IWidget FocusedWidget { get { return focusedWidget; } }

        public bool IsDragDrop = false;

        public void RequestFocus(IWidget widget)
        {
            GUI.FocusControl("");
            // Notify that the old widget has lost focus
            if (focusedWidget != null)
            {
                if (WidgetLostFocus != null)
                {
                    WidgetLostFocus.Invoke(focusedWidget);
                }
            }

            focusedWidget = widget;
            if (focusedWidget != null)
            {
                if (WidgetFocused != null)
                {
                    WidgetFocused.Invoke(focusedWidget);
                }
            }
        }

        public event OnWidgetFocus WidgetFocused;
        public event OnWidgetLostFocus WidgetLostFocus;
    }
}
