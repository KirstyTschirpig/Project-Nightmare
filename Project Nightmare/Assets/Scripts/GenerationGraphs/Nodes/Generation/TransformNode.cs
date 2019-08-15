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
            if (f.GetCreationInfo() == null) output.Call(f);

            var creationInfo = f.GetCreationInfo();
            if (relative.value)
            {
                creationInfo.position += position.value;
                creationInfo.rotation *= Quaternion.Euler(rotation.value);
            }
            else
            {
                creationInfo.position = position.value;
                creationInfo.rotation = Quaternion.Euler(rotation.value);
            }

            output.Call(f);
        });
    }
}
