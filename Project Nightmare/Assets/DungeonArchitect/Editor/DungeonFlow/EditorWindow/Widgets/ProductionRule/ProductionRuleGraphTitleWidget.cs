using DungeonArchitect.Editors.UI.Widgets;
using DungeonArchitect.Grammar;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.DungeonFlow
{
    public class ProductionRuleRHSTitleWidget : WidgetBase
    {
        public delegate void OnDeletePressed(ProductionRuleWidgetRHSState state);
        public event OnDeletePressed DeletePressed;

        public ProductionRuleWidgetRHSState State;

        public override void Draw(WidgetContext context)
        {
            Vector2 titleOffset = new Vector2(5, 4);

            var titleStyle = new GUIStyle(EditorStyles.whiteLabel);
            titleStyle.normal.textColor = new Color(0.75f, 0.75f, 0.75f);

            var titleBounds = WidgetBounds;
            titleBounds.position += titleOffset;
            titleBounds.size -= titleOffset * 2;

            GUI.BeginGroup(titleBounds);

            float PX_BUTTON_SIZE = 18;
            float PX_BUTTON_PADDING = 10;
            float PX_PADDING = 5;
            float PX_WEIGHT_INPUT = 40;
            float PX_HEIGHT = 16;

            float x = titleBounds.width;
            x -= PX_BUTTON_SIZE;

            var guiState = new GUIState();
            GUI.backgroundColor = new Color(0.8f, 0.1f, 0.1f, 1.0f);
            bool deletePressed = false;
            if (GUI.Button(new Rect(x, 0, PX_BUTTON_SIZE, PX_BUTTON_SIZE), "X"))
            {
                deletePressed = true;
            }
            guiState.Restore();

            var WeightGraph = State.WeightedGraph;

            x -= PX_BUTTON_PADDING;
            x -= PX_WEIGHT_INPUT;

            string Weight = (WeightGraph != null) ? WeightGraph.weight.ToString() : "";
            guiState.Save();
            GUI.backgroundColor = new Color(0.6f, 0.6f, 0.6f);
            if (GUI.Button(new Rect(x, 1, PX_WEIGHT_INPUT, PX_HEIGHT), Weight, EditorStyles.miniButton))
            {
                Selection.activeObject = WeightGraph;
            }
            guiState.Restore();

            x -= PX_PADDING;

            var weightCaption = new GUIContent("Weight:");
            float PX_WEIGHT_LABEL = titleStyle.CalcSize(weightCaption).x;

            x -= PX_WEIGHT_LABEL;
            GUI.Label(new Rect(x, 1, PX_WEIGHT_LABEL, PX_HEIGHT), weightCaption, titleStyle);

            float remainingWidth = x;
            if (x > 0)
            {
                GUI.Label(new Rect(0, 0, remainingWidth, PX_HEIGHT), "RHS Graph", titleStyle);
            }


            GUI.EndGroup();

            if (deletePressed)
            {
                if (DeletePressed != null)
                {
                    DeletePressed.Invoke(State);
                }
            }
        }
    }

    public class ProductionRuleLHSTitleWidget : WidgetBase
    {
        public override void Draw(WidgetContext context)
        {
            Vector2 titleOffset = new Vector2(5, 5);

            var titleStyle = new GUIStyle(EditorStyles.whiteLabel);
            titleStyle.normal.textColor = new Color(0.75f, 0.75f, 0.75f);

            var titleBounds = WidgetBounds;
            titleBounds.position += titleOffset;

            GUI.BeginGroup(titleBounds);
            EditorGUILayout.LabelField("LHS Graph", titleStyle);
            GUI.EndGroup();
        }

    }

}
