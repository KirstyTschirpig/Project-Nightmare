using System.Collections;
using System.Collections.Generic;
using Generation.Graphs;
using Generation.Nodes;
using UnityEngine;
using ParadoxNotion.Design;

[Category("Generation")]
[Name("Start")]
public class GenerationStart : GenerationControlNode
{
    private GenerationOutput output;
    private ValueInput<bool> shouldCreateDefaultObject;
    protected override void RegisterPorts()
    {
        output = AddGenerationOutput("Generator");
        shouldCreateDefaultObject = AddValueInput<bool>("Create Default Object");
    }

    public void Start()
    {
        var flow = new GenerationFlow();

        if (shouldCreateDefaultObject.value)
        {
            flow.SetCurrentGenerationResult(new GameObject("GenerationResult"));
        }

        output.Call(flow);
    }
}
