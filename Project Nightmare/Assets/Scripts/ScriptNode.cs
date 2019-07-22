using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using UnityEngine;

public class ScriptNode : GenerationNode
{

    [SerializeField] [ExposeField] private GenerationScript script;
    public override int maxOutConnections
    {
        get { return 0; }
    }

    public override string name
    {
        get { return "Script"; }
    }

    public override bool allowAsPrime
    {
        get { return false; }
    }
}
