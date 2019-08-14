using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;

namespace Generation.Nodes
{

    [Description("Utility node to merge the flow. It's exactly the same as connecting multiple Flow outputs to the same Flow input.")]
    [Name("Merge", 89)]
    public class Merge : GenerationControlNode
    {

        [SerializeField]
        [ExposeField]
        [GatherPortsCallback]
        [MinValue(2)]
        [DelayedField]
        private int _portCount = 4;

        protected override void RegisterPorts() {
            var fOut = AddGenerationOutput("Out");
            for ( var i = 0; i < _portCount; i++ ) {
                AddGenerationInput(i.ToString(), fOut.Call);
            }
        }
    }
}