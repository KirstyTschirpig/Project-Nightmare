using System;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;

namespace Generation.Editor.Graphs
{
    public class ConstraintsGraphEditorWindow : EditorWindow
    {
        [MenuItem("Window/ProjectNightmare/Constraints Graph", false, 2)]
        public static void InitWindow()
        {
            //Create the window and default dock it in the same tab as the scene view.
            GetWindow<ConstraintsGraphEditorWindow>(new System.Type[1]
            {
                typeof(SceneView)
            });
        }

        public void OnGUI()
        {
            EventType type = Event.current.type;
            int button = Event.current.button;
            this.autoRepaintOnSceneChange = true;
        }

        public void OnEnable()
        {
            this.titleContent = new GUIContent("Constraints Graph");
            this.Init();
        }

        private void Init()
        {
            throw new NotImplementedException();
        }
    }
}
