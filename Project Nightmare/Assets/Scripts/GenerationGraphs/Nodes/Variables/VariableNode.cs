using Generation.Graphs;
using ParadoxNotion.Design;

namespace Generation.Nodes
{

    [Category("Variables")]
    abstract public class VariableNode : GenerationNode
    {

        ///For setting the default variable
        abstract public void SetVariable(object o);
    }
}