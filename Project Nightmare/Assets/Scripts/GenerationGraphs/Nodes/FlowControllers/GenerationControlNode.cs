using Generation.Graphs;
using ParadoxNotion.Design;

namespace Generation.Nodes
{

    [Category("Flow Controllers")]
    [Color("bf7fff")]
    [ContextDefinedInputs(typeof(GenerationFlow))]
    [ContextDefinedOutputs(typeof(GenerationFlow))]
    abstract public class GenerationControlNode : GenerationNode { }
}