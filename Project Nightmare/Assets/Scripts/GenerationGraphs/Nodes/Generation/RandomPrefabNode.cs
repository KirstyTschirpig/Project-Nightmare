using System.Collections;
using System.Collections.Generic;
using Generation.Nodes;
using ParadoxNotion.Design;
using UnityEngine;
using System.Linq;

namespace Generation.Nodes
{
    [Category("Generation")]
    [Name("Random Prefab")]
    [Description("Spawns a random prefab")]
    public class RandomPrefabNode : GenerationControlNode
    {
        private GameObjectCreationInfo creationInfo;

        protected override void RegisterPorts()
        {
            var prefabs = AddValueInput<List<GameObject>>("Prefabs");
            var scale = AddValueInput<Vector3>("Scale");
            var position = AddValueInput<Vector3>("Position");
            var output = AddGenerationOutput("Out");
            var prefabOutput = AddValueOutput<GameObjectCreationInfo>("Object", () => { return creationInfo; });

            AddGenerationInput("In", (f) =>
            {
                var parent = f.GetCreationInfo();
                var chosenPrefab = prefabs.value[Random.Range(0, prefabs.value.Count)];
                creationInfo = new GameObjectCreationInfo(chosenPrefab.name);
                if(parent != null) parent.AddChild(creationInfo);
                else f.SetCreationInfo(creationInfo);
                Debug.Log("Created " + prefabs.value);
                creationInfo.position = position.value;
                creationInfo.scale = scale.value;
                output.Call(f);
            });
        }
    }
}