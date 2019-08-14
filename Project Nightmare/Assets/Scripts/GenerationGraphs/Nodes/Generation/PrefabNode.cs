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
        private GameObject createdObject;

        protected override void RegisterPorts()
        {
            var prefab = AddValueInput<GameObject>("Prefab");
            var scale = AddValueInput<Vector3>("Scale");
            var position = AddValueInput<Vector3>("Position");
            var output = AddGenerationOutput("Out");
            var prefabOutput = AddValueOutput<GameObject>("Object", () => { return createdObject; });

            AddGenerationInput("In", (f) =>
            {
                var parent = f.GetCurrentGenerationResult();
                createdObject = Object.Instantiate(prefab.value);
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