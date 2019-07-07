using System;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UIElements;

namespace Generation.Editor.Graphs
{
    public class GenerationEditorWindow : EditorWindow
    {
        private GenerationGraph generationGraph;
        private GenerationGraphGUI generationGraphGUI;
        
        [MenuItem("Window/ProjectNightmare/Generation", false, 2)]
        public static void InitWindow()
        {
            //Create the window and default dock it in the same tab as the scene view.
            GetWindow<GenerationEditorWindow>(typeof(SceneView));
        }

        public void OnGUI()
        {
            EventType type = Event.current.type;
            int button = Event.current.button;
            autoRepaintOnSceneChange = true;
            
            generationGraphGUI.BeginGraphGUI(this, new Rect(0, 0, position.width, position.height - 100));
            
            generationGraphGUI.EndGraphGUI();
            
            generationGraphGUI.BeginToolbarGUI(new Rect(0, position.height - 100, position.width, 100));

            GUI.Button(new Rect(0, position.height - 80, position.width, 80), "1");
            
            generationGraphGUI.EndToolbarGUI();
        }

        public void OnEnable()
        {
            Debug.Log("Init");
            titleContent = new GUIContent("Constraints Graph");
            Init();
        }

        private void Init()
        {
            if (generationGraph == null)
            {
                generationGraph = CreateInstance<GenerationGraph>();
                generationGraph.hideFlags = HideFlags.HideAndDontSave;
            }

            if (generationGraphGUI == null)
            {
                generationGraphGUI = (GenerationGraphGUI) generationGraph.GetEditor();
            }
        }

        private void SetupGUI()
        {
            
        }
        
    }
}
