using DungeonArchitect.Editors.UI.Widgets;
using DungeonArchitect.Grammar;
using DungeonArchitect.Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.DungeonFlow
{
    [CustomEditor(typeof(GrammarTaskNode))]
    public class GrammarTaskNodeEditor : Editor
    {
        SerializedObject sobject;
        SerializedProperty executionIndex;

        public void OnEnable()
        {
            sobject = new SerializedObject(target);
            executionIndex = sobject.FindProperty("executionIndex");
        }

        public override void OnInspectorGUI()
        {
            sobject.Update();

            GUILayout.Label("Task Node", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(executionIndex);
            sobject.ApplyModifiedProperties();
        }
    }

}
