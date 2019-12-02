using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DMathUtils = DungeonArchitect.Utils.MathUtils;

namespace DungeonArchitect.Editors.UI.Widgets
{
    public class WidgetUtils
    {
        public static void GetWidgets(IWidget widget, ref List<IWidget> result)
        {
            if (widget == null)
            {
                return;
            }
            result.Add(widget);

            var children = widget.GetChildWidgets();
            if (children != null)
            {
                foreach (var child in children)
                {
                    GetWidgets(child, ref result);
                }
            }
        }

        public static List<T> GetWidgetsOfType<T>(IWidget root) where T : IWidget
        {
            var widgets = new List<IWidget>();
            GetWidgets(root, ref widgets);

            var result = new List<T>();
            foreach (var widget in widgets)
            {
                if (widget != null && widget is T)
                {
                    result.Add((T)widget);
                }
            }
            return result;
        }

        public static bool BuildWidgetEvent(Vector2 mousePosition, IWidget root, IWidget widgetToFind, ref Vector2 widgetMousePosition)
        {
            mousePosition += root.ScrollPosition;
            if (root == widgetToFind)
            {
                widgetMousePosition = mousePosition;
                return true;
            }

            var children = root.GetChildWidgets();
            if (children != null)
            {
                foreach (var child in children)
                {
                    if (child == null) continue;

                    var childMousePosition = mousePosition - child.WidgetBounds.position;
                    if (BuildWidgetEvent(childMousePosition, child, widgetToFind, ref widgetMousePosition))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool FindWidgetOnCursor(Event e, IWidget widget, out Event outEvent, out IWidget outWidget)
        {
            if (widget != null)
            {
                var mousePosition = e.mousePosition;
                var bounds = new Rect(Vector2.zero, widget.WidgetBounds.size);
                if (bounds.Contains(mousePosition))
                {
                    mousePosition += widget.ScrollPosition;
                    // Check if the children can be found first
                    var children = widget.GetChildWidgets();
                    if (children != null)
                    {
                        foreach (var child in children)
                        {
                            var childEvent = new Event(e);
                            childEvent.mousePosition = mousePosition - child.WidgetBounds.position;
                            if (FindWidgetOnCursor(childEvent, child, out outEvent, out outWidget))
                            {
                                return true;
                            }
                        }
                    }

                    outEvent = new Event(e);
                    outWidget = widget;
                    return true;
                }
            }

            outEvent = null;
            outWidget = null;
            return false;
        }

        public static void ProcessDragOperation(Event e, IWidget widget, WidgetContext context)
        {
            if (e.type == EventType.DragUpdated || e.type == EventType.DragPerform)
            {
                Event widgetEvent;
                IWidget widgetOnCursor;
                if (FindWidgetOnCursor(e, widget, out widgetEvent, out widgetOnCursor))
                {
                    if (widgetOnCursor != null)
                    {
                        if (e.type == EventType.DragUpdated)
                        {
                            widgetOnCursor.HandleInput(e, context);
                        }
                        else if (e.type == EventType.DragPerform)
                        {
                            // Request the focus so our focus input handling code will handle this
                            if (context.inputManager != null)
                            {
                                context.inputManager.RequestFocus(widgetOnCursor);
                            }
                        }
                    }
                }
            }
        }

        public static bool ProcessInputFocus(Vector2 mousePosition, WidgetInputManager inputManager, IWidget widget)
        {
            if (inputManager == null || widget == null)
            {
                return false;
            }

            var bounds = new Rect(Vector2.zero, widget.WidgetBounds.size);
            if (!bounds.Contains(mousePosition))
            {
                return false;
            }

            mousePosition += widget.ScrollPosition;


            if (widget.CanAcquireFocus())
            {
                inputManager.RequestFocus(widget);
                return true;
            }

            var children = widget.GetChildWidgets();
            if (children != null)
            {
                foreach (var child in children)
                {
                    if (child == null) continue;
                    var childMousePosition = mousePosition - child.WidgetBounds.position;
                    if (ProcessInputFocus(childMousePosition, inputManager, child))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static void DrawWidgetFocusHighlight(Rect bounds, Color color)
        {
            DrawWidgetFocusHighlight(bounds, color, 1);
        }


        public static void DrawWidgetFocusHighlight(Rect bounds, Color color, float thickness)
        {
            DrawWidgetFocusHighlight(bounds, color, thickness, null);
        }

        public static void DrawWidgetFocusHighlight(Rect bounds, Color color, float thickness, Texture2D texture)
        {
            const float padding = 1;
            float x0 = (bounds.xMin + padding);
            float y0 = (bounds.yMin + padding);
            float x1 = (bounds.xMax - padding);
            float y1 = (bounds.yMax - padding);

            Handles.BeginGUI();
            Handles.color = color;
            Vector2 P00 = new Vector2(x0, y0);
            Vector2 P10 = new Vector2(x1, y0);
            Vector2 P11 = new Vector2(x1, y1);
            Vector2 P01 = new Vector2(x0, y1);
            if (thickness == 1)
            {
                Handles.DrawPolyLine(P00, P10, P11, P01, P00);
            }
            else
            {
                if (texture != null)
                {
                    Handles.DrawAAPolyLine(texture, thickness, P00, P10, P11, P01, P00);
                }
                else
                {
                    Handles.DrawAAPolyLine(thickness, P00, P10, P11, P01, P00);
                }
            }
            Handles.EndGUI();
        }

        private readonly static Color FOCUS_HIGHLITE_COLOR = new Color(1, 0.5f, 0, 1);
        public static void DrawWidgetGroup(WidgetContext context, IWidget widget)
        {
            var widgetContext = context;
            GUI.BeginGroup(widget.WidgetBounds);
            widget.Draw(widgetContext);

            bool isFocused = (context.inputManager != null) ? context.inputManager.FocusedWidget == widget : false;
            if (isFocused && widget.ShowFocusHighlight)
            {
                var hilightBounds = new Rect(new Vector2(0.5f, 0.5f), widget.WidgetBounds.size - Vector2.one);
                DrawWidgetFocusHighlight(hilightBounds, FOCUS_HIGHLITE_COLOR);
            }
            GUI.EndGroup();
        }

        public static bool IsDragEvent(Event e)
        {
            return e.type == EventType.DragPerform
                || e.type == EventType.DragUpdated
                || e.type == EventType.DragExited
                || e.type == EventType.MouseDrag;
        }
    }


}
