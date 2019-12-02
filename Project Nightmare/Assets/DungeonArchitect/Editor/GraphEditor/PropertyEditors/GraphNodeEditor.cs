//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using UnityEditor;
using DungeonArchitect.Utils;
using System.Collections;
using DungeonArchitect.Graphs;

namespace DungeonArchitect.Editors
{
    public class GraphNodeEditor : Editor
    {
        protected SerializedObject sobject;

        public virtual void OnEnable()
        {
            sobject = new SerializedObject(targets);
        }


        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            sobject.Update();

            HandleInspectorGUI();

            sobject.ApplyModifiedProperties();
            
            if (EditorGUI.EndChangeCheck())
            {
                OnGuiChanged();
            }
        }

        protected virtual void OnGuiChanged()
        {
        }

        protected virtual void HandleInspectorGUI() { }
    }
}
