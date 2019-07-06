using System.Collections;
using System.Collections.Generic;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.Assertions;

namespace Generation.Editor.Graphs
{
    public class ConstraintsGraphEditorGraph : UnityEditor.Graphs.Graph
    {
        private Generator activeGenerator;

        public UnityEditor.Graphs.GraphGUI GetEditor()
        {
            GraphGUI instance = CreateInstance<GraphGUI>();
            instance.graph = this;
            instance.hideFlags = HideFlags.HideAndDontSave;
            return instance;
        }

        public void BuildGraphFromGenerator(Generator constraintsGraph)
        {
            Assert.IsNotNull(constraintsGraph);
            activeGenerator = constraintsGraph;
        }

        private void CreateNodes()
        {
//            foreach(Generator generator in activeGenerator)
        }

        private void CreateNodeFromSubGenerator()
        {
            
        }
    }
}
