using System.Collections;
using System.Collections.Generic;
using Generation.Graphs;
using Generation.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

[Name("Transform Flow")]
[Category("Generation")]
public class TransformNode : GenerationControlNode
{
    protected override void RegisterPorts()
    {
        GenerationOutput output = AddGenerationOutput("Out");

        var relative = AddValueInput<bool>("Relative?");
        var position = AddValueInput<Vector3>("Position");
        var rotation = AddValueInput<Vector3>("Rotation");

        AddGenerationInput("In", (f) =>
        {
            if (f.GetCurrentGenerationResult() == null) output.Call(f);

            var go = f.GetCurrentGenerationResult();
            if (relative.value)
            {
                go.transform.position += position.value;
                go.transform.Rotate(rotation.value);
            }
            else
            {
                go.transform.position = position.value;
                go.transform.rotation = Quaternion.Euler(rotation.value);
            }

            output.Call(f);
        });
    }
}
