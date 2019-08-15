using System.Collections;
using System.Collections.Generic;
using Generation.Nodes;
using UnityEngine;
using ParadoxNotion.Design;

[Name("Generate")]
[Category("Generation")]
[Description("Causes the input flow to be generated in the level")]
public class GenerateNode : GenerationControlNode
{
    protected override void RegisterPorts()
    {
        AddGenerationInput("In", (f) =>
        {
            GenerationController.GenerateObject(f.GetCreationInfo());
        });
    }
}
