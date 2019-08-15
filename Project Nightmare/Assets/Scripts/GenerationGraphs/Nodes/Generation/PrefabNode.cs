using System.Collections;
using System.Collections.Generic;
using Generation.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Generation.Nodes
{
    [Category("Generation")]
    [Name("Prefab")]
    [Description("Spawns a prefab")]
    public class PrefabNode : GenerationControlNode
    {
        private GameObjectCreationInfo creationInfo;

        protected override void RegisterPorts()
        {
            var prefab = AddValueInput<GameObject>("Prefab");
            var scale = AddValueInput<Vector3>("Scale");
            var position = AddValueInput<Vector3>("Position");
            var output = AddGenerationOutput("Out");
            var prefabOutput = AddValueOutput<GameObjectCreationInfo>("Object", () => { return creationInfo; });

            AddGenerationInput("In", (f) =>
            {
                var parent = f.GetCreationInfo();
                creationInfo = new GameObjectCreationInfo(prefab.value.name);
                creationInfo.SetTemplate(prefab.value);
                if(parent != null) parent.AddChild(creationInfo);
                else f.SetCreationInfo(creationInfo);
                Debug.Log("Created " + prefab.value.name);
                creationInfo.position = position.value;
                creationInfo.scale = scale.value;
                output.Call(f);
            });
        }
    }
}