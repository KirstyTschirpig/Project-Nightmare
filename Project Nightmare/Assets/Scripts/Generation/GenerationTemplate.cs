using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Generation.Editor;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Generation
{
    public class GenerationTemplate : ScriptableObject
    {
        private UndoHandler undoHandler;
        private ChildGenerator[] generators;
        private ChildGenerationTemplate[] generationTemplates;

        public ChildGenerator[] Generators
        {
            get { return generators; }
        }

        public ChildGenerationTemplate[] GenerationTemplates
        {
            get { return generationTemplates; }
        }

        public bool ShouldUndo
        {
            get { return undoHandler.undo; }
            set { undoHandler.undo = value; }
        }

        public GenerationTemplate()
        {
            undoHandler = new UndoHandler(true);
            generators = new ChildGenerator[0];
            generationTemplates = new ChildGenerationTemplate[0];
        }

        public Generator AddGenerator(string name)
        {
            return AddGenerator(name, this.generators.Length <= 0 ? new Vector2(200, 0) : generators[generators.Length - 1].Position + new Vector2(35, 65));
        }

        private Generator AddGenerator(string name, Vector2 position)
        {
            Generator generator = new Generator();
            generator.hideFlags = HideFlags.HideInHierarchy;
            generator.name = "Generator";
            if (AssetDatabase.GetAssetPath(this) != null)
                AssetDatabase.AddObjectToAsset(generator, AssetDatabase.GetAssetPath(this));
            AddGenerator(generator, position);
            return generator;
        }

        private void AddGenerator(Generator generator, Vector2 position)
        {
            if (Array.Exists(generators, (Predicate<ChildGenerator>) (childGenerator => childGenerator.Generator == generator)))
            {
                Debug.LogWarning(string.Format("Generator '{0}' already exists in Generation Template '{1}', discarding new generator.", generator.name, name));
            }
            else
            {
                undoHandler.RegisterUndo(this, "Generator added");
                ArrayUtility.Add(ref generators, new ChildGenerator()
                {
                    Generator = generator,
                    Position = position
                });
            }
        }

        public void RemoveGenerator(Generator generator)
        {
            undoHandler.RegisterUndo(this, "Generator removed");
            undoHandler.RegisterUndo(generator, "Generator removed");
            List<ChildGenerator> childGenerators = new List<ChildGenerator>(generators);
            childGenerators.Remove(childGenerators.Find(childGenerator => childGenerator.Generator == generator));
            if (GenerationUtils.AreSameAsset(generator, this)) return;
            Undo.DestroyObjectImmediate(generator);
        }

        public GenerationTemplate AddGenerationTemplate(string name)
        {
            return AddGenerationTemplate(name, Vector2.zero);
        }

        public GenerationTemplate AddGenerationTemplate(string name, Vector2 position)
        {
            GenerationTemplate generationTemplate = CreateInstance<GenerationTemplate>();
            generationTemplate.hideFlags = HideFlags.HideInHierarchy;
            generationTemplate.name = "Generation Template";
            AddGenerationTemplate(generationTemplate, position);
            if (AssetDatabase.GetAssetPath(this) != "")
                AssetDatabase.AddObjectToAsset(generationTemplate, AssetDatabase.GetAssetPath(this));
            return generationTemplate;
        }

        private void AddGenerationTemplate(GenerationTemplate generationTemplate, Vector2 position)
        {
            if (Array.Exists(generationTemplates, (Predicate<ChildGenerationTemplate>) (template => template.GenerationTemplate == generationTemplate)))
            {
                Debug.LogWarning(string.Format("Sub Generation Template '{0}' already exists in Generation Template '{1}', discarding new Generation Template.",
                    generationTemplate.name, this.name));
            }
            else
            {
                undoHandler.RegisterUndo(this, "Generation Template " + generationTemplate.name + " added");
                ArrayUtility.Add(ref generationTemplates, new ChildGenerationTemplate()
                {
                    GenerationTemplate = generationTemplate,
                    Position = position
                });
            }
        }

        public void RemoveGenerationTemplate(GenerationTemplate generationTemplate)
        {
            undoHandler.RegisterUndo(this, "GenerationTemplate removed");
            undoHandler.RegisterUndo(generationTemplate, "GenerationTemplate removed");
            List<ChildGenerationTemplate> childGenerationTemplates = new List<ChildGenerationTemplate>(generationTemplates);
            childGenerationTemplates.Remove(childGenerationTemplates.Find(child => child.GenerationTemplate == generationTemplate));
            if (GenerationUtils.AreSameAsset(generationTemplate, this)) return;
            Undo.DestroyObjectImmediate(generationTemplate);
        }

        [MenuItem("CONTEXT/Create/ProjectNightmare/Generation Template")]
        public static void CreateGenerationTemplate()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            CreateGenerationTemplateAtPath(path + "/GenerationTemplate.asset");
        }

        public static GenerationTemplate CreateGenerationTemplateAtPath([NotNull] string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            GenerationTemplate template = CreateInstance<GenerationTemplate>();
            template.name = "Generation Template";
            AssetDatabase.CreateAsset(template, path);
            template.ShouldUndo = false;
            template.AddGenerator("Base Generator");
            template.ShouldUndo = true;
            return template;
        }
    }
}
