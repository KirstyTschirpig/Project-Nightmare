using DungeonArchitect.Grammar;
using DungeonArchitect.Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Editors.DungeonFlow
{
    public class GrammarExecEntryNodeRenderer : GrammarNodeRendererBase
    {
        protected override string GetCaption(GraphNode node)
        {
            return "Entry";
        }

        protected override Color GetPinColor(GraphNode node)
        {
            return new Color(0.1f, 0.4f, 0.4f);
        }

        protected override Color GetBodyColor(GraphNode node)
        {
            return new Color(0.1f, 0.1f, 0.1f, 1);
        }
    }
}
