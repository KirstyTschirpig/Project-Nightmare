using DungeonArchitect.Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DungeonArchitect.Grammar
{
    public class GrammarExecNodeBase : GraphNode
    {
        public override void Initialize(string id, Graph graph)
        {
            base.Initialize(id, graph);
            Size = new Vector2(120, 120);

            // Create an input pin on the top
            CreatePinOfType<GrammarNodePin>(GraphPinType.Input,
                        Vector2.zero,
                        Rect.zero,
                        new Vector2(0, -1));

            // Create an output pin at the bottom
            CreatePinOfType<GrammarNodePin>(GraphPinType.Output,
                        Vector2.zero,
                        Rect.zero,
                        new Vector2(0, -1));

        }
    }
}
