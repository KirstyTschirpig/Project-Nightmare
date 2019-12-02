using DungeonArchitect.Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DungeonArchitect.Grammar
{
    public class GrammarExecEntryNode : GrammarExecNodeBase
    {
        public override void Initialize(string id, Graph graph)
        {
            base.Initialize(id, graph);
            canBeDeleted = false;
            caption = "Entry";
        }
    }
}
