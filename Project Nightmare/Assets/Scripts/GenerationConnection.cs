using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using FlowCanvas;
using NodeCanvas.Framework;
using ParadoxNotion;

public class GenerationConnection : Connection
{
  public override PlanarDirection direction
  {
    get { return PlanarDirection.Vertical; }
  }
}

