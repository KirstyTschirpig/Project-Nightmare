using UnityEditor;
using UnityEngine;

namespace Generation
{
    public class UndoHandler
    {
        public bool undo;
        public UndoHandler(bool undo)
        {
            this.undo = undo;
        }

        public void RegisterUndo(Object target, string undoOperation)
        {
            if (!undo)
                return;
            Undo.RegisterCompleteObjectUndo(target, undoOperation);
        }
    }
}
