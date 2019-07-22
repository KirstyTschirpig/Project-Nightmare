using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositionNode : GenerationNode
{
    public override int maxOutConnections
    {
        get { return 4; }
    }

    public override string name
    {
        get { return "Composition"; }
    }
}
