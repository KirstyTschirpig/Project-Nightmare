using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Generation.Editor.Graphs
{
    public class GenerationTemplateNode : UnityEditor.Graphs.Node
    {
        private UndoHandler undoHandler = new UndoHandler(true);
    }
}
