//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using UnityEditor;

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using DungeonArchitect;
using DungeonArchitect.Utils;
using DungeonArchitect.Graphs;
using DungeonArchitect.SpatialConstraints;

namespace DungeonArchitect.Editors
{
    /// <summary>
    /// Custom property editor for visual nodes
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(VisualNode))]
    public class VisualNodeEditor : PlaceableNodeEditor
	{
        SerializedProperty IsStatic;
        SerializedProperty affectsNavigation;
        SerializedProperty useSpatialConstraint;
        InstanceCache instanceCache = new InstanceCache();
        bool foldoutSpatialAdvanced = false;

        public override void OnEnable()
        {
            base.OnEnable();
            drawOffset = true;
			drawAttachments = true;
            IsStatic = sobject.FindProperty("IsStatic");
            affectsNavigation = sobject.FindProperty("affectsNavigation");
            useSpatialConstraint = sobject.FindProperty("useSpatialConstraint");
        }

        protected override void DrawPreInspectorGUI()
		{
			EditorGUILayout.PropertyField(IsStatic);

			// affectsNavigation flag is only valid if the object is static.  So disable it if not static
			GUI.enabled = IsStatic.boolValue;
			EditorGUILayout.PropertyField(affectsNavigation);
			GUI.enabled = true;

            GUILayout.Space(CATEGORY_SPACING);
        }
        protected override void DrawPostInspectorGUI()
        {
            GUILayout.Label("Rules", EditorStyles.boldLabel);

            var meshNode = target as VisualNode;
            DrawRule<SelectorRule>(" Selection Rule", ref meshNode.selectionRuleEnabled, ref meshNode.selectionRuleClassName);
            DrawRule<TransformationRule>(" Transform Rule", ref meshNode.transformRuleEnabled, ref meshNode.transformRuleClassName);

            GUI.enabled = true;

            GUILayout.Space(CATEGORY_SPACING);


            DrawSpatialConstraintCategory();
        }

        void DrawSpatialConstraintCategory()
        {
            EditorGUI.indentLevel++;
            GUILayout.Label("SpatialConstraint", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(useSpatialConstraint);

            var visualNode = target as VisualNode;
            if (visualNode.useSpatialConstraint && visualNode.spatialConstraint != null)
            {
                visualNode.spatialConstraint.rotateToFit = EditorGUILayout.Toggle("Rotate to Fit?", visualNode.spatialConstraint.rotateToFit);
                visualNode.spatialConstraint.applyFitRotation = EditorGUILayout.Toggle("Apply Fit Rotation?", visualNode.spatialConstraint.applyFitRotation);

                foldoutSpatialAdvanced = EditorGUILayout.Foldout(foldoutSpatialAdvanced, "Advanced");
                if (foldoutSpatialAdvanced)
                {
                    EditorGUI.indentLevel++;
                    visualNode.spatialConstraint.checkRelativeToMarkerRotation = EditorGUILayout.Toggle("Check relative to marker rotation?", visualNode.spatialConstraint.checkRelativeToMarkerRotation);
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUI.indentLevel--;
        }

        string GetTypeName(System.Type type)
        {
            var meta = System.Attribute.GetCustomAttribute(type, typeof(MetaAttribute)) as MetaAttribute;
            if (meta != null)
            {
                return meta.displayText;
            }
            return type.Name;
        }
        

        void DrawRule<T>(string caption, ref bool ruleEnabled, ref string ruleClassName) where T : ScriptableObject
        {
            GUI.enabled = true;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(CATEGORY_SPACING);
            ruleEnabled = EditorGUILayout.ToggleLeft(caption, ruleEnabled);
            GUI.enabled = ruleEnabled; 
            MonoScript script = null;
            if (ruleClassName != null)
            {
                var rule = instanceCache.GetInstance(ruleClassName) as ScriptableObject;
                if (rule != null)
                {
                    script = MonoScript.FromScriptableObject(rule);
                } 
            }
            var oldScript = script;
            script = EditorGUILayout.ObjectField(script, typeof(MonoScript), false) as MonoScript;
            if (oldScript != script && script != null)
            {
                ruleClassName = script.GetClass().FullName;
            }
            else if (script == null)
            {
                ruleClassName = null;
            }

            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;
        }
    }

    /// <summary>
    /// Renders a visual node
    /// </summary>
    public abstract class VisualNodeRenderer : GraphNodeRenderer
    {
		protected virtual void DrawFrameTexture(GraphRendererContext rendererContext, GraphNode node, GraphCamera camera) {
			DrawNodeTexture(rendererContext, node, camera, DungeonEditorResources.TEXTURE_GO_NODE_FRAME);
		}

		protected virtual void DrawBackgroundTexture(GraphRendererContext rendererContext, GraphNode node, GraphCamera camera) {
			DrawNodeTexture(rendererContext, node, camera, DungeonEditorResources.TEXTURE_GO_NODE_BG);
		}

		protected virtual void DrawThumbnail(GraphRendererContext rendererContext, GraphNode node, GraphCamera camera) {
			var thumbObject = GetThumbObject(node);
			var visualNode = node as VisualNode;
			var thumbnailSize = 96 / camera.ZoomLevel;
			if (thumbObject != null)
			{
				Texture texture = AssetThumbnailCache.Instance.GetThumb(thumbObject);
				if (texture != null)
				{
					var positionWorld = new Vector2(12, 12) + visualNode.Position;
					var positionScreen = camera.WorldToScreen(positionWorld);
					GUI.DrawTexture(new Rect(positionScreen.x, positionScreen.y, thumbnailSize, thumbnailSize), texture);
				}
			}
			else
			{
				DrawTextCentered(rendererContext, node, camera, "None");
			}
		}

        public override void Draw(GraphRendererContext rendererContext, GraphNode node, GraphCamera camera)
        {
            DrawBackgroundTexture(rendererContext, node, camera);

			DrawThumbnail(rendererContext, node, camera);

			DrawFrameTexture(rendererContext, node, camera);

            base.Draw(rendererContext, node, camera);

            if (node.Selected)
            {
                DrawNodeTexture(rendererContext, node, camera, DungeonEditorResources.TEXTURE_GO_NODE_SELECTION);
            }
        }

        protected abstract Object GetThumbObject(GraphNode node);
    }
}
