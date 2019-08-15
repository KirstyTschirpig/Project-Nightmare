using Generation.Nodes;
using UnityEngine;
using ParadoxNotion.Design;


[Name("Tile Object")]
[Category("Generation")]
[Description("Takes an object and tiles it")]
public class TileObjectNode : GenerationControlNode
{
    protected override void RegisterPorts()
    {
        var output = AddGenerationOutput("Out");
        var tileAmount = AddValueInput<Vector3>("Tile Count");
        var tileSize = AddValueInput<Vector3>("Tile Size");
        var input = AddValueInput<GameObjectCreationInfo>("Object");

        AddGenerationInput("In", (f) =>
        {
            var parent = new GameObjectCreationInfo("TileParent");
            Vector3 pos;
            for (int x = 0; x < tileAmount.value.x; x++)
            {
                for (int y = 0; y < tileAmount.value.y; y++)
                {
                    for (int z = 0; z < tileAmount.value.z; z++)
                    {
                        var creationInfo = new GameObjectCreationInfo(input.value);
                        pos.x = x * tileSize.value.x - tileSize.value.x - tileAmount.value.x / 2;
                        pos.y = y * tileSize.value.y - tileSize.value.y - tileAmount.value.y / 2;
                        pos.z = z * tileSize.value.z - tileSize.value.z - tileAmount.value.z / 2;
                        creationInfo.position = pos;
                        parent.AddChild(creationInfo);
                    }
                }
            }

            f.SetCreationInfo(parent);

            output.Call(f);
        });
    }
}