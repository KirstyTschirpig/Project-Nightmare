using System;
using System.Collections;
using System.Collections.Generic;
using FlowCanvas;
using NodeCanvas.Framework;
using ParadoxNotion;
using UnityEngine;

public class GenerationNode : Node, ISerializationCallbackReceiver
{
    [NonSerialized] private Dictionary<string, Port> inPorts;
    [NonSerialized] private Dictionary<string, Port> outPorts;
    
    public override int maxInConnections
    {
        get { return -1; }
    }

    public override int maxOutConnections
    {
        get { return -1; }
    }

    public override Type outConnectionType
    {
        get { return typeof(GenerationConnection); }
    }

    public override bool allowAsPrime
    {
        get { return true; }
    }

    public override Alignment2x2 commentsAlignment
    {
        get { return Alignment2x2.Bottom; }
    }

    public override Alignment2x2 iconAlignment
    {
        get { return Alignment2x2.Bottom; }
    }

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        throw new NotImplementedException();
    }

    public virtual void OnPortConnected(Port port, Port otherPort)
    {
        
    }

    public virtual void OnPortDisconnected(Port port, Port otherPort)
    {
        
    }
    
    
}
