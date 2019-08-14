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
        private GameObject createdObject;

        protected override void RegisterPorts()
        {
            var prefab = AddValueInput<List<GameObject>>("Prefabs");
            var scale = AddValueInput<Vector3>("Scale");
            var position = AddValueInput<Vector3>("Position");
            var output = AddGenerationOutput("Out");
            var prefabOutput = AddValueOutput<GameObject>("Object", () => { return createdObject; });

            AddGenerationInput("In", (f) =>
            {
                var parent = f.GetCurrentGenerationResult();
                createdObject = Object.Instantiate(prefab.value[Random.Range(0, prefab.value.Count)]);
                if(parent != null) createdObject.transform.SetParent(parent.transform);
                else f.SetCurrentGenerationResult(createdObject);
                Debug.Log("Created " + prefab.value);
                createdObject.transform.localPosition = position.value;
                createdObject.transform.localScale = scale.value;
                output.Call(f);
            });
        }
    }
}