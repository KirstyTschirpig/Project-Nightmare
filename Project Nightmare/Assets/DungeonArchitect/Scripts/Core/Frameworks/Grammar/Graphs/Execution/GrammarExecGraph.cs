using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonArchitect.Graphs;

namespace DungeonArchitect.Grammar
{
    public class GrammarExecGraph : Graph
    {
        [SerializeField]
        public GrammarExecEntryNode entryNode;
        public override void OnEnable()
        {
            base.OnEnable();

            hideFlags = HideFlags.HideInHierarchy;
        }
    }

}
