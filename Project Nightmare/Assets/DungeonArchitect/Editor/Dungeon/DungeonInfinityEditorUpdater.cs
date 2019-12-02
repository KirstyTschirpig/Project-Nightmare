using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DungeonArchitect.Editors
{
    [CustomEditor(typeof(InfinityDungeonEditorUpdate))]
    public class DungeonInfinityEditorUpdater : Editor
    {

        protected virtual void OnEnable()
        {
            EditorApplication.update += EditorUpdate;
        }

        protected virtual void OnDisable()
        {
            EditorApplication.update -= EditorUpdate;
        }

        void EditorUpdate()
        {
            var updater = target as InfinityDungeonEditorUpdate;
            updater.EditorUpdate();
        }

    }
}
