using System.Collections;
using System.Collections.Generic;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.Assertions;

namespace Generation.Editor.Graphs
{
    public class GenerationGraph : UnityEditor.Graphs.Graph
    {
        private GenerationTemplate activeTemplate;
        private Dictionary<Generator, GeneratorNode> generatorNodeLookup = new Dictionary<Generator, GeneratorNode>();
        private Dictionary<GenerationTemplate, GenerationTemplateNode> generationTemplateNodeLookup = new Dictionary<GenerationTemplate, GenerationTemplateNode>();

        public UnityEditor.Graphs.GraphGUI GetEditor()
        {
            GraphGUI instance = CreateInstance<GenerationGraphGUI>();
            instance.graph = this;
            instance.hideFlags = HideFlags.HideAndDontSave;
            return instance;
        }

        public void RebuildGraph()
        {
            if (activeTemplate != null) BuildGraphFromGenerationTemplate(activeTemplate);
            else Clear(false);
        }

        public void BuildGraphFromGenerationTemplate(GenerationTemplate template)
        {
            Assert.IsNotNull(template);
            Clear(false);
            activeTemplate = template;

            
            CreateNodes();
        }

        private void CreateNodes()
        {
            generatorNodeLookup.Clear();
            generationTemplateNodeLookup.Clear();
            foreach (ChildGenerator generator in activeTemplate.Generators)
            {
                CreateNodeFromGenerator(generator);
            }

            foreach (ChildGenerationTemplate generationTemplate in activeTemplate.GenerationTemplates)
            {
                CreateNodeFromGenerationTemplate(generationTemplate);
            }

        }

        private void CreateNodeFromGenerationTemplate(ChildGenerationTemplate generationTemplate)
        {
        }

        private void CreateNodeFromGenerator(ChildGenerator generator)
        {
            if (generatorNodeLookup.ContainsKey(generator.Generator))
            {
                Debug.LogWarningFormat("The generation template already containts the generator '{0}', this will not behave as intended!", generator.Generator.name);
            }
            else
            {
                GeneratorNode node = CreateAndAddNode<GeneratorNode>("", generator.Position);
                node.Generator = generator.Generator;
                generatorNodeLookup.Add(generator.Generator, node);
            }
        }

        private T CreateAndAddNode<T>(string name, Vector2 position) where T : Node
        {
            T instance = ScriptableObject.CreateInstance<T>();
            instance.hideFlags = HideFlags.HideAndDontSave;
            instance.name = name;
            instance.position = new Rect(position.x, position.y, 200f, 50f);
            instance.AddInputSlot("In");
            instance.AddOutputSlot("Out");
            AddNode(instance);
            return instance;
        }
    }
}

