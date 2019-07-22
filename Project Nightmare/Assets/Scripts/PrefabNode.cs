using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using UnityEngine;

public class PrefabNode : GenerationNode
{
    [ExposeField] private GameObject prefab;
    public override string name
    {
        get { return "Prefab"; }
    }

    public override int maxOutConnections
    {
        get { return 0; }
    }
}
