using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Generation
{
    public class Generator : ScriptableObject
    {
        private GeneratorComposition[] compositions;
        private UndoHandler undoHandler;

        public Generator()
        {
            compositions = new GeneratorComposition[0];
            undoHandler = new UndoHandler(true);
        }

        public void AddComposition(GeneratorComposition composition)
        {
            undoHandler.RegisterUndo(this, "Composition added");
            ArrayUtility.Add(ref compositions, composition);
        }

        public void RemoveComposition(GeneratorComposition composition)
        {
            undoHandler.RegisterUndo(this, "Composition removed");
            ArrayUtility.Remove(ref compositions, composition);
            if (GenerationUtils.AreSameAsset(composition, this)) return;
            Undo.DestroyObjectImmediate(composition);
        }

        public GeneratorComposition CreateCompositioin()
        {
            GeneratorComposition newComposition = new GeneratorComposition();
            if (AssetDatabase.GetAssetPath(this) != "")
                AssetDatabase.AddObjectToAsset(newComposition, AssetDatabase.GetAssetPath(this));
            newComposition.hideFlags = HideFlags.HideInHierarchy;
            return newComposition;
        }
    }
}
