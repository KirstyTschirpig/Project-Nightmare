//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using DungeonArchitect.Splatmap;

namespace DungeonArchitect.Editors
{
    /// <summary>
    /// Custom property editor for the dungeon game object
    /// </summary>
	[CustomEditor(typeof(Dungeon))]
	public class DungeonPropertyEditor : Editor {

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			
			if (GUILayout.Button ("Build Dungeon")) {
				BuildDungeon();
			}
			if (GUILayout.Button ("Destroy Dungeon")) {
				DestroyDungeon ();
			}
		}

        protected virtual void OnEnable()
        {
            //EditorApplication.update += EditorUpdate;
        }
        
        protected virtual void OnDisable()
        {
            //EditorApplication.update -= EditorUpdate;
        }

        void EditorUpdate()
        {
            var dungeon = target as Dungeon;
            dungeon.Update();
        }
        
        void BuildDungeon() {
			// Make sure we have a theme defined
			Dungeon dungeon = target as Dungeon;
			if (dungeon != null) {
				if (HasValidThemes(dungeon)) {
                    // Create the splat maps for this dungeon, if necessary
                    var splatComponent = dungeon.GetComponent<DungeonSplatmap>();
                    SplatmapPropertyEditor.CreateSplatMapAsset(splatComponent);

                    // Build the dungeon
                    Undo.RecordObjects(new Object[] { dungeon, dungeon.ActiveModel }, "Dungeon Built");
					dungeon.Build(new EditorDungeonSceneObjectInstantiator());
                    DungeonEditorHelper.MarkSceneDirty();

                    // Mark the splatmaps as dirty
                    if (splatComponent != null && splatComponent.splatmap != null)
                    {
                        EditorUtility.SetDirty(splatComponent.splatmap);
                    }
				} 
				else {
					Highlighter.Highlight ("Inspector", "Dungeon Themes");

					// Notify the user that atleast one theme needs to be set
					EditorUtility.DisplayDialog("Dungeon Architect", "Please assign atleast one Dungeon Theme before building", "Ok");
				}
			}
		}

		IEnumerator StopHighlighter() {
			yield return new WaitForSeconds(2);
			Highlighter.Stop();
		}

		void DestroyDungeon() {
			Dungeon dungeon = target as Dungeon;
            if (dungeon != null)
            {
                Undo.RecordObjects(new Object[] { dungeon, dungeon.ActiveModel }, "Dungeon Destroyed");
                dungeon.DestroyDungeon();
                EditorUtility.SetDirty(dungeon.gameObject);
            }
		}

		bool HasValidThemes(Dungeon dungeon) {
            var builder = dungeon.gameObject.GetComponent<DungeonBuilder>();
            if (builder != null && !builder.IsThemingSupported())
            {
                // Theming is not supported in this builder. empty theme configuration would do
                return true;
            }

            if (dungeon.dungeonThemes == null) return false;
			foreach (var theme in dungeon.dungeonThemes) {
				if (theme != null) {
					return true;
				}
			}
			return false;
		}

	}
}
