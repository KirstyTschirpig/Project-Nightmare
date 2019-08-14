using System.Collections;
using ParadoxNotion.Design;

namespace Generation.Nodes
{

    [Description("Stops and cease execution of the FlowSript")]
    public class Finish : GenerationControlNode
    {
        protected override void RegisterPorts() {
            var c = AddValueInput<bool>("Success");
            AddGenerationInput("In", (f) => { graph.Stop(c.value); });
        }
    }
}