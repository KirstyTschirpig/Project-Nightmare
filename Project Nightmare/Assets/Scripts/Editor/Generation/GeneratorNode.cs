using System.Collections;
using System.Collections.Generic;
using Generation;
using UnityEngine;

namespace Generation.Editor.Graphs
{
    public class GeneratorNode : UnityEditor.Graphs.Node
    {
        private UndoHandler undoHandler = new UndoHandler(true);
        public Generator Generator;
    }
}
