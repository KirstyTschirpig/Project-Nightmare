using DungeonArchitect.Editors.UI.Widgets;
using DungeonArchitect.Grammar;
using DungeonArchitect.Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.DungeonFlow
{
    [CustomEditor(typeof(GrammarExecRuleNode))]
    public class GrammarExecRuleNodeEditor : Editor
    {
        SerializedObject sobject;
        SerializedProperty runMode;
        SerializedProperty runProbability;
        SerializedProperty iterateCount;
        SerializedProperty minIterateCount;
        SerializedProperty maxIterateCount;

        public void OnEnable()
        {
            sobject = new SerializedObject(target);
            runMode = sobject.FindProperty("runMode");
            runProbability = sobject.FindProperty("runProbability");
            iterateCount = sobject.FindProperty("iterateCount");
            minIterateCount = sobject.FindProperty("minIterateCount");
            maxIterateCount = sobject.FindProperty("maxIterateCount");
        }

        public override void OnInspectorGUI()
        {
            sobject.Update();

            var node = target as GrammarExecRuleNode;

            if (node != null)
            {
                GUILayout.Label("Task Node", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(runMode);
                if (node.runMode == GrammarExecRuleRunMode.RunWithProbability)
                {
                    EditorGUILayout.PropertyField(runProbability);
                }
                else if(node.runMode == GrammarExecRuleRunMode.Iterate)
                {
                    EditorGUILayout.PropertyField(iterateCount);
                }
                else if (node.runMode == GrammarExecRuleRunMode.IterateRange)
                {
                    EditorGUILayout.PropertyField(minIterateCount);
                    EditorGUILayout.PropertyField(maxIterateCount);
                }
            }

            sobject.ApplyModifiedProperties();
        }
    }

}
