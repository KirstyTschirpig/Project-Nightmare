using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using UnityEngine;

[Serializable]
public abstract class GenerationGraph : Graph
{

    public override Type baseNodeType
    {
        get { return typeof(GenerationNode); }
    }

    public override bool requiresAgent
    {
        get { return false; }
    }

    public override bool requiresPrimeNode
    {
        get { return true; }
    }

    public override bool isTree
    {
        get { return true; }
    }

    public override bool useLocalBlackboard
    {
        get { return false; }
    }

    public override bool canAcceptVariableDrops
    {
        get { return true; }
    }
}
