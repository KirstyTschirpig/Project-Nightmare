using System.Collections.Generic;
using Generation.Graphs;
using UnityEngine;
using ParadoxNotion.Design;

namespace Generation.Nodes
{

    [Description("Split the Flow in multiple directions. Calls all outputs in the same frame but in order")]
    [Name("Split", 90)]
    public class Split : GenerationControlNode
    {

        [SerializeField]
        [ExposeField]
        [GatherPortsCallback]
        [MinValue(2)]
        [DelayedField]
        private int _portCount = 4;

        protected override void RegisterPorts() {
            var outs = new List<GenerationOutput>();
            for ( var i = 0; i < _portCount; i++ ) {
                outs.Add(AddGenerationOutput(i.ToString()));
            }
            AddGenerationInput("In", (f) =>
            {
                for ( var i = 0; i < _portCount; i++ ) {
                    if ( !graph.isRunning ) {
                        break;
                    }
                    outs[i].Call(f);
                }
            });
        }
    }
}