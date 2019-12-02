using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.UI.Widgets
{
    public delegate void WidgetDragEvent(Event e, WidgetContext context);

    public interface IWidget
    {
        void UpdateWidget(WidgetContext context, Rect bounds);
        void Draw(WidgetContext context);
        void HandleInput(Event e, WidgetContext context);

        bool IsCompositeWidget();
        bool CanAcquireFocus();
        IWidget[] GetChildWidgets();
        Vector2 GetDesiredSize(Vector2 size);
        Rect WidgetBounds { get; set; }
        bool ShowFocusHighlight { get; set; }
        Vector2 ScrollPosition { get; set; }

    }

    public struct WidgetContext
    {
        public WidgetInputManager inputManager;
    }

    public abstract class WidgetBase : IWidget
    {
        private bool showFocusHighlight = false;
        private Rect widgetBounds = Rect.zero;
        private Vector2 scrollPosition = Vector2.zero;

        public bool ShowFocusHighlight
        {
            get { return showFocusHighlight; }
            set { showFocusHighlight = value; }
        }

        public Rect WidgetBounds
        {
            get { return widgetBounds; }
            set { widgetBounds = value; }
        }

        public virtual Vector2 ScrollPosition
        {
            get { return scrollPosition; }
            set { scrollPosition = value; }
        }

        public bool DragDropEnabled = false;
        public virtual bool CanAcquireFocus() { return false; }
        public virtual Vector2 GetDesiredSize(Vector2 size) { return size; }

        public abstract void Draw(WidgetContext context);

        public virtual void UpdateWidget(WidgetContext context, Rect bounds)
        {
            WidgetBounds = bounds;
        }
        
        public virtual void HandleInput(Event e, WidgetContext context)
        {
            switch (e.type)
            {
                case EventType.MouseDrag:
                    if (DragDropEnabled)
                    {
                        HandleDragStart(e, context);
                    }
                    break;

                case EventType.DragUpdated:
                    if (DragDropEnabled && IsDragDataSupported(e, context))
                    {
                        HandleDragUpdate(e, context);
                    }
                    break;

                case EventType.DragPerform:
                    if (DragDropEnabled && IsDragDataSupported(e, context))
                    {
                        HandleDragPerform(e, context);
                    }
                    break;
            }
        }

        public virtual bool IsCompositeWidget() { return false; }
        public virtual IWidget[] GetChildWidgets() { return null; }

        protected virtual bool IsDragDataSupported(Event e, WidgetContext context) { return false; }

        public event WidgetDragEvent DragStart;
        public event WidgetDragEvent DragUpdate;
        public event WidgetDragEvent DragPerform;

        protected virtual void HandleDragStart(Event e, WidgetContext context)
        {
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.StartDrag("Widget Drag");

            // Make sure no one uses the event after us
            Event.current.Use();

            if (DragStart != null)
            {
                DragStart.Invoke(e, context);
            }
        }

        void HandleDragUpdate(Event e, WidgetContext context)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (DragUpdate != null)
            {
                DragUpdate.Invoke(e, context);
            }
        }

        void HandleDragPerform(Event e, WidgetContext context)
        {
            DragAndDrop.AcceptDrag();

            if (DragPerform != null)
            {
                DragPerform.Invoke(e, context);
            }
        }


    }

    public class NullWidget : WidgetBase
    {
        public override void Draw(WidgetContext context) { }
    }
}

