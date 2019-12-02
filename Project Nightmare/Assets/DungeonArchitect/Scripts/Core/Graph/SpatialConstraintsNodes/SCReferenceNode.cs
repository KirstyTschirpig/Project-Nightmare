using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Graphs.SpatialConstraints
{
    public class SCReferenceNode : SCRuleNode
    {
        public override void Initialize(string id, Graph graph)
        {
            base.Initialize(id, graph);
            
            canBeDeleted = false;
        }

        public override Color GetColor()
        {
            return new Color(0.3f, 0, 1);
        }

    }
}