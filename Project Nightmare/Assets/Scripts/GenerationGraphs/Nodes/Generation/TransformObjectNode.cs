using System.Collections;
using System.Collections.Generic;
using Generation.Graphs;
using Generation.Nodes;
using UnityEngine;
using ParadoxNotion.Design;


[Name("Transform Object")]
[Description("Transforms an object.")]
[Category("Generation")]
public class TransformObjectNode : PureFunctionNode<GameObjectCreationInfo, GameObjectCreationInfo, bool, Vector3, Vector3>
{
    public override GameObjectCreationInfo Invoke(GameObjectCreationInfo obj, bool relative, Vector3 position, Vector3 rotation)
    {
        if (obj == null) return null;

        var creationInfo = new GameObjectCreationInfo(obj);

        if (relative)
        {
            creationInfo.position += position;
            creationInfo.rotation *= Quaternion.Euler(rotation);
        }
        else
        {
            creationInfo.position = position;
            creationInfo.rotation = Quaternion.Euler(rotation);
        }

        return creationInfo;
    }
}