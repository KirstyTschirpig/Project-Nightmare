using System.Collections;
using System.Collections.Generic;
using Generation.Graphs;
using Generation.Nodes;
using UnityEngine;
using ParadoxNotion.Design;


[Name("Transform Object")]
[Category("Generation")]
public class TransformObjectNode : GenerationControlNode
{
    protected override void RegisterPorts()
    {
        GenerationOutput output = AddGenerationOutput("Out");

        ValueInput<bool> relative = AddValueInput<bool>("Relative?");
        var position = AddValueInput<Vector3>("Position");
        var rotation = AddValueInput<Vector3>("Rotation");
        var obj = AddValueInput<GameObject>("Object");

        AddGenerationInput("In", (f) =>
        {
            if (obj.value == null) output.Call(f);

            if (relative.value)
            {
                obj.value.transform.position += position.value;
                obj.value.transform.Rotate(rotation.value);
            }
            else
            {
                obj.value.transform.position = position.value;
                obj.value.transform.rotation = Quaternion.Euler(rotation.value);
            }

            output.Call(f);
        });
    }
}
