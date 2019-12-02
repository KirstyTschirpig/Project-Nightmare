using DungeonArchitect.Editors.UI.Widgets;
using DungeonArchitect.Grammar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Editors.DungeonFlow
{
    public enum DungeonFlowErrorType
    {
        Error,
        Warning,
        Info,
        Success
    }


    public class DungeonFlowErrorEntry
    {
        public string Message = "";
        public DungeonFlowErrorType ErrorType = DungeonFlowErrorType.Info;
        public IDungeonFlowValidatorAction Action;

        public DungeonFlowErrorEntry() { }
        public DungeonFlowErrorEntry(string message)
        {
            this.Message = message;
        }

        public DungeonFlowErrorEntry(string message, DungeonFlowErrorType errorType)
        {
            this.Message = message;
            this.ErrorType = errorType;
        }

        public DungeonFlowErrorEntry(string message, DungeonFlowErrorType errorType, IDungeonFlowValidatorAction action)
        {
            this.Message = message;
            this.ErrorType = errorType;
            this.Action = action;
        }
    }

    public class DungeonFlowErrorList
    {
        public List<DungeonFlowErrorEntry> Errors = new List<DungeonFlowErrorEntry>();
    }

    public class ErrorListViewSource : ListViewSource<DungeonFlowErrorEntry>
    {
        DungeonEditorResources resources = new DungeonEditorResources();
        DungeonFlowErrorList errorList;
        public ErrorListViewSource(DungeonFlowErrorList errorList)
        {
            this.errorList = errorList;
        }

        public override DungeonFlowErrorEntry[] GetItems()
        {
            return (errorList != null && errorList.Errors != null) 
                ? errorList.Errors.ToArray() : null;
        }

        public override IWidget CreateWidget(DungeonFlowErrorEntry item)
        {
            var itemWidget = new ErrorListViewItem(item, resources);

            return itemWidget;
        }
    }

    public class ErrorListViewItem : ListViewTextItemWidget
    {
        HighlightWidget highlight;
        DungeonEditorResources resources;

        public ErrorListViewItem(DungeonFlowErrorEntry entry, DungeonEditorResources resources)
            : base(entry, () => entry.Message)
        {
            this.resources = resources;
            OffsetX = 2;

            int fontSize = 14;

            TextStyle.fontSize = fontSize;
            SelectedTextStyle.normal.textColor = Color.blue;

            SelectedTextStyle.fontSize = fontSize;
            SelectedTextStyle.normal.textColor = Color.black;

            SelectedColor = ErrorListPanel.ThemeColor * 2.0f;
        }

        Texture GetTexture()
        {
            var entry = ItemData as DungeonFlowErrorEntry;
            if (entry != null)
            {
                switch(entry.ErrorType)
                {
                    case DungeonFlowErrorType.Error:
                        return resources.GetResource<Texture>(DungeonEditorResources.ICON_ERROR_16x);

                    case DungeonFlowErrorType.Warning:
                        return resources.GetResource<Texture>(DungeonEditorResources.ICON_WARNING_16x);

                    case DungeonFlowErrorType.Info:
                        return resources.GetResource<Texture>(DungeonEditorResources.ICON_INFO_16x);

                    case DungeonFlowErrorType.Success:
                        return resources.GetResource<Texture>(DungeonEditorResources.ICON_SUCCESS_16x);
                }
            }
            return resources.GetResource<Texture>(DungeonEditorResources.ICON_INFO_16x);
        }

        public override void DrawText(Rect bounds)
        {
            var iconOffset = new Vector2(1, 1) * (bounds.height - 16) * 0.5f;
            var iconBounds = new Rect(iconOffset, new Vector2(16, 16));
            GUI.DrawTexture(iconBounds, GetTexture());

            var style = Selected ? SelectedTextStyle : TextStyle;
            float x = OffsetX + bounds.height;
            float y = (bounds.height - style.lineHeight) / 2.0f - 1;
            string message = GetCaption();
            var content = new GUIContent(message, message);
            var textSize = style.CalcSize(content);
            var textBounds = new Rect(new Vector2(x, y), textSize);
            GUI.Label(textBounds, content, style);
        }
    }

    public class ErrorListPanel : WidgetBase
    {
        IWidget host;
        public DungeonFlowErrorList errorList { get; private set; }

        public static readonly Color ThemeColor = new Color(0.3f, 0.2f, 0.2f);
        public ListViewWidget<DungeonFlowErrorEntry> ListView;

        public ErrorListPanel(DungeonFlowErrorList errorList)
        {
            this.errorList = errorList;

            ListView = new ListViewWidget<DungeonFlowErrorEntry>();
            ListView.Bind(new ErrorListViewSource(errorList));
            ListView.ItemHeight = 20;

            host = new BorderWidget()
                   .SetTitle("Error List")
                   .SetColor(ThemeColor)
                   .SetContent(ListView)
                    ;

        }

        public override void Draw(WidgetContext context)
        {
            host.Draw(context);
        }

        public override void UpdateWidget(WidgetContext context, Rect bounds)
        {
            base.UpdateWidget(context, bounds);

            if (host != null)
            {
                var childBounds = new Rect(Vector2.zero, bounds.size);
                host.UpdateWidget(context, childBounds);
            }
        }

        public override void HandleInput(Event e, WidgetContext context)
        {
            host.HandleInput(e, context);
        }

        public override bool IsCompositeWidget()
        {
            return true;
        }

        public override IWidget[] GetChildWidgets()
        {
            return new[] { host };
        }
    }
}