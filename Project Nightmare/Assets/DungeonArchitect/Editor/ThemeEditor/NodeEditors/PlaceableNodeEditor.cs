//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using UnityEditor;
using DungeonArchitect.Utils;
using System.Collections;
using DungeonArchitect.Graphs;

namespace DungeonArchitect.Editors
{
    /// <summary>
    /// Custom property editor for placeable node
    /// </summary>
    public class PlaceableNodeEditor : GraphNodeEditor
    {
        SerializedProperty ConsumeOnAttach;
        SerializedProperty AttachmentProbability;
        protected bool drawOffset = false;
        protected bool drawAttachments = false;

        public override void OnEnable()
        {
            base.OnEnable();

            ConsumeOnAttach = sobject.FindProperty("consumeOnAttach");
            AttachmentProbability = sobject.FindProperty("attachmentProbability");
        }

        protected override void HandleInspectorGUI() {
            DrawPreInspectorGUI();

            if (drawOffset)
            {
                // Draw the transform offset editor
                GUILayout.Label("Offset", EditorStyles.boldLabel);
                if (targets != null && targets.Length > 1)
                {
                    // Multiple object editing not supported
                    EditorGUILayout.HelpBox("Multiple Objects selected", MessageType.Info);
                }
                else
                {
                    var node = target as PlaceableNode;
                    InspectorUtils.DrawMatrixProperty("Offset", ref node.offset);
                    GUILayout.Space(CATEGORY_SPACING);
                }
            }

            if (drawAttachments)
            {
                // Draw the attachment properties
                GUILayout.Label("Attachment", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(AttachmentProbability, new GUIContent("Probability"));
                EditorGUILayout.PropertyField(ConsumeOnAttach);
                GUILayout.Space(CATEGORY_SPACING);


                // Clamp the probability to [0..1]
                if (!AttachmentProbability.hasMultipleDifferentValues)
                {
                    AttachmentProbability.floatValue = Mathf.Clamp01(AttachmentProbability.floatValue);
                }
            }

            DrawPostInspectorGUI();
        }

        protected override void OnGuiChanged()
        {
            var themeEditorWindow = DungeonEditorHelper.GetWindowIfOpen<DungeonThemeEditorWindow>();
            if (themeEditorWindow != null)
            {
                var graphEditor = themeEditorWindow.GraphEditor;
                graphEditor.HandleGraphStateChanged();
                graphEditor.HandleNodePropertyChanged(target as GraphNode);
            }
        }

        protected virtual void DrawPreInspectorGUI() { }
        protected virtual void DrawPostInspectorGUI() { }

        protected const int CATEGORY_SPACING = 10;

    }
}
